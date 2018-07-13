using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Entitas.Data;

namespace UnityDelegate
{
    internal class CameraManager : Util.Singleton<CameraManager>
    {
        public void OnStart()
        {
            CameraConfig config = CameraConfigProvider.Instance.GetCameraConfig(1);
            if(null != config)
            {
                m_FixPitch = config.Pitch;
                m_FixYaw = config.Yaw;
                m_Distance = config.Distance;
            }
        }
        public Camera GetMainCamera()
        {
            if (m_MainCamera == null)
                m_MainCamera = Camera.main;
            return m_MainCamera;
        }

        public void Tick()
        {
            Transform t = GetMainCamera().transform;

            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(m_FixPitch, m_FixYaw, 0);

            UnityEngine.Vector3 pos = m_TargetPos + rotation * (Vector3.back * m_Distance);

            //t.transform.position = Vector3.Lerp(t.transform.position, pos, 0.1f);
            t.transform.position = Vector3.SmoothDamp(t.transform.position, pos, ref m_CameraSpeed, 0.1f);

            Quaternion quaternion = Quaternion.LookRotation(m_TargetPos - t.transform.position);

           // t.transform.rotation = Quaternion.Lerp(quaternion, t.transform.rotation, 0.1f);
        }
        public void UpdateCamera(float x, float y, float z)
        {
            m_TargetPos = new Vector3(x, y, z);
        }


        private UnityEngine.Camera m_MainCamera;

        private float m_Distance = 12f;
        private float m_FixYaw = 0;
        private float m_FixPitch = 42f;

        private Vector3 m_TargetPos = Vector3.zero;
        private Vector3 m_Speed = Vector3.zero;

        private Vector3 m_CameraSpeed = Vector3.zero;
    }
}
