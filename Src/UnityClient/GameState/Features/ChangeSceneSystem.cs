using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

namespace UnityClient
{
    public class ChangeSceneSystem : ReactiveSystem<GameStateEntity>
    {
        public ChangeSceneSystem(Contexts contexts) : base(contexts.gameState)
        {
        }
        protected override ICollector<GameStateEntity> GetTrigger(IContext<GameStateEntity> context)
        {
            return context.CreateCollector(GameStateMatcher.NextSceneId);
        }
        protected override bool Filter(GameStateEntity entity)
        {
            return true;
        }
        protected override void Execute(List<GameStateEntity> entities)
        {
            int sceneId = Contexts.sharedInstance.gameState.nextSceneId.Value;

            SceneConfig config = SceneConfigProvider.Instance.GetSceneConfig(sceneId);
            if(null != config)
                Services.Instance.SceneService.LoadSceneAsync(config.Id, config.Name);

        }
    }
}
