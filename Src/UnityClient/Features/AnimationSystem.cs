using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class AnimationSystem : ReactiveSystem<GameEntity>
    {
        public AnimationSystem(Contexts contexts) : base(contexts.game)
        {
            m_Context = contexts.game;
        }
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(new TriggerOnEvent<GameEntity>(GameMatcher.Movement, GroupEvent.AddedOrRemoved));
        }
        protected override bool Filter(GameEntity entity)
        {
            return entity.movement.State != Entitas.Data.MoveState.SkillMoving;
        }
        protected override void Execute(List<GameEntity> entities)
        {
            foreach(GameEntity entity in entities)
            {
                if(entity.movement.State == Entitas.Data.MoveState.UserMoving)
                {
                    GfxSystem.PlayAnimation(entity.resource.ResourceId, "zhankuang_run_01");
                }
                else
                {
                    GfxSystem.PlayAnimation(entity.resource.ResourceId, "zhankuang_stand_01");
                }
            }
        }
        private readonly GameContext m_Context;
    }
}
