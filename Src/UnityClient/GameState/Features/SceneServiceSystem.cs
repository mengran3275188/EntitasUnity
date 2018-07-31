using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class SceneServiceSystem : IInitializeSystem, IExecuteSystem
    {
        public SceneServiceSystem(Contexts contexts)
        {
        }
        public void Initialize()
        {
            Services.Instance.UIService.Init();
        }
        public void Execute()
        {
            Services.Instance.SceneService.Tick();
        }
    }
}
