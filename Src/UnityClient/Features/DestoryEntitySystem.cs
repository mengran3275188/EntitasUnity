using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class DestoryEntitySystem : IExecuteSystem
    {
        public DestoryEntitySystem(Contexts contexts)
        {
            m_NeedDestroyEntities = contexts.game.GetGroup(GameMatcher.Destory);
        }
        public void Execute()
        {
            var entities = m_NeedDestroyEntities.GetEntities();
            foreach(var entity in entities)
            {
                entity.Destroy();
            }
        }

        private readonly IGroup<GameEntity> m_NeedDestroyEntities;
    }
}
