using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Util;

namespace UnityClient
{
    public class MovementSystem : IExecuteSystem
    {
        public MovementSystem(Contexts contexts)
        {
            m_GameContext = contexts.game;
            m_MovingEntities = contexts.game.GetGroup(GameMatcher.Movement);
        }
        public void Execute()
        {
            var entities = m_MovingEntities.GetEntities();
            foreach (GameEntity entity in entities)
            {

                if(entity.hasPhysics)
                {
                    entity.ReplacePosition(entity.physics.Rigid.Position);
                }
            }
        }

        private bool CanGo(float x, float y, float dir, float distance)
        {
            float tryX = (float)(x + distance * Math.Sin(dir));
            float tryY = (float)(y + distance * Math.Cos(dir));

            return SpatialSystem.Instance.CanPass(x, y, tryX, tryY);
        }

        private readonly IGroup<GameEntity> m_MovingEntities;
        private readonly GameContext m_GameContext;
    }
}
