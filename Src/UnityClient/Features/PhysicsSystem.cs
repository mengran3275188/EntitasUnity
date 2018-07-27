using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public class PhysicsSystem : IInitializeSystem, IExecuteSystem
    {
        public PhysicsSystem(Contexts contexts)
        {
            m_Context = contexts.game;
        }


        public void Initialize()
        {

            m_Context.GetGroup(GameMatcher.Physics).OnEntityAdded += PhysicsSystem_OnEntityAdded;
            m_Context.GetGroup(GameMatcher.Physics).OnEntityRemoved += PhysicsSystem_OnEntityRemoved;

            m_Context.GetGroup(GameMatcher.Collision).OnEntityAdded += PhysicsSystem_OnEntityAdded;
            m_Context.GetGroup(GameMatcher.Collision).OnEntityRemoved += PhysicsSystem_OnEntityRemoved;
        }

        public void Execute()
        {
        }

        private void PhysicsSystem_OnEntityAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
        }
        private void PhysicsSystem_OnEntityRemoved(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
        }

        private readonly GameContext m_Context;
    }

}
