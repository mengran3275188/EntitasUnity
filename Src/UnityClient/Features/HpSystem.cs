using System;
using System.Collections.Generic;
using Entitas;
using Util;

namespace UnityClient
{
    public class HpSystem : ReactiveSystem<GameEntity>
    {
        public HpSystem(Contexts contexts) : base(contexts.game)
        {
            m_Context = contexts.game;
        }

        protected override Entitas.ICollector<GameEntity> GetTrigger(Entitas.IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.Hp);
        }

        protected override bool Filter(GameEntity entity)
        {
            return entity.isEnabled;
        }

        protected override void Execute(System.Collections.Generic.List<GameEntity> entities)
        {
            foreach(var entity in entities)
            {
            }
        }
        private readonly GameContext m_Context;
    }
}
