using Entitas.Data;
using UnityEngine;

namespace Entitas.Data
{
    [Game]
    public sealed class MovementComponent : IComponent
    {
        public Vector3 Velocity;
    }
    [Game]
    public sealed class PositionComponent : IComponent
    {
        public Vector3 Value;
    }
    public sealed class RotationComponent : IComponent
    {
        public float Value;
    }

    public sealed class AnimationComponent : IComponent
    {
        public int ActionId;
        public string Prefix;
    }
}

