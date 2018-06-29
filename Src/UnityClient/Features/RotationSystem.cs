using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityDelegate;

namespace UnityClient
{
    public class RotationSystem : ReactiveSystem<GameEntity>
    {
        public RotationSystem(Contexts contexts) : base(contexts.game)
        {
            m_Context = contexts.game;
        }

        protected override bool Filter(GameEntity entity)
        {
            return true;
        }
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Rotation, GameMatcher.Resource));
        }
        protected override void Execute(List<GameEntity> entities)
        {
            foreach(GameEntity entity in entities)
            {
                if (entity.rotation.State == Entitas.Data.RotateState.SkillRotate)
                    continue;
                GfxSystem.UpdateRotation(entity.resource.ResourceId, entity.rotation.RotateDir);
            }
        }

        private readonly GameContext m_Context;
    }
}
