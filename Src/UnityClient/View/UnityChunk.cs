using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    public class UnityChunk : MonoBehaviour, IChunk
    {
        public void Init(UnityChunkDoor[] doors, UnityChunkTrigger[] triggers)
        {
            m_Doors = doors;
            m_Triggers = triggers;
        }

        private UnityChunkDoor[] m_Doors;
        private UnityChunkTrigger[] m_Triggers;
    }
}
