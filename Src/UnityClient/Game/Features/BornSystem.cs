using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class BornSystem : IExecuteSystem
    {
        public BornSystem(Contexts contexts)
        {
            m_Contexts = contexts;
            m_EntitiesGroup = m_Contexts.game.GetGroup(GameMatcher.Born);
        }
        public void Execute()
        {
            var entities = m_EntitiesGroup.GetEntities();
            foreach(var entity in entities)
            {
                if(m_Contexts.input.time.Value > entity.born.BornTime + m_BornRemainTime)
                {
                    entity.RemoveBorn();
                }
            }
        }

        private const float m_BornRemainTime = 1.3f;
        private readonly IGroup<GameEntity> m_EntitiesGroup;
        private readonly Contexts m_Contexts;
    }
}
