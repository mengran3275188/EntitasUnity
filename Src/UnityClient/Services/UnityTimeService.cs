using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClient
{
    public class UnityTimeService : Service
    {
        public UnityTimeService(Contexts contexts) : base(contexts)
        {
        }
        public void Initialize()
        {
            // Make it bulletproof
            Execute();
        }
        public void Execute()
        {
            float time = Time.time;
            float realTime = Time.realtimeSinceStartup;

            _contexts.input.ReplaceTime(time);
            _contexts.input.ReplaceRealTimeSinceStartup(realTime);
        }
    }
}
