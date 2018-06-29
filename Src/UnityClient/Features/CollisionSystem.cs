using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class CollisionSystem : IInitializeSystem, IExecuteSystem
    {
        public CollisionSystem(Contexts contexts)
        {
            m_Context = contexts.game;
        }
        public void Initialize()
        {
            m_CollisionGroup = m_Context.GetGroup(GameMatcher.AllOf(GameMatcher.Collision, GameMatcher.Position));
        }
        public void Execute()
        {
            var entities = m_CollisionGroup.GetEntities();
            foreach(var entity in entities)
            {
                entity.collision.Collider.UpdatePosition(new Util.Vector3(entity.collision.Offset.x + entity.position.x, entity.collision.Offset.y + entity.position.y, entity.collision.Offset.z + entity.position.z));
            }
            for(int i = 0; i < entities.Length - 1; ++i)
            {
                for(int j = i + 1; j < entities.Length; ++j)
                {
                    if(entities[i].collision.Collider.Intersects(entities[j].collision.Collider))
                    {
                        if(null != entities[i].collision.OnCollision)
                        {
                            entities[i].collision.OnCollision(entities[j].id.value);
                        }
                        if(null != entities[j].collision.OnCollision)
                        {
                            entities[j].collision.OnCollision(entities[i].id.value);
                        }
                    }
                }
            }
        }


        private readonly GameContext m_Context;
        private IGroup<GameEntity> m_CollisionGroup;
    }
}
