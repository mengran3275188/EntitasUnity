using System;
using System.Collections.Generic;
using ScriptableData;
using UnityEngine;

namespace SkillCommands
{
    public class FindTargetCommand : AbstractCommand
    {
        public void Load(Vector3 offset, float radius, string retValueName)
        {
            m_Offset = offset;
            m_Radius = radius;
            m_ObjIdVarName = retValueName;
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 2)
            {
                m_Offset = ScriptableDataUtility.CalcVector3(callData.GetParam(0) as ScriptableData.CallData);
                m_Radius = float.Parse(callData.GetParamId(1));
            }
        }
        protected override void Load(FunctionData funcData)
        {
            CallData callData = funcData.Call;
            if(null != callData)
            {
                Load(callData);
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
            if(callData.GetId() == "ret" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName = callData.GetParamId(0);
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null == obj)
                return ExecResult.Finished;

            List<uint> findedTargets = new List<uint>();


            Vector3 center = obj.position.Value + Quaternion.Euler(0, Mathf.Rad2Deg * obj.rotation.Value, 0) * m_Offset;

            var entities = Contexts.sharedInstance.game.GetGroup(GameMatcher.Position);
            foreach(var entity in entities)
            {
                if (entity == obj)
                    continue;
                if (entity.hasCamp && obj.hasCamp && entity.camp.Value == obj.camp.Value)
                    continue;

                if (entity.hasDead)
                    continue;

                if(ExtensionUnityVector3.DistanceXZ(center, entity.position.Value) < m_Radius)
                {
                    findedTargets.Add(entity.id.value);
                }
            }

            instance.AddVariable(m_ObjIdVarName, findedTargets);

            return ExecResult.Finished;
        }

        private Vector3 m_Offset;
        private float m_Radius;
        private string m_ObjIdVarName = string.Empty;
    }
}
