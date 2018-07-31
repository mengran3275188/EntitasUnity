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
            m_GameStateContext = contexts.gameState;
        }
        public void Initialize()
        {
            m_GameStateContext.SetTimeInfo(GfxSystemImpl.Gfx.GetTime(), 0);
        }
        public void Execute()
        {
            float curTime = GfxSystemImpl.Gfx.GetTime();

            behaviac.Workspace.Instance.DoubleValueSinceStartup = curTime * 1000f;

            m_GameStateContext.ReplaceTimeInfo(curTime, curTime - m_GameStateContext.timeInfo.Time);
        }

        private readonly GameStateContext m_GameStateContext;
    }
}
