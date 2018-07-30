using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityClient
{
    public class UnitySceneService : Service
    {
        public UnitySceneService(Contexts contexts) : base(contexts)
        {
        }

        public void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
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
        public void LoadScene(string name)
        {
            SceneManager.LoadScene("MainCity");
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Util.LogUtil.Error("OnSceneLoaded~");
            SceneSystem.Instance.Load(1);
        }
    }
}
