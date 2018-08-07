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
            keyBindings.Add(Keyboard.Code.H, 1);
            keyBindings.Add(Keyboard.Code.J, 2);
            keyBindings.Add(Keyboard.Code.K, 3);
            keyBindings.Add(Keyboard.Code.L, 4);
            keyBindings.Add(Keyboard.Code.Space, 5);

            Contexts.sharedInstance.input.ReplaceSkillKeyBinding(keyBindings);
        }
    }
}
