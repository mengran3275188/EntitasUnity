using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class SceneServiceSystem : IExecuteSystem
    {
        public SceneServiceSystem(Contexts contexts)
        {
        }
        public void Execute()
        {
            Services.Instance.SceneService.Tick();
        }
    }
}
