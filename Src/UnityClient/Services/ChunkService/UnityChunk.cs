using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    public class UnityChunk : MonoBehaviour, IChunk
    {
        public string m_ChunkName;
        public NpcSpawn[] m_Npcs;
        [Serializable]
        public class NpcSpawn
        {
            public Vector3 Position;
            public int NpcId;
        }
        public ChunkEntity Entity
        {
            get { return m_Entity; }
        }
        public UnityChunkDoor[] Doors
        {
            get { return m_Doors; }
        }
        public UnityChunkTrigger[] Triggers
        {
            get { return m_Triggers; }
        }
        public bool Triggered
        {
            get { return m_Triggered; }
            set { m_Triggered = value; }
        }
        public void Init(ChunkEntity entity, UnityChunkDoor[] doors, UnityChunkTrigger[] triggers)
        {
            m_Doors = doors;
            m_Triggers = triggers;
            m_Triggered = false;
            m_Entity = entity;
        }
        public void OnDrawGizmos()
        {
            Vector3 cubeSize = new Vector3(1, 2, 1);
            foreach(var npcSpawn in m_Npcs)
            {
                Gizmos.DrawCube(transform.TransformPoint(npcSpawn.Position) + cubeSize / 2, cubeSize);
            }
        }

        private bool m_Triggered = false;
        private ChunkEntity m_Entity;
        private UnityChunkDoor[] m_Doors;
        private UnityChunkTrigger[] m_Triggers;
    }
}
