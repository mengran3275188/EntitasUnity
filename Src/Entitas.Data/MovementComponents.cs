using Entitas.Data;
using Util;

namespace Entitas.Data
{
    public sealed class MovementComponent : IComponent
    {
        public MoveState State;
        public Vector3 Velocity;
    }
    public sealed class PositionComponent : IComponent
    {
        public Vector3 Value;
    }
    public sealed class RotationComponent : IComponent
    {
        public RotateState State;
        public float RotateDir;
    }

    public sealed class AnimationComponent : IComponent
    {
        public int ActionId;
        public string Prefix;
    }
}

