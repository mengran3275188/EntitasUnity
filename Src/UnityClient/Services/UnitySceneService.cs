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
        public void Tick()
        {
            float loadProgress = -1f;
            if(null != m_LoadOperation)
            {
                loadProgress = m_LoadOperation.progress;
                _contexts.gameState.ReplaceLoadingProgress(loadProgress);
            }

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
        public void ChangeToLoadingScene()
        {
            SceneManager.LoadScene("Loading");
        }
        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }
        public void LoadSceneAsync(string name)
        {
            m_LoadOperation = SceneManager.LoadSceneAsync(name);
            _contexts.gameState.ReplaceLoadingProgress(0.0f);
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Util.LogUtil.Error("OnSceneLoaded~");
            SceneSystem.Instance.Load(1);
        }
        private AsyncOperation m_LoadOperation;
    }
}
