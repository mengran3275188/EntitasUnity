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
            m_InputContext = contexts.input;
        }
        public void Initialize()
        {
            Services.Instance.TimeService.Initialize();
        }
        public void Execute()
        {
            Services.Instance.TimeService.Execute();

            float curTime = m_InputContext.time.Value;

            behaviac.Workspace.Instance.DoubleValueSinceStartup = curTime * 1000f;

        }

        private readonly InputContext m_InputContext;
    }
}
