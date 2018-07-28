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
            UnityChunk unityChunk = chunk as UnityChunk;

            UnityChunkDoor[] chunkDoors = unityChunk.GetComponentsInChildren<UnityChunkDoor>();
            foreach(var chunkDoor in chunkDoors)
            {
                LoadChunkDoor(unityChunk, chunkDoor);
            }

            UnityChunkTrigger[] chunkTriggers = unityChunk.GetComponentsInChildren<UnityChunkTrigger>();
            foreach(var chunkTrigger in chunkTriggers)
            {
                LoadChunkTrigger(unityChunk, chunkTrigger);
            }

            unityChunk.Init(chunkDoors, chunkTriggers);
        }
        public void LoadChunkDoor(UnityChunk chunk, UnityChunkDoor chunkDoor)
        {
            chunkDoor.Init(chunk);
        }
        public void LoadChunkTrigger(UnityChunk chunk, UnityChunkTrigger chunkTrigger)
        {
            chunkTrigger.Init(chunk);

            chunkTrigger.OnChunkTriggerEnter += OnChunkTriggerEnter;
        }
        private void OnChunkTriggerEnter(IChunk chunk)
        {
            Util.LogUtil.Info("On chunk trigger enter!");
        }
    }
}
