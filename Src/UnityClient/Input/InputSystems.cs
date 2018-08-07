using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityClient
{
    public class InputSystems : Feature
    {
        public InputSystems(Contexts contexts, Services services)
        {
            Add(new TimeSystem(contexts));
            Add(new SkillKeyBindingSystem(contexts));
            Add(new InputSystem(contexts));
            Add(new SkillInputSystem(contexts));
        }
    }
}
