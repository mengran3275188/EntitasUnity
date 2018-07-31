using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas.Data;
using UnityEngine;
using Entitas;

namespace UnityClient
{
    public class CameraSystem : IInitializeSystem, IExecuteSystem
    {
        public CameraSystem(Contexts contexts)
        {
            m_Context = contexts.game;
        }
        public void Initialize()
        {
            CameraConfig config = CameraConfigProvider.Instance.GetCameraConfig(1);
            if(null != config)
            {
                m_Rotation = Quaternion.Euler(config.Pitch, config.Yaw, 0);
                m_Distance = config.Distance;
            }
        }
        public void Execute()
        {
            var e = m_Context.GetGroup(GameMatcher.MainPlayer).GetSingleEntity();
            if (null != e)
            {
                Vector3 cameraTargetPosition = e.physics.Rigid.Position + m_Rotation * (Vector3.back * m_Distance);
                Services.Instance.CameraService.SetPosition(cameraTargetPosition);
            }
        }


        private float m_Distance;
        private Quaternion m_Rotation = Quaternion.identity;
        private readonly GameContext m_Context;
    }
}
