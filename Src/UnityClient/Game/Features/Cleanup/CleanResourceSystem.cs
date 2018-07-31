using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class CleanResourceSystem : ReactiveSystem<GameEntity>
    {
        public CleanResourceSystem(Contexts contexts) : base(contexts.game)
        {
        }
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Cleanup);
        }
        protected override bool Filter(GameEntity entity)
        {
            return true;
        }
        protected override void Execute(List<GameEntity> entities)
        {
            UnityDelegate.ResourceSystem.Cleanup();

            Contexts.sharedInstance.game.isCleanup = false;
        }
    }
}
