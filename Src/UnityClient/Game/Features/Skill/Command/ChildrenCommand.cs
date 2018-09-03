using System;
using System.Collections.Generic;
using ScriptableData;
using UnityEngine;

namespace SkillCommands
{
    public class ChildrenCommand : AbstractCommand
    {
        public void Load(string retValueName)
        {
            m_RetString = retValueName;
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
                m_RetString = callData.GetParamId(0);
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null == obj)
                return ExecResult.Finished;

            List<uint> findedTargets = new List<uint>();

            var entities = Contexts.sharedInstance.game.GetEntitiesWithParent(obj.id.value);
            foreach(var entity in entities)
            {
                findedTargets.Add(entity.id.value);
            }

            instance.AddVariable(m_RetString, findedTargets);

            return ExecResult.Finished;

        }

        private string m_RetString = string.Empty;
    }
}
