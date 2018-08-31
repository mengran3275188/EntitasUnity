using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

namespace UnityClient
{
    public class ChunkSystem : ReactiveSystem<ChunkEntity>, IInitializeSystem, ITearDownSystem
    {
        public ChunkSystem(Contexts contexts) : base(contexts.chunk)
        {
            m_ChunkGroup = contexts.chunk.GetGroup(ChunkMatcher.Chunk);
            m_NpcGroup = contexts.game.GetGroup(GameMatcher.Npc);
        }
        public void Initialize()
        {
            m_ChunkGroup.OnEntityAdded += ChunkSystem_OnEntityAdded;
            m_ChunkGroup.OnEntityRemoved += ChunkGroup_OnEntityRemoved;

            m_NpcGroup.OnEntityRemoved += NpcGroup_OnEntityRemoved;

        }


        public void TearDown()
        {
            m_ChunkGroup.OnEntityAdded -= ChunkSystem_OnEntityAdded;
            m_ChunkGroup.OnEntityRemoved -= ChunkGroup_OnEntityRemoved;

            m_NpcGroup.OnEntityRemoved -= NpcGroup_OnEntityRemoved;
        }
        protected override ICollector<ChunkEntity> GetTrigger(IContext<ChunkEntity> context)
        {
            return context.CreateCollector(ChunkMatcher.Chunk);
        }
        protected override bool Filter(ChunkEntity entity)
        {
            return true;
        }
        protected override void Execute(List<ChunkEntity> entities)
        {
        }
        private void ChunkSystem_OnEntityAdded(IGroup<ChunkEntity> group, ChunkEntity entity, int index, IComponent component)
        {
            if(component is ChunkComponent chunkComponent)
            {
                Services.Instance.ChunkService.LoadChunk(entity, chunkComponent.Value);
            }
        }
        private void ChunkGroup_OnEntityRemoved(IGroup<ChunkEntity> group, ChunkEntity entity, int index, IComponent component)
        {
        }
        private void NpcGroup_OnEntityRemoved(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            if(component is NpcComponent npcComponent)
            {
                var chunkEntity = Contexts.sharedInstance.chunk.activeChunkEntity;
                var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                if(null != chunkEntity && null != mainPlayer)
                {
                    bool hasEnemy = false;

                    var npcs = group.GetEntities();
                    foreach(var npc in npcs)
                    {
                        if(npc.hasCamp && npc.camp.Value != mainPlayer.camp.Value)
                        {
                            hasEnemy = true;
                            break;
                        }
                    }
                    if (!hasEnemy)
                    {
                        Services.Instance.ChunkService.OpenDoor(chunkEntity.chunk.Value);
                        chunkEntity.isActiveChunk = false;
                    }
                }
            }
        }

        private readonly IGroup<ChunkEntity> m_ChunkGroup;
        private readonly IGroup<GameEntity> m_NpcGroup;
    }
}
