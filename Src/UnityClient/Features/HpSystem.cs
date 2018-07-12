using System;
using System.Collections.Generic;
using Entitas;
using Util;

namespace UnityClient
{
    public class HpSystem : ReactiveSystem<GameEntity>, IInitializeSystem
    {
        public HpSystem(Contexts contexts) : base(contexts.game)
        {
            m_Context = contexts.game;
        }
        public void Initialize()
        {
            m_Context.GetGroup(GameMatcher.Hp).OnEntityAdded += HpSystem_OnEntityAdded;
            m_Context.GetGroup(GameMatcher.Hp).OnEntityRemoved += HpSystem_OnEntityRemoved;
        }


        protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Hp);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isEnabled;
        }

        protected override void Execute(List<GameEntity> entities)
        {
            foreach(var entity in entities)
            {
                GfxSystem.UpdateHudHead(entity.resource.ResourceId, entity.hp.Value, entity.attr.Value.HpMax);
            }
        }
        private void HpSystem_OnEntityAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            // add hp hud;
            GfxSystem.CreateHudHead(entity.resource.ResourceId);
        }

        private void HpSystem_OnEntityRemoved(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            // remove hp hud;
        }

        private readonly GameContext m_Context;
    }
}
