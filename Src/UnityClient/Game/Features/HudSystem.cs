using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class HudSystem : IExecuteSystem
    {
        public HudSystem(Contexts contexts)
        {
        }
        public void Execute()
        {
            Services.Instance.HudService.Tick(Services.Instance.CameraService);
        }
    }
}
