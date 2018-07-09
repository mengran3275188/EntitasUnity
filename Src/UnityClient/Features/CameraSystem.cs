using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace UnityClient
{
    public class CameraSystem : ReactiveSystem<GameEntity>
    {
        public CameraSystem(Contexts contexts) : base(contexts.game)
        {
            m_Context = contexts.game;
        }
        protected override ICollector<GameEntity> GetTrigger(IContext<GameEntity> context)
        {
            return context.CreateCollector(GameMatcher.AllOf(GameMatcher.MainPlayer, GameMatcher.Position));
        }
        protected override bool Filter(GameEntity entity)
        {
            return entity.isEnabled;
        }
        protected override void Execute(List<GameEntity> entities)
        {
            foreach(GameEntity e in entities)
            {
                GfxSystem.UpdateCamera(e.position.Value.x, e.position.Value.y, e.position.Value.z);
            }
        }

        private readonly GameContext m_Context;
    }
}
