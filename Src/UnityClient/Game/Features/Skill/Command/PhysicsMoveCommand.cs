using System;
using System.Collections.Generic;
using UnityEngine;
using ScriptableData;

namespace SkillCommands
{
    public class PhysicsMoveCommand : AbstractCommand
    {
        public void Load(Vector3 speed)
        {
            m_Speed = speed;
        }
        protected override void Load(CallData callData)
        {
            if (callData.GetParamNum() > 1)
            {
                m_RemainTime = long.Parse(callData.GetParamId(0));
                m_Speed = ScriptableDataUtility.CalcVector3(callData.GetParam(1) as CallData);
            }
        }

        protected override void ResetState()
        {
            m_Inited = false;
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (null == target)
            {
                return ExecResult.Finished;
            }
            if (!m_Inited)
            {
                target.physics.Rigid.Velocity = Quaternion.Euler(0, Mathf.Rad2Deg * target.rotation.Value, 0) * m_Speed;
                target.physics.Rigid.IsKinematic = false;
                m_StartTime = instance.Time;
                m_Inited = true;
            }

            if (instance.Time - m_StartTime > m_RemainTime)
            {
                return ExecResult.Finished;
            }

            if (!target.hasPhysics)
            {
                return ExecResult.Finished;
            }

            instance.Velocity = target.physics.Rigid.Velocity;

            return ExecResult.Parallel;

        }

        private bool m_Inited = false;
        private long m_StartTime = 0;
        private Vector3 m_Speed = Vector3.zero;
        private long m_RemainTime = 0;
    }
}
