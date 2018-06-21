using System;
using System.Collections.Generic;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.Component
{
    public sealed class ResourceComponent : IComponent
    {
        public uint ResourceId;
    }
}
