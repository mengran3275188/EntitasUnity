using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

namespace UnityClient
{
    public class ChunkSystem : ReactiveSystem<GameEntity>, IInitializeSystem, ITearDownSystem
    {
        public ChunkSystem(Contexts contexts) : base(contexts.game)
        {
            m_ChunkGroup = contexts.game.GetGroup(GameMatcher.Chunk);
        }
        public void Initialize()
        {
            m_ChunkGroup.OnEntityAdded += ChunkSystem_OnEntityAdded;
            m_ChunkGroup.OnEntityRemoved += ChunkGroup_OnEntityRemoved;
        }
        public void TearDown()
        {
            m_ChunkGroup.OnEntityAdded -= ChunkSystem_OnEntityAdded;
            m_ChunkGroup.OnEntityRemoved -= ChunkGroup_OnEntityRemoved;
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
        }
        private void ChunkSystem_OnEntityAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            if(component is ChunkComponent chunkComponent)
            {
                Services.Instance.ChunkService.LoadChunk(chunkComponent.Value);
            }
        }
        private void ChunkGroup_OnEntityRemoved(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            throw new NotImplementedException();
        }
        private void OnChunkTriggerEnter()
        {
        }

        private readonly IGroup<GameEntity> m_ChunkGroup;
    }
}
