using System;
using Entitas.Data;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Data
{
    [Unique]
    public sealed class MainPlayerComponent : IComponent
    {
    }
    [Unique]
    public sealed class SceneComponent : IComponent
    {
        public SceneConfig Config;
        public SceneInstanceInfo ScriptInstance;
    }
    [Unique]
    public sealed class Cleanup : IComponent
    {
    }
}
