using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class ChunkSystem : ReactiveSystem<GameEntity>
    {
        public ChunkSystem(Contexts contexts) : base(contexts.game)
        {
        }
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Chunk);
        }
        protected override bool Filter(GameEntity entity)
        {
            return true;
        }
        protected override void Execute(List<GameEntity> entities)
        {
            foreach(GameEntity entity in entities)
            {
                Services.Instance.ChunkService.LoadChunk(entity.chunk.Value);
            }
        }
    }
}
