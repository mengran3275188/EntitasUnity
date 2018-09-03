using System;
using System.Collections.Generic;
using Util;
using Entitas;
using Entitas.Data;
using ScriptableData;
using UnityEngine;

namespace UnityClient
{
    public class BuffSystem : Singleton<BuffSystem>, IInitializeSystem, IExecuteSystem
    {
        public BuffSystem()
        {
            m_Contexts = Contexts.sharedInstance;
            m_ImpactEntities = m_Contexts.game.GetGroup(GameMatcher.Buff);
        }
        public void Initialize()
        {
        }
        public void Execute()
        {
            long time = (long)(m_Contexts.input.time.Value * 1000);
            foreach (GameEntity entity in m_ImpactEntities)
            {
                BuffComponent buffComponent = entity.buff;
                foreach (var pair in buffComponent.InstanceInfos)
                {
                    int buffId = pair.Key;
                    var infos = pair.Value;
                    for (int i = infos.Count - 1; i >= 0; i--)
                    {
                        var info = infos[i];
                        info.BuffInstance.Tick(time);
                        if (info.BuffInstance.IsTerminated)
                        {
                            RecycleImpactInstance(info);

                            entity.isBuffAttrChanged = true;

                            infos.Remove(info);

                        }
                    }
                }
                foreach (var startParam in buffComponent.StartParams)
                {
                    //SkillSystem.Instance.BreakSkill(entity);

                    BuffConfig buffConfig = BuffConfigProvider.Instance.GetBuffConfig(startParam.Id);
                    if (null != buffConfig)
                    {
                        int maxCount = buffConfig.MaxCount;

                        if (!buffComponent.InstanceInfos.TryGetValue(startParam.Id, out List<BuffInstanceInfo> infos))
                        {
                            infos = new List<BuffInstanceInfo>();
                            buffComponent.InstanceInfos.Add(startParam.Id, infos);
                        }
                        if(maxCount == 0 && infos.Count > 0)
                        {
                            continue;
                        }else if(maxCount > 0 && infos.Count >= maxCount)
                        {
                            for(int i = 0; i < infos.Count - maxCount + 1; ++i)
                            {
                                if(!infos[i].BuffInstance.IsTerminated)
                                {
                                    infos[i].BuffInstance.SendMessage("onbreak");
                                    infos[i].BuffInstance.IsTerminated = true;
                                }
                            }
                        }

                        BuffInstanceInfo instance = NewBuffInstance(startParam.Id);
                        if (null != instance)
                        {
                            instance.BuffInstance.SenderId = startParam.SenderId;
                            instance.BuffInstance.SenderPosition = startParam.SenderPosition;
                            instance.BuffInstance.SenderDirection = startParam.SenderDirection;
                            instance.BuffInstance.Target = entity;
                            instance.BuffInstance.Context = Contexts.sharedInstance.game;
                            instance.BuffInstance.AddVariable("@@id", entity.id.value);
                            instance.BuffInstance.GlobalVariables = m_GlobalVariables;
                            instance.BuffInstance.Start();

                            infos.Add(instance);

                            entity.isBuffAttrChanged = true;
                        }
                    }

                }
                buffComponent.StartParams.Clear();
            }
        }
        public void StartBuff(GameEntity sender, GameEntity target, int buffId, Vector3 senderPosition, float direction)
        {

            StartBuffParam param = new StartBuffParam
            {
                Id = buffId,
                SenderId = sender.id.value,
                SenderPosition = senderPosition,
                SenderDirection = direction
            };

            if (target.hasBuff)
            {
                target.buff.StartParams.Add(param);
            }
        }

        public Vector3 GetBuffVelocity(GameEntity entity)
        {
            Vector3 velocity = Vector3.zero;
            if(entity.hasBuff)
            {
                foreach (var pair in entity.buff.InstanceInfos)
                {
                    int buffId = pair.Key;
                    var infos = pair.Value;
                    for (int i = infos.Count - 1; i >= 0; i--)
                    {
                        var info = infos[i];
                        velocity += info.BuffInstance.Velocity;
                    }
                }
            }
            return velocity;
        }
        public bool IsDisableMoveInput(GameEntity entity)
        {
            if(entity.hasBuff)
            {
                foreach (var pair in entity.buff.InstanceInfos)
                {
                    int buffId = pair.Key;
                    var infos = pair.Value;
                    for (int i = infos.Count - 1; i >= 0; i--)
                    {
                        var info = infos[i];
                        if (info.BuffInstance.DisableMoveInput)
                            return true;
                    }
                }
            }
            return false;
        }
        public bool IsDisableRotationInput(GameEntity entity)
        {
            if(entity.hasBuff)
            {
                foreach (var pair in entity.buff.InstanceInfos)
                {
                    int buffId = pair.Key;
                    var infos = pair.Value;
                    for (int i = infos.Count - 1; i >= 0; i--)
                    {
                        var info = infos[i];
                        if (info.BuffInstance.DisableRotationInput)
                            return true;
                    }
                }
            }
            return false;
        }

        private BuffInstanceInfo NewBuffInstance(int buffId)
        {
            BuffInstanceInfo instanceInfo = GetUnusedBuffInstanceInfoFromPool(buffId);
            if (null == instanceInfo)
            {
                BuffConfig config = BuffConfigProvider.Instance.GetBuffConfig(buffId);
                if (null != config)
                    ConfigManager.Instance.LoadIfNotExist(buffId, 2, HomePath.Instance.GetAbsolutePath(config.Script));
                IInstance instance = ConfigManager.Instance.NewInstance(buffId, 2);
                if (null == instance)
                {
                    LogUtil.Error("ImpactSystem.NewImpactInstance : Can't load impact config, impact:{0}.", buffId);
                }
                BuffInstanceInfo res = new BuffInstanceInfo
                {
                    BuffId = buffId,
                    BuffInstance = instance,
                    IsUsed = true
                };

                AddImpactInstanceInfoToPool(buffId, res);
                return res;
            }
            else
            {
                instanceInfo.IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleImpactInstance(BuffInstanceInfo info)
        {
            info.BuffInstance.Reset();
            info.IsUsed = false;
        }
        private void AddImpactInstanceInfoToPool(int buffId, BuffInstanceInfo info)
        {
            if (m_BuffInstancePool.TryGetValue(buffId, out List<BuffInstanceInfo> infos))
            {
                infos.Add(info);
            }
            else
            {
                infos = new List<BuffInstanceInfo>();
                infos.Add(info);
                m_BuffInstancePool.Add(buffId, infos);
            }
        }
        private BuffInstanceInfo GetUnusedBuffInstanceInfoFromPool(int buffId)
        {
            BuffInstanceInfo info = null;
            if (m_BuffInstancePool.TryGetValue(buffId, out List<BuffInstanceInfo> infos))
            {
                foreach (var buffInfo in infos)
                {
                    if (!buffInfo.IsUsed)
                    {
                        info = buffInfo;
                        break;
                    }
                }
            }
            return info;
        }

        private Dictionary<string, object> m_GlobalVariables = new Dictionary<string, object>();
        private List<BuffInstanceInfo> m_ImpactLogicInfos = new List<BuffInstanceInfo>();
        private Dictionary<int, List<BuffInstanceInfo>> m_BuffInstancePool = new Dictionary<int, List<BuffInstanceInfo>>();

        private readonly IGroup<GameEntity> m_ImpactEntities;
        private readonly Contexts m_Contexts;
    }
}
