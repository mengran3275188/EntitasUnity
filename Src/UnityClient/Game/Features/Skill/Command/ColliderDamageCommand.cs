using System;
using System.Collections.Generic;
using ScriptableSystem;
using ScriptableData;
using Entitas.Data;
using Util;
using UnityClient;
using UnityEngine;

namespace SkillCommands
{
    public class ColliderDamageCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            ColliderDamageCommand copy = new ColliderDamageCommand();
            copy.m_Offset = m_Offset;
            copy.m_Size = m_Size;
            copy.m_Interval = m_Interval;

            copy.m_HaveObjId = m_HaveObjId;
            copy.m_ObjIdVarName = m_ObjIdVarName.Clone();

            foreach(var pair in m_StateImpacts)
            {
                copy.m_StateImpacts[pair.Key] = pair.Value;
            }

            return copy;
        }
        protected override void UpdateArguments(object iterator, object[] args)
        {
            if (m_HaveObjId)
                m_ObjIdVarName.Evaluate(iterator, args);
        }
        protected override void UpdateVariables(Instance instance)
        {
            if (m_HaveObjId)
                m_ObjIdVarName.Evaluate(instance);
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 4)
            {
                m_RemainTime = long.Parse(callData.GetParamId(0));
                m_Interval = long.Parse(callData.GetParamId(1));
                m_Offset = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
                m_Size = ScriptableDataUtility.CalcVector3(callData.GetParam(3) as ScriptableData.CallData);
            }
        }
        protected override void Load(FunctionData funcData)
        {
            CallData callData = funcData.Call;
            if(null != callData)
            {
                Load(callData);

                for(int i = 0; i < funcData.Statements.Count; ++i)
                {
                    CallData stCall = funcData.Statements[i] as ScriptableData.CallData;
                    if(null != stCall)
                    {
                        string id = stCall.GetId();
                        if(id == "statebuff")
                        {
                            StateBuff stateBuff = ScriptableDataUtility.CalcStateBuff(stCall);
                            m_StateImpacts[stateBuff.m_State] = stateBuff;
                        }
                    }
                }
            }
        }
        protected override void Load(StatementData statementData)
        {
            if(statementData.Functions.Count == 2)
            {
                FunctionData first = statementData.First;
                FunctionData second = statementData.Second;
                if(null != first && null != first.Call && null != second && null != second.Call)
                {
                    Load(first);
                    LoadVarName(second.Call);
                }
            }
        }
        private void LoadVarName(CallData callData)
        {
            if(callData.GetId() == "objid" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObjId = true;
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            m_Instance = instance;
            m_Target = target;

            Services.Instance.PhysicsService.CreateBoxCollider(target.view.Value, m_RemainTime, m_Size, m_Offset, OnCollision);

            /*

            Jitter.Collision.Shapes.BoxShape shape = new Jitter.Collision.Shapes.BoxShape(m_Size);

            Jitter.Dynamics.Material physicsMaterial = new Jitter.Dynamics.Material();
            physicsMaterial.KineticFriction = 0;
            physicsMaterial.StaticFriction = 0;

            RigidObject rigid = new RigidObject(target.id.value, shape, physicsMaterial, false);
            rigid.IsTrigger = true;
            rigid.Position = target.position.Value + m_Offset;
            rigid.Orientation = Matrix3x3.CreateRotationY(target.rotation.Value);

            target.AddCollision(rigid, m_Offset, OnCollision);

            if (m_HaveObjId)
            {
                int objId = 10086;
                string varName = m_ObjIdVarName.Value;
                if(varName.StartsWith("@") && !varName.StartsWith("@@"))
                {
                    instance.AddLocalVariable(varName, objId);
                }
                else
                {
                    instance.AddGlobalVariable(varName, objId);
                }
            }
            */

            return ExecResult.Finished;
        }

        private void OnCollision(uint targetEntityId)
        {
            GameEntity target = m_Instance.Target as GameEntity;
            GameEntity collideTarget = Contexts.sharedInstance.game.GetEntityWithId(targetEntityId);

            if (null == target || null == collideTarget)
                return;
            if (!target.hasCamp || !collideTarget.hasCamp || target.camp.Value == collideTarget.camp.Value)
                return;

            float time = Contexts.sharedInstance.gameState.timeInfo.Time;

            long lastHitTime = -1;

            if (m_Interval > 0 && m_DamagedEntities.TryGetValue(targetEntityId, out lastHitTime))
            {
                if(time * 1000 < lastHitTime + m_Interval)
                {
                    LogUtil.Info("ColliderDamage.OnCollision drop collide event due to interval.");
                    return;
                }
            }

            if(null !=  target && null != collideTarget && !collideTarget.hasDead)
            {
                StateBuff_State state = GetState(collideTarget);

                StateBuff stateBuff;
                if (!m_StateImpacts.TryGetValue(state, out stateBuff))
                {
                    m_StateImpacts.TryGetValue(StateBuff_State.Default, out stateBuff);
                }
                if(null != stateBuff)
                {
                    foreach (int buffId in stateBuff.m_Buffs)
                        UnityClient.BuffSystem.Instance.StartBuff(target, collideTarget, buffId, target.position.Value, target.rotation.Value);
                }

                m_DamagedEntities[targetEntityId] = (long)(time * 1000);

                m_Instance.SendMessage("oncollision");
            }
        }
        private StateBuff_State GetState(GameEntity obj)
        {
            return obj.skill.Instance == null ? StateBuff_State.Default : StateBuff_State.Skill;
        }

        // config
        private Vector3 m_Offset = Vector3.zero;
        private Vector3 m_Size = Vector3.one;
        private long m_Interval = -1;
        private long m_RemainTime = 1000;
        private Dictionary<StateBuff_State, StateBuff> m_StateImpacts = new Dictionary<StateBuff_State, StateBuff>();

        private IValue<string> m_ObjIdVarName = new SkillValue<string>();
        private bool m_HaveObjId = false;
        private GameEntity m_Target = null;
        private Instance m_Instance = null;
        private Dictionary<uint, long> m_DamagedEntities = new Dictionary<uint, long>();

    }

    internal class RemoveColliderCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if(null != obj && obj.hasCollision)
            {
                obj.RemoveCollision();
            }
            return ExecResult.Finished;
        }
    }
}
