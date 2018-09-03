using System;
using System.Collections.Generic;
using UnityClient;
using ScriptableData;
using Entitas;

namespace SkillCommands
{
    internal class SelfBuffCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1)
                m_BuffId = int.Parse(callData.GetParamId(0));
        }

        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {

            if(instance.Target is GameEntity obj)
            {
                BuffSystem.Instance.StartBuff(obj, obj, m_BuffId, obj.position.Value, obj.rotation.Value);
            }
            return ExecResult.Finished;
        }

        private int m_BuffId;
    }
}
