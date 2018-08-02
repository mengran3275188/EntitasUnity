using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public class UnityChunkDoor : MonoBehaviour, IChunkDoor
    {
        public IChunk Chunk
        {
            get { return m_Chunk; }
        }
        public void Init(UnityChunk chunk)
        {
            m_Chunk = chunk;
        }
        private UnityChunk m_Chunk;
    }
}
