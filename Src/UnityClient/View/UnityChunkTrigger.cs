using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    public class UnityChunkTrigger : MonoBehaviour, IChunkTrigger
    {
        public delegate void ChunkTriggerEnterDelegate(IChunk chunk, IView view);

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
            UnityView view = collider.gameObject.GetComponent<UnityView>();
            if(null != view)
            {
                if (null != OnChunkTriggerEnter)
                    OnChunkTriggerEnter(m_Chunk, view);
            }
        }
        private UnityChunk m_Chunk;
    }
}
