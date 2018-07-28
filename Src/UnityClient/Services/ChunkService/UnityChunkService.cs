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
            foreach (var chunkDoor in chunkDoors)
            {
                LoadChunkDoor(unityChunk, chunkDoor);
            }

            UnityChunkTrigger[] chunkTriggers = unityChunk.GetComponentsInChildren<UnityChunkTrigger>();
            foreach (var chunkTrigger in chunkTriggers)
            {
                LoadChunkTrigger(unityChunk, chunkTrigger);
            }

            unityChunk.Init(chunkDoors, chunkTriggers);
            OpenDoor(chunk);
        }
        public void OpenDoor(IChunk chunk)
        {
            UnityChunk unityChunk = chunk as UnityChunk;

            UnityChunkDoor[] doors = unityChunk.Doors;
            foreach (var door in doors)
            {
                door.gameObject.SetActive(false);
            }
        }
        public void CloseDoor(IChunk chunk)
        {
            UnityChunk unityChunk = chunk as UnityChunk;

            UnityChunkDoor[] doors = unityChunk.Doors;
            foreach (var door in doors)
            {
                door.gameObject.SetActive(true);
            }
        }
        public void SummonNpc(IChunk chunk)
        {
            UnityChunk unityChunk = chunk as UnityChunk;

            foreach (var npcSpawn in unityChunk.m_Npcs)
            {

                uint entityId = IdSystem.Instance.GenId(IdEnum.Entity);
                Vector3 position = unityChunk.gameObject.transform.TransformPoint(npcSpawn.Position);
                float rotation = 0;

                Services.Instance.CreateCharacterService.CreateCharacter(entityId, npcSpawn.NpcId, (int)CampId.Blue, position, rotation);
            }
        }
        private void LoadChunkDoor(UnityChunk chunk, UnityChunkDoor chunkDoor)
        {
            chunkDoor.Init(chunk);
        }
        private void LoadChunkTrigger(UnityChunk chunk, UnityChunkTrigger chunkTrigger)
        {
            chunkTrigger.Init(chunk);

            chunkTrigger.OnChunkTriggerEnter += OnChunkTriggerEnter;
        }
        private void OnChunkTriggerEnter(IChunk chunk, IView view)
        {
            UnityChunk unityChunk = chunk as UnityChunk;

            if (unityChunk.Triggered)
                return;

            UnityView unityView = view as UnityView;

            if (unityView.Entity is GameEntity gameEntity)
            {
                if (gameEntity.isMainPlayer)
                {
                    CloseDoor(chunk);
                    SummonNpc(chunk);
                    unityChunk.Triggered = true;
                }
            }


        }
    }
}
