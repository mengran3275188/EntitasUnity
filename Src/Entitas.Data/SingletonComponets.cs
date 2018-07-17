using System;
using Entitas.Data;
using Spatial;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Data
{
    [Unique]
    public sealed class MainPlayerComponent : IComponent
    {
    }
    [Unique]
    public sealed class TimeInfoComponent : IComponent
    {
        public float Time;
        public float DeltaTime;
    }
    [Unique]
    public sealed class SceneComponent : IComponent
    {
        public SceneConfig Config;
        public SceneInstanceInfo ScriptInstance;
    }
}
