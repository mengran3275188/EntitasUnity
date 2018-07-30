using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class LevelSystem : ReactiveSystem<GameStateEntity>
    {
        public LevelSystem(Contexts contexts) : base(contexts.gameState)
        {
        }
        protected override ICollector<GameStateEntity> GetTrigger(IContext<GameStateEntity> context)
        {
            return context.CreateCollector(GameStateMatcher.Level);
        }
        protected override bool Filter(GameStateEntity entity)
        {
            return true;
        }
        protected override void Execute(List<GameStateEntity> entities)
        {
        }
    }
}
