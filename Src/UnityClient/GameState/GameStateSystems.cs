using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityClient
{
    public class GameStateSystems : Feature
    {
        public GameStateSystems(Contexts contexts, Services services)
        {
            Add(new GameStartSystem(contexts));
            Add(new ChangeSceneSystem(contexts));
            Add(new SceneStartSystem(contexts));
            Add(new SceneServiceSystem(contexts));
        }
    }
}
