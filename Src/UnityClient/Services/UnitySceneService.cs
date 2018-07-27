using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClient
{
    public class SceneService : Service
    {
        public SceneService(Contexts contexts) : base(contexts)
        {
        }

        public void InitChunks()
        {
            GameObject sceneRoot = GameObject.Find("Scene");
            if(null != sceneRoot)
            {
                UnityChunk[] chunks = sceneRoot.GetComponentsInChildren<UnityChunk>();

                foreach(var chunk in chunks)
                {
                    var e = _contexts.game.CreateEntity();
                    e.AddChunk(chunk);
                }
            }
        }
    }
}
