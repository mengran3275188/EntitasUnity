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
            m_GameContext.SetTimeInfo(GfxSystemImpl.Gfx.GetTime(), 0);
        }
        public void Execute()
        {
            float curTime = GfxSystemImpl.Gfx.GetTime();

            behaviac.Workspace.Instance.DoubleValueSinceStartup = curTime * 1000f;

            m_GameContext.ReplaceTimeInfo(curTime, curTime - m_GameContext.timeInfo.Time);
        }

        private readonly GameContext m_GameContext;
    }
}
