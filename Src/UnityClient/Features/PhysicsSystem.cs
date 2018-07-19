using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using Util;

namespace UnityClient
{
    public class PhysicsSystem : IInitializeSystem, IExecuteSystem
    {
        public PhysicsSystem(Contexts contexts)
        {
            m_Context = contexts.game;
            m_World = new Jitter.World(new Jitter.Collision.CollisionSystemSAP());

            m_World.CollisionSystem.CollisionDetected += CollisionSystem_CollisionDetected;
        }


        public void Initialize()
        {

            m_Context.GetGroup(GameMatcher.Physics).OnEntityAdded += PhysicsSystem_OnEntityAdded;
            m_Context.GetGroup(GameMatcher.Physics).OnEntityRemoved += PhysicsSystem_OnEntityRemoved;

            Jitter.Collision.Shapes.BoxShape shape = new Jitter.Collision.Shapes.BoxShape(new Util.Vector3(300, 1, 300));
            Jitter.Dynamics.RigidBody rigid = new Jitter.Dynamics.RigidBody(shape);
            rigid.Position = new Util.Vector3(0, -0.5f, 0);
            rigid.LinearVelocity = Util.Vector3.zero;
            rigid.IsStatic = true;
            rigid.Tag = false;
            m_World.AddBody(rigid);
        }

        public void Execute()
        {
            m_World.Step(m_Context.timeInfo.DeltaTime, false);

            DebugDrawer drawer = new DebugDrawer();
            foreach(Jitter.Dynamics.RigidBody body in m_World.RigidBodies)
            {
               body.DebugDraw(drawer);
            }
        }

        private void PhysicsSystem_OnEntityAdded(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            PhysicsComponent physics = component as PhysicsComponent;
            if(null != physics)
            {
                physics.Rigid.EnableDebugDraw = true;
                m_World.AddBody(physics.Rigid);
            }
        }
        private void PhysicsSystem_OnEntityRemoved(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
            PhysicsComponent physics = component as PhysicsComponent;
            if(null != physics)
            {
                m_World.RemoveBody(physics.Rigid, true);
            }
        }
        private void CollisionSystem_CollisionDetected(Jitter.Dynamics.RigidBody body1, Jitter.Dynamics.RigidBody body2, Util.Vector3 point1, Util.Vector3 point2, Util.Vector3 normal, float penetration)
        {
            RigidObject rigidObject1 = body1 as RigidObject;
            RigidObject rigidObject2 = body2 as RigidObject;
            if(null != rigidObject1 && null != rigidObject2)
            {
                var entity1 = m_Context.GetEntityWithId(rigidObject1.EntityId);
                var entity2 = m_Context.GetEntityWithId(rigidObject2.EntityId);
                if(null != entity1 && null != entity2 && entity1.camp.Value != entity2.camp.Value)
                {
                    if(null != entity1.physics.OnCollision)
                        entity1.physics.OnCollision(entity2.id.value);
                    if(null != entity2.physics.OnCollision)
                        entity2.physics.OnCollision(entity1.id.value);
                }
            }
        }

        private class DebugDrawer : Jitter.IDebugDrawer
        {
            public void DrawLine(Vector3 start, Vector3 end)
            {
                GfxSystem.DrawLine(start, end);
            }
            public void DrawPoint(Vector3 pos)
            {
                GfxSystem.DrawPoint(pos, 0.1f);
            }
            public void DrawTriangle(Vector3 pos1, Vector3 pos2, Vector3 pos3)
            {
                DrawLine(pos1, pos2);
                DrawLine(pos2, pos3);
                DrawLine(pos3, pos1);
            }
        }

        private readonly GameContext m_Context;

        private readonly Jitter.World m_World;
    }

}
