﻿using System;
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
    public class LoadingProgressComponent : IComponent
    {
        public float Value;
    }
    [GameState, Unique]
    public class NextSceneNameComponent : IComponent
    {
        public string Value;
    }
}
