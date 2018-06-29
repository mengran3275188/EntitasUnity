using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

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
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.Animation, GameMatcher.Movement));
        }
        protected override bool Filter(GameEntity entity)
        {
            return entity.isEnabled && entity.movement.State != Entitas.Data.MoveState.SkillMoving && entity.movement.State != MoveState.ImpactMoving;
        }
        protected override void Execute(List<GameEntity> entities)
        {
            foreach(GameEntity entity in entities)
            {
                var animation = entity.animation;

                if(entity.movement.State == Entitas.Data.MoveState.UserMoving)
                {
                    GfxSystem.PlayAnimation(entity.resource.ResourceId, GetAnimationName(animation.ActionId, animation.Prefix, AnimationType.Run));
                }
                else
                {
                    GfxSystem.PlayAnimation(entity.resource.ResourceId, GetAnimationName(animation.ActionId, animation.Prefix, AnimationType.Idle));
                }
            }
        }
        private string GetAnimationName(int actionId, string prefix, AnimationType animationType)
        {
            string animationName = string.Empty;

            ActionConfig config = ActionConfigProvider.Instance.GetActionConfig(actionId);
            if(null != config)
            {
                switch(animationType)
                {
                    case AnimationType.Idle:
                        animationName = string.Format("{0}{1}", prefix, config.Stand);
                        break;
                    case AnimationType.Run:
                        animationName = string.Format("{0}{1}", prefix, config.Run);
                        break;
                }
            }
            return animationName;
        }
        private readonly GameContext m_Context;
    }
}
