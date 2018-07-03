using System;
using System.Collections.Generic;
using ScriptableData;
using ScriptableSystem;
using Entitas;
using Entitas.Component;
using Util;

namespace SkillCommands
{
    internal class AreaDamageCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 3)
            {
                m_RelativeCenter = ScriptableDataUtility.CalcVector3(callData.GetParam(0) as ScriptableData.CallData);
                m_Range = float.Parse(callData.GetParamId(1));
                m_ImpactId = int.Parse(callData.GetParamId(2));
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if(null != obj)
            {
                Quaternion quaternion = Quaternion.CreateFromYawPitchRoll(obj.rotation.RotateDir, 0, 0);
                Vector3 center = obj.position.Value + quaternion * m_RelativeCenter;

                var entities = Contexts.sharedInstance.game.GetGroup(GameMatcher.Position);
                foreach(var entity in entities)
                {
                    if (entity == obj)
                        continue;

                    if(InCircle(center, m_Range, entity.position))
                    {
                        LogUtil.Debug("AreaDamageCommand.ExecCommand : find damage target {0}", entity.ToString());
                        UnityClient.ImpactSystem.Instance.StartBuff(obj, entity, 1);
                    }
                }
            }
            return ExecResult.Finished;
        }

        private bool InCircle(Vector3 point, float range, PositionComponent position)
        {
            return Vector3.Distance(point, position.Value) < range;
        }

        private Vector3 m_RelativeCenter;
        private float m_Range;
        private int m_ImpactId;
    }
}
