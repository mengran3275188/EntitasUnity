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
                CharacterConfig config = CharacterConfigProvider.Instance.GetCharacterConfig(npcSpawn.NpcId);
                if (null != config)
                {
                    uint entityId = IdSystem.Instance.GenId(IdEnum.Entity);
                    GameEntity entity = _contexts.game.CreateEntity();
                    entity.AddId(entityId);

                    uint resId = IdSystem.Instance.GenId(IdEnum.Resource);
                    Services.Instance.ViewService.LoadAsset(entity, resId, config.Model);
                    entity.AddResource(resId);

                    entity.AddAnimation(config.ActionId, config.ActionPrefix);

                    Quaternion quaternion = Quaternion.Euler(0, 0, 0);
                    entity.AddMovement(Vector3.zero);
                    entity.physics.Rigid.Position = unityChunk.gameObject.transform.TransformPoint(npcSpawn.Position);

                    entity.AddPosition(unityChunk.gameObject.transform.TransformPoint(npcSpawn.Position));
                    entity.AddRotation(0);

                    var agent = new CharacterAgent();
                    agent.Init(entityId);
                    bool ret = agent.btload("");
                    agent.btsetcurrent("");
                    entity.AddAI(agent);

                    entity.AddSkill(null, null);
                    entity.AddBuff(new Dictionary<int, List<BuffInstanceInfo>>(), new List<StartBuffParam>());

                    entity.AddCamp((int)CampId.Blue);

                    AttributeConfig attrConfig = AttributeConfigProvider.Instance.GetAttributeConfig(1000);
                    if (null != attrConfig)
                    {
                        AttributeData attrData = new AttributeData();
                        attrData.SetAbsoluteByConfig(attrConfig);
                        entity.AddAttr(1000, attrData);
                        entity.ReplaceHp(attrData.HpMax);
                    }

                    entity.AddBorn(Contexts.sharedInstance.game.timeInfo.Time);

                }

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

            UnityView unityView = view as UnityView;

            if (unityView.Entity is GameEntity gameEntity)
            {
                if (gameEntity.isMainPlayer)
                {
                    CloseDoor(chunk);
                    SummonNpc(chunk);
                }
            }


        }
    }
}
