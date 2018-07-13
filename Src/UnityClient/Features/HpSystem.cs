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
                if (entity.hp.Value <= 0)
                {
                    if (entity.isMainPlayer)
                        entity.ReplaceHp(entity.attr.Value.HpMax);
                    else
                        entity.ReplaceDead(m_Context.timeInfo.Time);
                }
                GfxSystem.UpdateHudHead(entity.resource.ResourceId, entity.hp.Value, entity.attr.Value.HpMax);
            }
        }
        private readonly GameContext m_Context;
    }
}
