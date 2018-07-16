using System;
using System.Collections.Generic;
using Entitas;
using UnityDelegate;

namespace UnityClient
{
    public class PositionSystem : ReactiveSystem<GameEntity>
    {
        public PositionSystem(Contexts contexts) : base(contexts.game)
        {
            m_Context = contexts.game;
        }
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Position, GameMatcher.Resource));
        }
        protected override bool Filter(GameEntity entity)
        {
            return entity.isEnabled;
        }
        protected override void Execute(List<GameEntity> entities)
        {
            foreach(GameEntity entity in entities)
            {
                GfxSystem.UpdatePosition(entity.resource.Value, entity.position.Value.x, entity.position.Value.y, entity.position.Value.z);
            }
        }

        private readonly GameContext m_Context;
    }
}
