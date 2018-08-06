using Entitas.Data;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Entitas.Data
{
    public sealed class MovementComponent : IComponent
    {
        public Vector3 Velocity;
    }
    [Game, Event(EventTarget.Self)]
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

