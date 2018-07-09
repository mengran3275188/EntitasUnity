using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;
using UnityEngine;

namespace UnityDelegate
{
    internal class CameraManager : Singleton<CameraManager>
    {
        public void UpdateCamera(float x, float y, float z)
        {
            Transform cameraTransform = GetMainCamera().transform;

            UnityEngine.Vector3 targetCenter = new UnityEngine.Vector3(x, y, z);

            float originalTargetAngle = c_FixYaw;
            float currentAngle = cameraTransform.eulerAngles.y;

            float targetAngle = originalTargetAngle;


            currentAngle = UnityEngine.Mathf.SmoothDampAngle(currentAngle, targetAngle, ref m_AngleVelocity, m_AngularSmoothLag, m_AngularMaxSpeed, Time.deltaTime);

            LogUtil.Debug(Time.deltaTime.ToString());
            float currentHeight = cameraTransform.position.y;

            currentHeight = UnityEngine.Mathf.SmoothDamp(currentHeight, c_Height, ref m_HeightVelocity, c_HeightSmoothLag);

            m_CurDistance = UnityEngine.Mathf.SmoothDamp(m_CurDistance, c_Distance, ref m_DistanceVelocity, c_DistanceSmoothLag);

            UnityEngine.Quaternion currentRotation = UnityEngine.Quaternion.Euler(0, currentAngle, 0);

            UnityEngine.Vector3 pos = targetCenter;
            pos += currentRotation * UnityEngine.Vector3.back * m_CurDistance;

            pos.y = currentHeight;

            cameraTransform.position = pos;

            if(m_NeedLookAt)
            {
                if(!UnityEngine.Mathf.Approximately(currentHeight, c_Height) || !UnityEngine.Mathf.Approximately(m_CurDistance, c_Distance))
                    cameraTransform.LookAt(targetCenter);
                else
                    m_NeedLookAt = false;
            }
        }

        private Camera GetMainCamera()
        {
            if (m_MainCamera == null)
                m_MainCamera = Camera.main;
            return m_MainCamera;
        }

        private static float GetNearestDegree(float curDegree, float targetDegree)
        {
            float deltaDegree = targetDegree - curDegree;
            int multiple = (int)(deltaDegree / 360);
            if(multiple >= 1.0f || multiple <= -1.0f)
            {
                curDegree += multiple * 360;
            }
            return curDegree;
        }

        private UnityEngine.Camera m_MainCamera;

        private const float c_Height = 6;
        private const float c_Distance = 8.5f;

        private const float c_FixYaw = 0;
        private const float c_FixedPitch = 42f;


        private const float c_HeightSmoothLag = 0.3f;
        private const float c_DistanceSmoothLag = 3.0f;

        private float m_HeightVelocity = 0.0f;
        private float m_DistanceVelocity = 0.0f;

        private float m_AngleVelocity = 0.0f;
        private float m_AngularMaxSpeed = 15.0f;
        private float m_AngularSmoothLag = 0.3f;

        private float m_CurDistance = 0.0f;

        private bool m_NeedLookAt = true;
    }
}
