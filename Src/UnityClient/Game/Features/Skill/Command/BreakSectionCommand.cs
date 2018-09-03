using System;
using System.Collections.Generic;
using Entitas.Data;
using ScriptableData;

namespace SkillCommands
{
    internal class BreakSectionCommand : AbstractCommand
    {
        public void Load(int breakType, long startTime, long endTime)
        {
            m_BreakType = breakType;
            m_StartTime = startTime;
            m_EndTime = endTime;
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num > 2)
            {
                m_BreakType = int.Parse(callData.GetParamId(0));

                m_StartTime = long.Parse(callData.GetParamId(1));
                m_EndTime = long.Parse(callData.GetParamId(2));
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if(null != target)
            {
                if(target.hasSkill && null != target.skill.Instance)
                {
                    BreakSection bs = new BreakSection
                    {
                        BreakType = m_BreakType,
                        StartTime = m_StartTime + instance.Time,
                        EndTime = m_EndTime + instance.Time,
                    };
                    target.skill.Instance.BreakSections.Add(bs);
                }
                else
                {
                    Util.LogUtil.Error("BreakSectionCommand target {0} has no skill or no skillinstance! Something must go wrong!", target);
                }
            }
            return ExecResult.Finished;
        }

        private int m_BreakType = 0;
        private long m_StartTime = 0;
        private long m_EndTime = 0;
    }
}
