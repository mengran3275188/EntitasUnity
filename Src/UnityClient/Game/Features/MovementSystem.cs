using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using UnityEngine;

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
                if (entity.hasPhysics)
                {
                    if (entity.physics.Rigid.IsKinematic)
                    {
                        float physicalYSpeed = entity.physics.Rigid.Velocity.y;

                        Vector3 velocity = entity.movement.Velocity;
                        Vector3 offset = entity.physics.Offset;
                        IRigidbody rigid = entity.physics.Rigid;

                        Vector3 skillVelocity = SkillSystem.Instance.GetSkillVelocity(entity);
                        Vector3 buffVelocity = BuffSystem.Instance.GetBuffVelocity(entity);

                        if (skillVelocity.IsNearlyZero() && buffVelocity.IsNearlyZero())
                        {
                            rigid.Velocity = velocity + Vector3.up * physicalYSpeed;
                        }
                        else
                        {
                            rigid.Velocity = velocity + skillVelocity + buffVelocity;
                        }

                        rigid.Rotation = Quaternion.Euler(0, Mathf.Rad2Deg * entity.rotation.Value, 0);
                    }

                    Vector3 position = entity.physics.Rigid.Position;
                    entity.ReplacePosition(position);
                }
            }

        }

        private readonly IGroup<GameEntity> m_MovingEntities;
        private readonly GameContext m_GameContext;
    }
}
