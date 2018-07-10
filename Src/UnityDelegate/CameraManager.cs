﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityDelegate
{
    internal class CameraManager : Util.Singleton<CameraManager>
    {
        public void UpdateCamera(float x, float y, float z)
        {
            Transform t = GetMainCamera().transform;

            UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(0, c_FixDir, 0);

            UnityEngine.Vector3 pos = new UnityEngine.Vector3(x, y, z) + rotation * UnityEngine.Vector3.back * c_Distance + UnityEngine.Vector3.up * c_Height;

            t.transform.position = pos;

            t.transform.LookAt(new UnityEngine.Vector3(x, y, z));
        }

        private Camera GetMainCamera()
        {
            if (m_MainCamera == null)
                m_MainCamera = Camera.main;
            return m_MainCamera;
        }

        private UnityEngine.Camera m_MainCamera;

        private const float c_Height = 6;
        private const float c_Distance = 8.5f;
        private const float c_FixDir = 0;

        private Vector3 m_Speed = Vector3.zero;
    }
}
