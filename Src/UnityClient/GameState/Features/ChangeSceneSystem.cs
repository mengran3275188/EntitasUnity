using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

namespace UnityClient
{
    public class ChangeSceneSystem : ReactiveSystem<GameStateEntity>
    {
        public ChangeSceneSystem(Contexts contexts) : base(contexts.gameState)
        {
        }
        protected override ICollector<GameStateEntity> GetTrigger(IContext<GameStateEntity> context)
        {
            return context.CreateCollector(GameStateMatcher.NextSceneName);
        }
        protected override bool Filter(GameStateEntity entity)
        {
            return true;
        }
        protected override void Execute(List<GameStateEntity> entities)
        {
            Services.Instance.SceneService.ChangeToLoadingScene();
        }
    }
}
