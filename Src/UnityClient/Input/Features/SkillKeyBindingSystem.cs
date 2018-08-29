using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

namespace UnityClient
{
    public class SkillKeyBindingSystem : IInitializeSystem
    {
        public SkillKeyBindingSystem(Contexts contexts)
        {
        }
        public void Initialize()
        {
            Dictionary<Keyboard.Code, int> keyBindings = new Dictionary<Keyboard.Code, int>();
            keyBindings.Add(Keyboard.Code.H, 0);
            keyBindings.Add(Keyboard.Code.J, 1);
            keyBindings.Add(Keyboard.Code.K, 2);
            keyBindings.Add(Keyboard.Code.L, 3);
            keyBindings.Add(Keyboard.Code.Space, 4);

            Contexts.sharedInstance.input.ReplaceSkillKeyBinding(keyBindings);
        }
    }
}
