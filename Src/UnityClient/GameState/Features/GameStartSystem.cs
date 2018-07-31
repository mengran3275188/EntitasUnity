using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using Util;

namespace UnityClient
{
    public class GameStartSystem : IInitializeSystem
    {
        readonly GameStateContext gameState;
        public GameStartSystem(Contexts contexts)
        {
            gameState = contexts.gameState;
        }

        public void Initialize()
        {
            Services.Instance.SceneService.Init();
            Services.Instance.SceneService.LoadSceneAsync(3, "MainCity");
        }
    }
}
