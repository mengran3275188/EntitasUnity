using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

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
            foreach(GameEntity entity in entities)
            {
                if (entity.movement.State != Entitas.Data.MoveState.UserMoving)
                    continue;

                float moveDir = entity.movement.MovingDir;
                float speed = 10.0f;
                int lastAdjust = entity.movement.LastAdjust;
                float distance = speed * m_GameContext.timeInfo.DeltaTime;
                bool canMove = true;
                do
                {
                    if (CanGo(entity.position.x, entity.position.z, moveDir, distance))
                        break;
                    float newDir = (float)((moveDir + Math.PI / 4) % (Math.PI * 2));
                    if(lastAdjust >= 0 && CanGo(entity.position.x, entity.position.z, newDir, distance))
                    {
                        moveDir = newDir;
                        lastAdjust = 1;
                        break;
                    }
                    newDir = (float)((moveDir + Math.PI * 2 - Math.PI / 4) % (Math.PI * 2));
                    if(lastAdjust <= 0 && CanGo(entity.position.x, entity.position.z, newDir, distance))
                    {
                        moveDir = newDir;
                        lastAdjust = -1;
                        break;
                    }
                    newDir = (float)((moveDir + Math.PI / 2) % (Math.PI * 2));
                    if(lastAdjust >= 0 && CanGo(entity.position.x, entity.position.z, newDir, distance))
                    {
                        moveDir = newDir;
                        lastAdjust = 1;
                        break;
                    }
                    newDir = (float)((moveDir + Math.PI * 2 - Math.PI / 4) % (Math.PI * 2));
                    if(lastAdjust >= 0 && CanGo(entity.position.x, entity.position.z, newDir, distance))
                    {
                        moveDir = newDir;
                        lastAdjust = -1;
                        break;
                    }
                    canMove = false;
                } while (false);

                if(canMove)
                {
                    float newX = entity.position.x + (float)Math.Sin(moveDir) * speed * m_GameContext.timeInfo.DeltaTime;
                    float newY = entity.position.y;
                    float newZ = entity.position.z + (float)Math.Cos(moveDir) * speed * m_GameContext.timeInfo.DeltaTime;

                    entity.ReplacePosition(newX, newY, newZ);
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
