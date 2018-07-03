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
                Entitas.Component.BuffComponent buffComponent = entity.buff;
                for(int i = buffComponent.InstanceInfos.Count - 1; i >= 0; i --)
                {
                    var info = buffComponent.InstanceInfos[i];
                    info.m_BuffInstance.Tick(time);
                    if(info.m_BuffInstance.IsTerminated)
                    {
                        RecycleImpactInstance(info);

                        buffComponent.InstanceInfos.Remove(info);

                        UpdateBuffControlMoveAndRotation(entity, false, false);
                    }
                }
            }
        }
        public void StartBuff(GameEntity sender, GameEntity target, int impactId, Vector3 position, float direction)
        {
            if(target.hasBuff)
            {
                SkillSystem.Instance.BreakSkill(target);

                BuffInstanceInfo instance = NewBuffInstance(impactId);
                if(null != instance)
                {
                    instance.m_BuffInstance.Sender = sender;
                    instance.m_BuffInstance.SenderPosition = position;
                    instance.m_BuffInstance.SenderDirection = direction;
                    instance.m_BuffInstance.Target = target;
                    instance.m_BuffInstance.Context = null;
                    instance.m_BuffInstance.GlobalVariables = m_GlobalVariables;
                    instance.m_BuffInstance.Start();

                    target.buff.InstanceInfos.Add(instance);

                    UpdateBuffControlMoveAndRotation(target, true, true);
                }
            }
        }

        private void UpdateBuffControlMoveAndRotation(GameEntity entity, bool controlMove, bool controlRotation)
        {
            entity.ReplaceMovement(controlMove ? Entitas.Data.MoveState.ImpactMoving : Entitas.Data.MoveState.Idle,
                                   entity.hasMovement ? entity.movement.MovingDir : 0, 0);
            entity.ReplaceRotation(controlRotation ? Entitas.Data.RotateState.ImpactRotate : Entitas.Data.RotateState.UserRotate,
                                   entity.hasRotation ? entity.rotation.RotateDir : 0);
        }

        private BuffInstanceInfo NewBuffInstance(int buffId)
        {
            BuffInstanceInfo instanceInfo = GetUnusedBuffInstanceInfoFromPool(buffId);
            if(null == instanceInfo)
            {
                ConfigManager.Instance.LoadIfNotExist(buffId, 2, HomePath.Instance.GetAbsolutePath("Tables/Impact/test.mr"));
                Instance instance = ConfigManager.Instance.NewInstance(buffId, 2);
                if(null == instance)
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
            if(m_BuffInstancePool.TryGetValue(buffId, out infos))
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
            if(m_BuffInstancePool.TryGetValue(buffId, out infos))
            {
                foreach(var buffInfo in infos)
                {
                    if(!buffInfo.m_IsUsed)
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
