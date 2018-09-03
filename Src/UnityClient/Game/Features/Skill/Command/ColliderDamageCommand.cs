using System;
using System.Collections.Generic;
using ScriptableData;
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
            copy.m_LayerId = m_LayerId;
            copy.m_Offset = m_Offset;
            copy.m_Size = m_Size;
            copy.m_Interval = m_Interval;
            copy.m_RemainTime = m_RemainTime;
            copy.m_Rotation = m_Rotation;

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
        protected override void UpdateVariables(IInstance instance)
        {
            if (m_HaveObjId)
                m_ObjIdVarName.Evaluate(instance);
        }
        internal void Load(int layer, long remainTime, long damageInterval, Vector3 offset, Vector3 size, params StateBuff[] stateBuffs)
        {
            m_LayerId = layer;
            m_RemainTime = remainTime;
            m_Interval = damageInterval;
            m_Offset = offset;
            m_Size = size;
            m_Rotation = Quaternion.identity;
            foreach (var stateBuff in stateBuffs)
                m_StateImpacts[stateBuff.m_State] = stateBuff;

        }
        internal void Load(int layer, long remainTime, long damageInterval, Vector3 startPos, Vector3 endPos, float radius, params StateBuff[] stateBuffs)
        {
            m_LayerId = layer;
            m_RemainTime = remainTime;
            m_Interval = damageInterval;

            m_Offset = (startPos + endPos) / 2;
            m_Size = new Vector3(Vector3.Distance(startPos, endPos), radius, radius);

            m_Rotation = Quaternion.LookRotation(Vector3.Cross((endPos - startPos), Vector3.up));
            foreach (var stateBuff in stateBuffs)
                m_StateImpacts[stateBuff.m_State] = stateBuff;
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 3)
            {
                m_LayerId = LayerMask.NameToLayer(callData.GetParamId(0));
                m_RemainTime = long.Parse(callData.GetParamId(1));
                m_Interval = long.Parse(callData.GetParamId(2));
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
                        if(id == "box")
                        {
                            int num = stCall.GetParamNum();
                            if(num >= 2)
                            {
                                m_Offset = ScriptableDataUtility.CalcVector3(stCall.GetParam(0) as ScriptableData.CallData);
                                m_Size = ScriptableDataUtility.CalcVector3(stCall.GetParam(0) as ScriptableData.CallData);
                                m_Rotation = Quaternion.identity;
                            }
                        }
                        if(id == "line")
                        {
                            int num = stCall.GetParamNum();
                            if(num >= 3)
                            {
                                Vector3 startPos = ScriptableDataUtility.CalcVector3(stCall.GetParam(0) as ScriptableData.CallData);
                                Vector3 endPos = ScriptableDataUtility.CalcVector3(stCall.GetParam(1) as ScriptableData.CallData);
                                float radius = float.Parse(stCall.GetParamId(2));

                                m_Offset = (startPos + endPos) / 2;
                                m_Size = new Vector3(Vector3.Distance(startPos, endPos), radius, radius);

                                m_Rotation = Quaternion.LookRotation(Vector3.Cross((endPos - startPos), Vector3.up));
                            }
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
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            m_Instance = instance;
            m_Target = target;

            Services.Instance.PhysicsService.CreateBoxCollider(target.view.Value, m_LayerId, m_RemainTime, m_Size, m_Offset, m_Rotation, OnCollision);

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

            float time = Contexts.sharedInstance.input.time.Value;

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
        private Quaternion m_Rotation = Quaternion.identity;
        private long m_Interval = -1;
        private long m_RemainTime = 1000;
        private int m_LayerId = 0;
        private Dictionary<StateBuff_State, StateBuff> m_StateImpacts = new Dictionary<StateBuff_State, StateBuff>();

        private IValue<string> m_ObjIdVarName = new SkillValue<string>();
        private bool m_HaveObjId = false;
        private GameEntity m_Target = null;
        private IInstance m_Instance = null;
        private Dictionary<uint, long> m_DamagedEntities = new Dictionary<uint, long>();

    }
}
