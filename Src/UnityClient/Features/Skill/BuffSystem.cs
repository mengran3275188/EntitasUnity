using System;
using System.Collections.Generic;
using Util;
using Entitas;
using Entitas.Data;
using ScriptableSystem;

namespace UnityClient
{
    public class BuffSystem : Singleton<BuffSystem>, IInitializeSystem, IExecuteSystem
    {
        public BuffSystem()
        {
            m_GameContext = Contexts.sharedInstance.game;
            m_ImpactEntities = m_GameContext.GetGroup(GameMatcher.Buff);
        }
        public void Initialize()
        {
        }
        public void Execute()
        {
            long time = (long)(m_GameContext.timeInfo.Time * 1000);
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
                        info.m_BuffInstance.Tick(time);
                        if (info.m_BuffInstance.IsTerminated)
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

                    BuffConfig buffConfig = BuffConfigProvider.Instance.GetBuffConfig(startParam.BuffId);
                    if (null != buffConfig)
                    {
                        int maxCount = buffConfig.MaxCount;


                        List<BuffInstanceInfo> infos;
                        if (!buffComponent.InstanceInfos.TryGetValue(startParam.BuffId, out infos))
                        {
                            infos = new List<BuffInstanceInfo>();
                            buffComponent.InstanceInfos.Add(startParam.BuffId, infos);
                        }
                        if(maxCount > 0 && infos.Count >= maxCount)
                        {
                            for(int i = 0; i < infos.Count - maxCount + 1; ++i)
                            {
                                if(!infos[i].m_BuffInstance.IsTerminated)
                                {
                                    infos[i].m_BuffInstance.SendMessage("onbreak");
                                    infos[i].m_BuffInstance.IsTerminated = true;
                                }
                            }
                        }

                        BuffInstanceInfo instance = NewBuffInstance(startParam.BuffId);
                        if (null != instance)
                        {
                            instance.m_BuffInstance.Sender = Contexts.sharedInstance.game.GetEntityWithId(startParam.SenderId);
                            instance.m_BuffInstance.SenderPosition = startParam.SenderPosition;
                            instance.m_BuffInstance.SenderDirection = startParam.SenderDirection;
                            instance.m_BuffInstance.Target = entity;
                            instance.m_BuffInstance.Context = null;
                            instance.m_BuffInstance.GlobalVariables = m_GlobalVariables;
                            instance.m_BuffInstance.Start();

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

            StartBuffParam param = new StartBuffParam();
            param.BuffId = buffId;
            param.SenderId = sender.id.value;
            param.SenderPosition = senderPosition;
            param.SenderDirection = direction;

            if (target.hasBuff)
            {
                target.buff.StartParams.Add(param);
            }
        }


        private BuffInstanceInfo NewBuffInstance(int buffId)
        {
            BuffInstanceInfo instanceInfo = GetUnusedBuffInstanceInfoFromPool(buffId);
            if (null == instanceInfo)
            {
                BuffConfig config = BuffConfigProvider.Instance.GetBuffConfig(buffId);
                if (null != config)
                    ConfigManager.Instance.LoadIfNotExist(buffId, 2, HomePath.Instance.GetAbsolutePath(config.Script));
                Instance instance = ConfigManager.Instance.NewInstance(buffId, 2);
                if (null == instance)
                {
                    LogUtil.Error("ImpactSystem.NewImpactInstance : Can't load impact config, impact:{0}.", buffId);
                }
                BuffInstanceInfo res = new BuffInstanceInfo();
                res.m_BuffId = buffId;
                res.m_BuffInstance = instance;
                res.m_IsUsed = true;

                AddImpactInstanceInfoToPool(buffId, res);
                return res;
            }
            else
            {
                instanceInfo.m_IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleImpactInstance(BuffInstanceInfo info)
        {
            info.m_BuffInstance.Reset();
            info.m_IsUsed = false;
        }
        private void AddImpactInstanceInfoToPool(int buffId, BuffInstanceInfo info)
        {
            List<BuffInstanceInfo> infos;
            if (m_BuffInstancePool.TryGetValue(buffId, out infos))
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
            List<BuffInstanceInfo> infos;
            if (m_BuffInstancePool.TryGetValue(buffId, out infos))
            {
                foreach (var buffInfo in infos)
                {
                    if (!buffInfo.m_IsUsed)
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
        private readonly GameContext m_GameContext;
    }
}
