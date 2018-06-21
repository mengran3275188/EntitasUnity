using Entitas.Data;

namespace Entitas.Component
{
    public sealed class MovementComponent : IComponent
    {
        public MoveState State;
        public float MovingDir;
        public int LastAdjust;
    }
    public sealed class PositionComponent : IComponent
    {
        public float x;
        public float y;
        public float z;
    }
    public sealed class RotationComponent : IComponent
    {
        public RotateState State;
        public float RotateDir;
    }

    public sealed class AnimationComponent : IComponent
    {
    }
}

