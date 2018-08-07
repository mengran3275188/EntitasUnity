using System;
using System.Collections.Generic;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Data
{
    [Input, Unique]
    public class TimeComponent : IComponent
    {
        public float Value;
    }
    [Input, Unique]
    public class RealTimeSinceStartupComponent : IComponent
    {
        public float Value;
    }
    [Input, Unique]
    public class SkillKeyBindingComponent : IComponent
    {
        public Dictionary<Keyboard.Code, int> Value;
    }
}
