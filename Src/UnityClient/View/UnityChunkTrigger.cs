using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    public class UnityChunkTrigger : MonoBehaviour, IChunkTrigger
    {
        public delegate void ChunkTriggerEnterDelegate(IChunk chunk);

        public ChunkTriggerEnterDelegate OnChunkTriggerEnter;
        public IChunk Chunk
        {
            get { return m_Chunk; }
        }
        public void Init(UnityChunk chunk)
        {
            m_Chunk = chunk;
        }
        private void OnTriggerEnter(Collider collider)
        {
            if (null != OnChunkTriggerEnter)
                OnChunkTriggerEnter(m_Chunk);
        }
        private UnityChunk m_Chunk;
    }
}
