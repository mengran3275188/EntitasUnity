using System;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Util;

namespace Entitas.Component
{
    public delegate void CollisionDelegate(uint targetEntityId);

    public sealed class ResourceComponent : IComponent
    {
        public uint ResourceId;
    }
    public sealed class IdComponent : IComponent
    {
        [PrimaryEntityIndex]public uint value;
    }
    public sealed class AIComponent : IComponent
    {
        public behaviac.Agent Agent;
    }
    // TODO(camp id)
    public sealed class PhysicsComponent : IComponent
    {
        public Jitter.Dynamics.RigidBody Rigid;
    }
    public sealed class DestoryComponent : IComponent
    {
    }
    public sealed class SpatialComponent : IComponent
    {
    }
}
