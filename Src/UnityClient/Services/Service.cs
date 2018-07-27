using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public abstract class Service
    {
        protected readonly Contexts _contexts;
        public Service(Contexts contexts)
        {
            _contexts = contexts;
        }
    }
}
