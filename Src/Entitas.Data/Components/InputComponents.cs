using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas;

namespace Entitas.Data
{
    [Input]
    public class KeyDownComponent : IComponent
    {
        public KeyCode Value;
    }
}
