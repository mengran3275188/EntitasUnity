using System;
using System.Collections.Generic;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Data
{
    [GameState, Unique]
    public class CurSceneIdComponent : IComponent
    {
        public int Value;
    }
    [GameState, Unique]
    public class NextSceneIdComponent : IComponent
    {
        public int Value;
    }
    [GameState, Unique]
    public class LoadingProgressComponent : IComponent
    {
        public float Value;
    }
    [GameState, Unique]
    public sealed class TimeInfoComponent : IComponent
    {
        public float Time;
        public float DeltaTime;
    }
    [GameState, Unique]
    public sealed class SceneLoadFinishedComponent : IComponent
    {
    }
}
