using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace UnityClient
{
    public class CameraSystem : IExecuteSystem
    {
        public CameraSystem(Contexts contexts)
        {
            m_Context = contexts.game;
        }
        public void Execute()
        {
            var e = m_Context.GetGroup(GameMatcher.MainPlayer).GetSingleEntity();
            if(null != e)
                GfxSystem.UpdateCamera(e.position.Value.x, e.position.Value.y, e.position.Value.z);
        }

        private readonly GameContext m_Context;
    }
}
