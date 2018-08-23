using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public sealed class UnityRigid : MonoBehaviour, IRigidbody
    {
        [SerializeField]
        private Rigidbody m_Rigidbody;
        private bool m_IsKinematic = true;

        public int Layer
        {
            set { m_Rigidbody.gameObject.layer = value; }
        }

        public Vector3 Position
        {
            get { return m_Rigidbody.position; }
            set { m_Rigidbody.position = value; }
        }
        public Vector3 Velocity
        {
            get { return m_Rigidbody.velocity; }
            set { m_Rigidbody.velocity = value; }
        }
        public Quaternion Rotation
        {
            get { return m_Rigidbody.rotation; }
            set { m_Rigidbody.rotation = value; }
        }
        public bool IsKinematic
        {
            get { return m_IsKinematic; }
            set { m_IsKinematic = value; }
        }

        public void MovePosition(Vector3 pos)
        {
            m_Rigidbody.MovePosition(pos);
        }
    }
}
