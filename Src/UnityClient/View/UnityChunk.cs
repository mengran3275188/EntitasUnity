using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    public class UnityChunk : MonoBehaviour, IChunk
    {
        public BoxCollider[] m_BaseCollider;
        public BoxCollider[] m_Doors;
        public BoxCollider[] m_Triggers;
        public BoxCollider[] GetBaseCollider()
        {
            return m_BaseCollider;
        }
        public BoxCollider[] GetDoorsCollider()
        {
            return m_Doors;
        }
        public BoxCollider[] GetTriggers()
        {
            return m_Triggers;
        }
    }
}
