using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public class UnityCameraService : Service, ICameraService
    {
        public Quaternion Rotation
        {
            get { return GetTransform().rotation; }
        }
        public UnityCameraService(Contexts contexts) : base(contexts)
        {
        }

        public void SetRotation(Quaternion rotation)
        {
            GetTransform().rotation = rotation;
        }
        public void SetPosition(Vector3 position)
        {
            GetTransform().position = Vector3.SmoothDamp(GetTransform().position, position, ref m_Speed, 0.1f);
        }
        public Vector3 WorldToScreenPoint(Vector3 pos)
        {
            return GetMainCamera().WorldToScreenPoint(pos);
        }
        public Vector3 WorldToViewportPoint(Vector3 pos)
        {
            return GetMainCamera().WorldToViewportPoint(pos);
        }

        private Transform GetTransform()
        {
            if (null == m_Transform)
                m_Transform = Camera.main.transform;
            return m_Transform;
        }
        private Camera GetMainCamera()
        {
            if (null == m_Main)
                m_Main = Camera.main;
            return m_Main;
        }

        private Vector3 m_Speed = Vector3.zero;
        private Camera m_Main;
        private Transform m_Transform;
    }
}
