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

            CommandManager.Instance.RegisterCommandFactory("effect", new CommandFactoryHelper<SkillCommands.EffectCommand>());
            CommandManager.Instance.RegisterCommandFactory("damage", new CommandFactoryHelper<SkillCommands.DamageCommand>());
            CommandManager.Instance.RegisterCommandFactory("teleport", new CommandFactoryHelper<SkillCommands.TeleportCommand>());
            CommandManager.Instance.RegisterCommandFactory("selfbuff", new CommandFactoryHelper<SkillCommands.SelfBuffCommand>());

            CommandManager.Instance.RegisterCommandFactory("enablemove", new CommandFactoryHelper<SkillCommands.EnableMoveCommand>());
            CommandManager.Instance.RegisterCommandFactory("enablerotation", new CommandFactoryHelper<SkillCommands.EnableRotationCommand>());
        }

        public void Execute()
        {
            long time = (long)(m_GameContext.timeInfo.Time * 1000);

            var entities = m_SkillEntities.GetEntities();
            foreach (GameEntity entity in entities)
            {
                SkillComponent skillComponent = entity.skill;
                SkillInstanceInfo info = skillComponent.Instance;
                if (null != info)
                {
                    info.m_SkillInstance.Tick(time);
                    if(info.m_SkillInstance.IsTerminated)
                    {
                        RecycleSkillInstance(info);

                        skillComponent.Instance = null;
                    }
                }
                if(null != skillComponent.StartParam)
                {
                    if(skillComponent.Instance == null)
                    {

                        SkillInstanceInfo instance = NewSkillInstance(skillComponent.StartParam.SkillId);
                        if(null != instance)
                        {
                            if(null != instance.m_SkillInstance)
                            {
                                instance.m_SkillInstance.Sender = Contexts.sharedInstance.game.GetEntityWithId(skillComponent.StartParam.SenderId);
                                instance.m_SkillInstance.Target = entity;
                                instance.m_SkillInstance.SenderPosition = skillComponent.StartParam.SenderPosition; ;
                                instance.m_SkillInstance.SenderDirection = skillComponent.StartParam.SenderDirection;
                                instance.m_SkillInstance.Context = null;
                                instance.m_SkillInstance.GlobalVariables = m_GlobalVariables;
                                instance.m_SkillInstance.Start();

                                entity.ReplaceSkill(instance, null);
                            }
                            else
                            {
                                entity.ReplaceSkill(null, null);
                            }

                        }
                    }
                    else
                    {
                        skillComponent.StartParam = null;
                    }
                }
            }
        }
        public void StartSkill(GameEntity sender, GameEntity target, int skillId, Vector3 senderPosition, float direction)
        {
            StartSkillParam param = new StartSkillParam();
            param.SkillId = skillId;
            param.SenderId = sender.id.value;
            param.SenderPosition = senderPosition;
            param.SenderDirection = direction;

            if(target.hasSkill)
            {
                target.skill.StartParam = param;
            }

        }
        public void BreakSkill(GameEntity target)
        {
            if(target.hasSkill && target.skill.Instance != null)
            {
                target.skill.Instance.m_SkillInstance.SendMessage("onbreak");
            }
        }
        private void PlayUseSkill(int skillId)
        {
            var mainPlayer = m_GameContext.GetGroup(GameMatcher.MainPlayer).GetSingleEntity();
            if(null != mainPlayer)
                StartSkill(mainPlayer, mainPlayer, skillId, mainPlayer.position.Value, mainPlayer.rotation.RotateDir);
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
