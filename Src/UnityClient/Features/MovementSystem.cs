using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using Util;

namespace UnityClient
{
    public class MovementSystem : IExecuteSystem
    {
        public MovementSystem(Contexts contexts)
        {
            m_GameContext = contexts.game;
            m_MovingEntities = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.Movement, GameMatcher.Physics));
        }
        public void Execute()
        {
            var entities = m_MovingEntities.GetEntities();
            foreach (GameEntity entity in entities)
            {
                if(entity.hasPhysics)
                {
                    Vector3 position = entity.physics.Rigid.Position;
                    Vector3 velocity = entity.movement.Velocity;
                    Vector3 offset = entity.physics.Offset;
                    RigidObject rigid = entity.physics.Rigid;

                    Matrix3x3 orientation = Matrix3x3.CreateRotationY(entity.rotation.Value);


                    Vector3 skillVelocity = SkillSystem.Instance.GetSkillVelocity(entity);
                    Vector3 buffVelocity = BuffSystem.Instance.GetBuffVelocity(entity);

                    rigid.LinearVelocity = velocity + skillVelocity + buffVelocity;
                    rigid.AngularVelocity = Vector3.zero;
                    rigid.Orientation = orientation;

                    entity.ReplacePosition(position - offset);

                    if(entity.hasCollision)
                    {
                        Vector3 collisionOffset = entity.collision.Offset;
                        entity.collision.Rigid.Position = position - offset + collisionOffset;
                        entity.collision.Rigid.Orientation = orientation;
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
