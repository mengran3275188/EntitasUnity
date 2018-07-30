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
        readonly GameContext game;
        public GameStartSystem(Contexts contexts)
        {
            game = contexts.game;
        }

        public void Initialize()
        {

            Services.Instance.SceneService.Init();
            Services.Instance.SceneService.LoadScene("MainCity");

        }
    }
}
