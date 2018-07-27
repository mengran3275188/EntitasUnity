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
                GfxSystem.UpdateCamera(e.physics.Rigid.Position.x, e.physics.Rigid.Position.y, e.physics.Rigid.Position.z);
        }

        private readonly GameContext m_Context;
    }
}
