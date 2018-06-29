using System;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using SpatialSystem;
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
    public sealed class CollisionComponent : IComponent
    {
        public CollisionDelegate OnCollision;
        public BoxCollider Collider;
        public Vector3 Offset;
    }
    public sealed class DestoryComponent : IComponent
    {
    }
}
