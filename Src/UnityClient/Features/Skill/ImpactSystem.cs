using System;
using System.Collections.Generic;
using Util;
using Entitas;
using Entitas.Data;
using ScriptableSystem;

namespace UnityClient
{
    public class ImpactSystem : Singleton<ImpactSystem>, IInitializeSystem, IExecuteSystem
    {
        public ImpactSystem()
        {
            m_GameContext = Contexts.sharedInstance.game;
            m_ImpactEntities = m_GameContext.GetGroup(GameMatcher.Impact);
        }
        public void Initialize()
        {
        }
        public void Execute()
        {
            long time = (long)(m_GameContext.timeInfo.Time * 1000);
            foreach (GameEntity entity in m_ImpactEntities)
            {
                Entitas.Component.ImpactComponent impactComponent = entity.impact;
                ImpactInstanceInfo info = impactComponent.Instance;
                if (null != info)
                {
                    info.m_ImpactInstance.Tick(time);
                    if(info.m_ImpactInstance.IsTerminated)
                    {
                        RecycleImpactInstance(info);

                        entity.ReplaceImpact(null);

                        UpdateImpactControlMoveAndRotation(entity, false, false);
                    }
                }
            }
        }
        public void StartImpact(GameEntity sender, GameEntity target, int impactId)
        {
            if(target.hasImpact && target.impact.Instance == null)
            {
                ImpactInstanceInfo instance = NewImpactInstance(impactId);
                if(null != instance)
                {
                    instance.m_ImpactInstance.Sender = sender;
                    instance.m_ImpactInstance.Target = target;
                    instance.m_ImpactInstance.Context = null;
                    instance.m_ImpactInstance.GlobalVariables = m_GlobalVariables;
                    instance.m_ImpactInstance.Start();

                    target.ReplaceImpact(instance);

                    UpdateImpactControlMoveAndRotation(target, true, true);
                }
            }
        }

        private void UpdateImpactControlMoveAndRotation(GameEntity entity, bool controlMove, bool controlRotation)
        {
            entity.ReplaceMovement(controlMove ? Entitas.Data.MoveState.ImpactMoving : Entitas.Data.MoveState.Idle,
                                   entity.hasMovement ? entity.movement.MovingDir : 0, 0);
            entity.ReplaceRotation(controlRotation ? Entitas.Data.RotateState.ImpactRotate : Entitas.Data.RotateState.UserRotate,
                                   entity.hasRotation ? entity.rotation.RotateDir : 0);
        }

        private ImpactInstanceInfo NewImpactInstance(int impactId)
        {
            ImpactInstanceInfo instanceInfo = GetUnusedImpactInstanceInfoFromPool(impactId);
            if(null == instanceInfo)
            {
                ConfigManager.Instance.LoadIfNotExist(impactId, 2, HomePath.Instance.GetAbsolutePath("Tables/Impact/test.mr"));
                Instance instance = ConfigManager.Instance.NewInstance(impactId, 2);
                if(null == instance)
                {
                    LogUtil.Error("ImpactSystem.NewImpactInstance : Can't load impact config, impact:{0}.", impactId);
                }
                ImpactInstanceInfo res = new ImpactInstanceInfo();
                res.m_ImpactId = impactId;
                res.m_ImpactInstance = instance;
                res.m_IsUsed = true;

                AddImpactInstanceInfoToPool(impactId, res);
                return res;
            }
            else
            {
                instanceInfo.m_IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleImpactInstance(ImpactInstanceInfo info)
        {
            info.m_ImpactInstance.Reset();
            info.m_IsUsed = false;
        }
        private void AddImpactInstanceInfoToPool(int impactId, ImpactInstanceInfo info)
        {
            List<ImpactInstanceInfo> infos;
            if(m_ImpactInstancePool.TryGetValue(impactId, out infos))
            {
                infos.Add(info);
            }
            else
            {
                infos = new List<ImpactInstanceInfo>();
                infos.Add(info);
                m_ImpactInstancePool.Add(impactId, infos);
            }
        }
        private ImpactInstanceInfo GetUnusedImpactInstanceInfoFromPool(int impactId)
        {
            ImpactInstanceInfo info = null;
            List<ImpactInstanceInfo> infos;
            if(m_ImpactInstancePool.TryGetValue(impactId, out infos))
            {
                foreach(var impactInfo in infos)
                {
                    if(!impactInfo.m_IsUsed)
                    {
                        info = impactInfo;
                        break;
                    }
                }
            }
            return info;
        }

        private Dictionary<string, object> m_GlobalVariables = new Dictionary<string, object>();
        private List<ImpactInstanceInfo> m_ImpactLogicInfos = new List<ImpactInstanceInfo>();
        private Dictionary<int, List<ImpactInstanceInfo>> m_ImpactInstancePool = new Dictionary<int, List<ImpactInstanceInfo>>();

        private readonly IGroup<GameEntity> m_ImpactEntities;
        private readonly GameContext m_GameContext;
    }
}
