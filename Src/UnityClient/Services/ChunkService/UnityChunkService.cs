using System;
using System.Collections.Generic;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    public class UnityChunkService : Service
    {
        public UnityChunkService(Contexts contexts) : base(contexts)
        {
        }
        public void LoadChunk(IChunk chunk)
        {
            BoxCollider[] baseColliders = chunk.GetBaseCollider();
            foreach(var collider in baseColliders)
            {
            }
        }
    }
}
