using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class DeadSystem : IExecuteSystem
    {
        public DeadSystem(Contexts contexts)
        {
            m_Contexts = contexts;
            m_EntitiesGroup = m_Contexts.game.GetGroup(GameMatcher.Dead);
        }

        public void Execute()
        {
            var entities = m_EntitiesGroup.GetEntities();
            foreach(var entity in entities)
            {
                if(m_Contexts.input.time.Value > entity.dead.DeadTime + m_DeadRemainTime)
                {
                    entity.isDestory = true;
                }
            }
        }

        private const float m_DeadRemainTime = 3.0f;
        private readonly IGroup<GameEntity> m_EntitiesGroup;
        private readonly Contexts m_Contexts;
    }
}
