using System;
using System.Collections.Generic;
using ScriptableData;
using Entitas.Data;
using UnityClient;
using UnityEngine;

namespace SkillCommands
{
    internal class AreaDamageCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            AreaDamageCommand command = new AreaDamageCommand
            {
                m_RelativeCenter = m_RelativeCenter,
                m_Range = m_Range
            };

            foreach (var pair in m_StateImpacts)
            {
                command.m_StateImpacts[pair.Key] = pair.Value;
            }
            return command;
        }
        public void Load(Vector3 offset, float radius, params StateBuff[] stateBuffs)
        {
            m_RelativeCenter = offset;
            m_Range = radius;
            foreach(var stateBuff in stateBuffs)
            {
                m_StateImpacts[stateBuff.m_State] = stateBuff;
            }
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 2)
            {
                m_RelativeCenter = ScriptableDataUtility.CalcVector3(callData.GetParam(0) as ScriptableData.CallData);
                m_Range = float.Parse(callData.GetParamId(1));
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
                    if (funcData.Statements[i] is ScriptableData.CallData stCall)
                    {
                        string id = stCall.GetId();
                        if (id == "statebuff")
                        {
                            StateBuff stateBuff = ScriptableDataUtility.CalcStateBuff(stCall);
                            m_StateImpacts[stateBuff.m_State] = stateBuff;
                        }
                    }
                }
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            if (instance.Target is GameEntity obj)
            {
                Quaternion quaternion = Quaternion.Euler(0, Mathf.Rad2Deg * obj.rotation.Value, 0);
                Vector3 center = obj.position.Value + quaternion * m_RelativeCenter;

                UnityClient.GfxSystem.DrawCircle(center, m_Range, 2.0f);

                var entities = Contexts.sharedInstance.game.GetGroup(GameMatcher.Position);
                foreach (var entity in entities)
                {
                    if (entity == obj)
                        continue;
                    if (entity.hasCamp && obj.hasCamp && entity.camp.Value == obj.camp.Value)
                        continue;

                    if (InCircle(center, m_Range, entity.position))
                    {
                        if (!entity.hasDead)
                        {

                            StateBuff_State state = GetState(entity);

                            if (!m_StateImpacts.TryGetValue(state, out StateBuff stateBuff))
                            {
                                m_StateImpacts.TryGetValue(StateBuff_State.Default, out stateBuff);
                            }
                            if (null != stateBuff)
                            {
                                foreach (int buffId in stateBuff.m_Buffs)
                                    UnityClient.BuffSystem.Instance.StartBuff(obj, entity, buffId, obj.position.Value, obj.rotation.Value);
                            }


                        }
                    }
                }
            }
            return ExecResult.Finished;
        }

        private bool InCircle(Vector3 point, float range, PositionComponent position)
        {
            return ExtensionUnityVector3.DistanceXZ(point, position.Value) < range;
        }

        private StateBuff_State GetState(GameEntity obj)
        {
            return obj.skill== null || obj.skill.Instance == null ? StateBuff_State.Default : StateBuff_State.Skill;
        }

        private Vector3 m_RelativeCenter;
        private float m_Range;

        private Dictionary<StateBuff_State, StateBuff> m_StateImpacts = new Dictionary<StateBuff_State, StateBuff>();
    }
}
