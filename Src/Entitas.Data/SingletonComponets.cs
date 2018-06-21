using System;
using Entitas.Data;
using Spatial;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Component
{
    [Unique]
    public sealed class MainPlayerComponent : IComponent
    {
    }
    [Unique]
    public sealed class InputComponent : IComponent
    {
        public bool IsMoving;
        public float MovingDir;
    }
    [Unique]
    public sealed class TimeInfoComponent : IComponent
    {
        public float Time;
        public float DeltaTime;
    }
    [Unique]
    public sealed class SpatialComponent : IComponent
    {
        public CellManager CellMgr;
        public JumpPointFinder Finder;
    }
    [Unique]
    public sealed class SceneComponent : IComponent
    {
        public SceneInstanceInfo Instance;
    }
}
