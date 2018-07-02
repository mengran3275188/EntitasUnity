using System;
using System.Collections.Generic;
using ScriptableSystem;
using Entitas;
using Util;
using Entitas.Data;

namespace UnityClient
{
    public class SkillSystem : Singleton<SkillSystem>, IInitializeSystem, IExecuteSystem
    {
        public SkillSystem()
        {
            m_GameContext = Contexts.sharedInstance.game;
            m_SkillEntities = m_GameContext.GetGroup(GameMatcher.Skill);
        }
        public void Initialize()
        {
            GfxSystem.EventForLogic.Subscribe<int>("player_use_skill", "skill_system", this.PlayUseSkill);

            CommandManager.Instance.RegisterCommandFactory("animation", new CommandFactoryHelper<SkillCommands.AnimationCommand>());
            CommandManager.Instance.RegisterCommandFactory("animationspeed", new CommandFactoryHelper<SkillCommands.AnimationSpeedCommand>());
            CommandManager.Instance.RegisterCommandFactory("movechild", new CommandFactoryHelper<SkillCommands.MoveChildTrigger>());
            CommandManager.Instance.RegisterCommandFactory("curvemove", new CommandFactoryHelper<SkillCommands.CurveMoveCommand>());

            CommandManager.Instance.RegisterCommandFactory("areadamage", new CommandFactoryHelper<SkillCommands.AreaDamageCommand>());
            CommandManager.Instance.RegisterCommandFactory("destroyself", new CommandFactoryHelper<SkillCommands.DestroySelfCommand>());
            CommandManager.Instance.RegisterCommandFactory("colliderdamage", new CommandFactoryHelper<SkillCommands.ColliderDamageCommand>());
            CommandManager.Instance.RegisterCommandFactory("removecollider", new CommandFactoryHelper<SkillCommands.RemoveColliderCommand>());
        }

        public void Execute()
        {
            long time = (long)(m_GameContext.timeInfo.Time * 1000);

            var entities = m_SkillEntities.GetEntities();
            foreach (GameEntity entity in entities)
            {
                Entitas.Component.SkillComponent skillComponent = entity.skill;
                SkillInstanceInfo info = skillComponent.Instance;
                if (null != info)
                {
                    info.m_SkillInstance.Tick(time);
                    if(info.m_SkillInstance.IsTerminated)
                    {
                        RecycleSkillInstance(info);

                        entity.ReplaceSkill(null);

                        UpdateSkillControlMoveAndRotation(entity, false, false);
                    }
                }
            }
        }
        public void StartSkill(GameEntity sender, GameEntity target, int skillId)
        {
            if(target.hasSkill && target.skill.Instance == null)
            {
                SkillInstanceInfo instance = NewSkillInstance(skillId);
                if(null != instance)
                {
                    instance.m_SkillInstance.Sender = sender;
                    instance.m_SkillInstance.Target = target;
                    instance.m_SkillInstance.Context = null;
                    instance.m_SkillInstance.GlobalVariables = m_GlobalVariables;
                    instance.m_SkillInstance.Start();

                    target.ReplaceSkill(instance);

                    UpdateSkillControlMoveAndRotation(target, true, true);
                }
            }
        }

        private void UpdateSkillControlMoveAndRotation(GameEntity entity, bool controlMove, bool controlRotation)
        {
            entity.ReplaceMovement(controlMove ? Entitas.Data.MoveState.SkillMoving : Entitas.Data.MoveState.Idle,
                                   entity.hasMovement ? entity.movement.MovingDir : 0, 0);
            entity.ReplaceRotation(controlRotation ? Entitas.Data.RotateState.SkillRotate : Entitas.Data.RotateState.UserRotate,
                                   entity.hasRotation ? entity.rotation.RotateDir : 0);
        }
        private void PlayUseSkill(int skillId)
        {
            var mainPlayer = m_GameContext.GetGroup(GameMatcher.MainPlayer).GetSingleEntity();
            if(null != mainPlayer)
                StartSkill(mainPlayer, mainPlayer, skillId);
        }
        private SkillInstanceInfo NewSkillInstance(int skillId)
        {
            SkillInstanceInfo instanceInfo = GetUnusedSkillInstanceInfoFromPool(skillId);
            if(null == instanceInfo)
            {
                //do load
                SkillConfig config = SkillConfigProvider.Instance.GetSkillConfig(skillId);
                if(null != config)
                    ConfigManager.Instance.LoadIfNotExist(skillId, 0, HomePath.Instance.GetAbsolutePath(config.Script));

                Instance instance = ConfigManager.Instance.NewInstance(skillId, 0);
                if(null == instance)
                {
                    LogUtil.Error("Can't load skill config, skill:{0}!", skillId);
                }
                SkillInstanceInfo res = new SkillInstanceInfo();
                res.m_SkillId = skillId;
                res.m_SkillInstance = instance;
                res.m_IsUsed = true;

                AddStoryInstanceInfoToPool(skillId, res);
                return res;
            }
            else
            {
                instanceInfo.m_IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleSkillInstance(SkillInstanceInfo info)
        {
            info.m_SkillInstance.Reset();
            info.m_IsUsed = false;
        }
        private void AddStoryInstanceInfoToPool(int skillId, SkillInstanceInfo info)
        {
            List<SkillInstanceInfo> infos;
            if(m_SkillInstancePool.TryGetValue(skillId, out infos))
            {
                infos.Add(info);
            }
            else
            {
                infos = new List<SkillInstanceInfo>();
                infos.Add(info);
                m_SkillInstancePool.Add(skillId, infos);
            }
        }
        private SkillInstanceInfo GetUnusedSkillInstanceInfoFromPool(int skillId)
        {
            SkillInstanceInfo info = null;
            List<SkillInstanceInfo> infos;
            if(m_SkillInstancePool.TryGetValue(skillId, out infos))
            {
                foreach(var skillInfo in infos)
                {
                    if (!skillInfo.m_IsUsed)
                    {
                        info = skillInfo;
                        break;
                    }
                }
            }
            return info;
        }
        private Dictionary<string, object> m_GlobalVariables = new Dictionary<string, object>();
        private List<SkillInstanceInfo> m_SkillLogicInfos = new List<SkillInstanceInfo>();
        private Dictionary<int, List<SkillInstanceInfo>> m_SkillInstancePool = new Dictionary<int, List<SkillInstanceInfo>>();

        private readonly IGroup<GameEntity> m_SkillEntities;
        private readonly GameContext m_GameContext;
    }
}
