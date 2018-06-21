using System;
using System.Collections.Generic;
using Util;
using Entitas;

namespace UnityClient
{
    public class SceneSystem : Singleton<SceneSystem>, IInitializeSystem, IExecuteSystem
    {
        public SceneSystem()
        {
            //Contexts.sharedInstance.game
        }
        public void Initialize()
        {
        }
        public void Execute()
        {
        }
    }
}
