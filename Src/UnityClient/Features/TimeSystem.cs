using System;
using System.Collections.Generic;
using Entitas;
using Util;

namespace UnityClient
{
    public class TimeSystem : IInitializeSystem, IExecuteSystem
    {
        public TimeSystem(Contexts contexts)
        {
            m_GameContext = contexts.game;
        }
        public void Initialize()
        {
            m_GameContext.SetTimeInfo(TimeUtility.Instance.GetLocalSeconds(), 0);
        }
        public void Execute()
        {
            float curTime = TimeUtility.Instance.GetLocalSeconds();

            behaviac.Workspace.Instance.DoubleValueSinceStartup = curTime * 1000;

            m_GameContext.ReplaceTimeInfo(curTime, curTime - m_GameContext.timeInfo.Time);
        }

        private readonly GameContext m_GameContext;
    }
}
