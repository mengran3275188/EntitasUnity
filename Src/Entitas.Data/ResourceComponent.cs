using System;
using System.Collections.Generic;
using Entitas.CodeGeneration.Attributes;
using Util;

namespace Entitas.Data
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
        public RigidObject Rigid;
        public Vector3 Offset;
        public CollisionDelegate OnCollision;
    }
    public sealed class DestoryComponent : IComponent
    {
    }
    public sealed class SpatialComponent : IComponent
    {
    }
    public sealed class CampComponent : IComponent
    {
        public int Value;
    }
    public sealed class AttrComponent : IComponent
    {
        public AttributeData Value;
    }
    public sealed class HpComponent : IComponent
    {
        public float Value;
    }
    public sealed class DeadComponent : IComponent
    {
        public float DeadTime;
    }
}
