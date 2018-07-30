using System;
using System.Collections.Generic;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Data
{
    [GameState, Unique]
    public class LevelComponent : IComponent
    {
        public int Value;
    }
    [GameState, Unique]
    public class NextLevelComponent : IComponent
    {
        public int Value;
    }
    [GameState, Unique]
    public class LevelCleanUpComponent : IComponent
    {
    }
    [GameState, Unique]
    public class LevelLoadedComponent : IComponent
    {
    }
}
