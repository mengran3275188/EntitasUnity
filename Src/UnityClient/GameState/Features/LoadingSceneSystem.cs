using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

namespace UnityClient
{
    public class LoadingSceneSystem : IExecuteSystem
    {
        public LoadingSceneSystem(Contexts contexts)
        {
            m_Contexts = contexts;
        }
        public void Execute()
        {
            if(m_Contexts.gameState.hasLoadingProgress)
            {
                var loadingProgress = m_Contexts.gameState.loadingProgress;
                //if(Mathf.Approximately(loadingProgress.Value, 1.0f))
                {
                }
            }
        }

        private readonly Contexts m_Contexts;
    }
}
