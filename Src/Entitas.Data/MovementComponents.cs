using Entitas.Data;
using Util;

namespace Entitas.Data
{
    public sealed class MovementComponent : IComponent
    {
        public Vector3 Velocity;
    }
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
    public sealed class DisableMoveControlComponent : IComponent
    {
    }
    public sealed class DisableRotationControlComponent : IComponent
    {
    }
}

