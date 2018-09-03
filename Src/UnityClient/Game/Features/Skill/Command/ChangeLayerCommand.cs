using System;
using ScriptableData;
using UnityEngine;

namespace SkillCommands
{
    internal class ChangeLayerCommand : AbstractCommand
    {
        public void Load(string layerName)
        {
            m_TargetLayer = LayerMask.NameToLayer(layerName);
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num > 0)
            {
                m_TargetLayer = LayerMask.NameToLayer(callData.GetParamId(0));
            }
        }

        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null == obj)
                return ExecResult.Finished;

            if (!obj.hasPhysics)
                return ExecResult.Finished;

            obj.physics.Rigid.Layer = m_TargetLayer;

            return ExecResult.Finished;
        }

        private int m_TargetLayer = 0;
    }
}
