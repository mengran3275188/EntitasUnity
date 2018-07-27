using System;
using System.Collections.Generic;
using ScriptableData;
using ScriptableSystem;
using UnityEngine;

namespace SkillCommands
{
    internal class TeleportCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num > 0)
            {
                m_Offset = ScriptableDataUtility.CalcVector3(callData.GetParam(0) as ScriptableData.CallData);
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if(null != obj)
            {
                Quaternion quaternion = Quaternion.Euler(0, obj.rotation.Value, 0);
                Vector3 targetPostion = obj.position.Value + quaternion * m_Offset;

                if(obj.hasPhysics)
                {
                    obj.physics.Rigid.Position = targetPostion + obj.physics.Offset;
                }
            }
            return ExecResult.Finished;
        }

        private Vector3 m_Offset = Vector3.zero;
    }
}
