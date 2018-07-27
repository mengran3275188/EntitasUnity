using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    [RequireComponent(typeof(Collider))]
    public sealed class UnityRigid : MonoBehaviour, IRigidbody
    {
        [SerializeField]
        private Rigidbody m_Rigidbody;

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
        public void OnTriggerEnter(Collider hit)
        {
            Util.LogUtil.Debug("On Trigger Enter!");
        }
    }
}
