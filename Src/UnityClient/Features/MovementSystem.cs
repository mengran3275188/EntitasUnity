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
                if (entity.movement.State != Entitas.Data.MoveState.UserMoving)
                    continue;

                float moveDir = entity.movement.MovingDir;
                float speed = 10.0f;
                int lastAdjust = entity.movement.LastAdjust;
                float distance = speed * m_GameContext.timeInfo.DeltaTime;
                Vector3 oldPos = entity.position.Value;
                Vector3 newPos = new Vector3();
                Vector3 testPos = new Vector3();
                for (int ct = 0; ct < 8; ++ct)
                {
                    float cosV = Mathf.Cos(moveDir);
                    float sinV = Mathf.Sin(moveDir);

                    float z = oldPos.z + cosV * distance;
                    float x = oldPos.x + sinV * distance;

                    float testDistance = distance + 0.75f;

                    float testZ = oldPos.z + cosV * testDistance;
                    float testX = oldPos.x + sinV * testDistance;

                    newPos = new Vector3(x, 0, z);
                    testPos = new Vector3(testX, 0, testZ);

                    if (SpatialSystem.Instance.CanPass(oldPos, testPos))
                    {
                        entity.ReplacePosition(newPos);
                        entity.ReplaceMovement(Entitas.Data.MoveState.UserMoving, moveDir, lastAdjust);
                        break;
                    }
                    else
                    {
                        float newDir = (moveDir + Mathf.PI / 4) % (Mathf.PI * 2);
                        if (lastAdjust >= 0 && CanGo(oldPos.x, oldPos.z, newDir, testDistance))
                        {
                            LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", moveDir, newDir, lastAdjust);
                            moveDir = newDir;
                            lastAdjust = 1;
                        }
                        else
                        {
                            newDir = (moveDir + Mathf.PI * 2 - Mathf.PI / 4) % (Mathf.PI * 2);
                            if (lastAdjust <= 0 && CanGo(oldPos.x, oldPos.z, newDir, testDistance))
                            {
                                LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", moveDir, newDir, lastAdjust);
                                moveDir = newDir;
                                lastAdjust = -1;
                            }
                            else
                            {
                                newDir = (moveDir + Mathf.PI / 2) % (Mathf.PI * 2);
                                if (lastAdjust >= 0 && CanGo(oldPos.x, oldPos.z, newDir, testDistance))
                                {
                                    LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", moveDir, newDir, lastAdjust);
                                    moveDir = newDir;
                                    lastAdjust = 1;
                                }
                                else
                                {
                                    newDir = (moveDir + Mathf.PI * 2 - Mathf.PI / 2) % (Mathf.PI * 2);
                                    if (lastAdjust <= 0 && CanGo(oldPos.x, oldPos.z, newDir, testDistance))
                                    {
                                        LogUtil.Info("adjust dir:{0}->{1} success, last adjust:{2}", moveDir, newDir, lastAdjust);
                                        moveDir = newDir;
                                        lastAdjust = -1;
                                    }
                                    else
                                    {
                                        LogUtil.Info("adjust dir:{0}->{1} failed, last adjust:{2}", moveDir, newDir, lastAdjust);
                                        break;
                                    }
                                }
                            }
                        }
                    }
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
