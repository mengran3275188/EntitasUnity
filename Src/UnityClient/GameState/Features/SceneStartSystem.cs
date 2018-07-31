using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

namespace UnityClient
{
    public class SceneStartSystem : ReactiveSystem<GameStateEntity>
    {
        public SceneStartSystem(Contexts contexts) : base(contexts.gameState)
        {
        }
        protected override ICollector<GameStateEntity> GetTrigger(IContext<GameStateEntity> context)
        {
            return context.CreateCollector(GameStateMatcher.SceneLoadFinished);
        }
        protected override bool Filter(GameStateEntity entity)
        {
            return Contexts.sharedInstance.gameState.isSceneLoadFinished && Contexts.sharedInstance.gameState.hasCurSceneId;
        }
        protected override void Execute(List<GameStateEntity> entities)
        {
            Services.Instance.UIService.Cleanup();

            int sceneId = Contexts.sharedInstance.gameState.curSceneId.Value;

            SceneSystem.Instance.Load(sceneId);

            Contexts.sharedInstance.gameState.isSceneLoadFinished = false;
        }
    }
}
