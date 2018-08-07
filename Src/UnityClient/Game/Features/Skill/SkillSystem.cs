using System;
using System.Collections.Generic;
using ScriptableSystem;
using Entitas;
using Util;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public class SkillSystem : Singleton<SkillSystem>, IInitializeSystem, IExecuteSystem
    {
        public SkillSystem()
        {
            m_Contexts = Contexts.sharedInstance;
            m_SkillEntities = m_Contexts.game.GetGroup(GameMatcher.Skill);
        }
        public void Initialize()
        {
            GfxSystem.EventForLogic.Subscribe<int>("player_use_skill", "skill_system", this.PlayUseSkill);

            CommandManager.Instance.RegisterCommandFactory("animation", new CommandFactoryHelper<SkillCommands.AnimationCommand>());
            CommandManager.Instance.RegisterCommandFactory("animationspeed", new CommandFactoryHelper<SkillCommands.AnimationSpeedCommand>());
            CommandManager.Instance.RegisterCommandFactory("effect", new CommandFactoryHelper<SkillCommands.EffectCommand>());
            CommandManager.Instance.RegisterCommandFactory("movechild", new CommandFactoryHelper<SkillCommands.MoveChildTrigger>());

            CommandManager.Instance.RegisterCommandFactory("curvemove", new CommandFactoryHelper<SkillCommands.CurveMoveCommand>());
            CommandManager.Instance.RegisterCommandFactory("teleport", new CommandFactoryHelper<SkillCommands.TeleportCommand>());

            CommandManager.Instance.RegisterCommandFactory("areadamage", new CommandFactoryHelper<SkillCommands.AreaDamageCommand>());
            CommandManager.Instance.RegisterCommandFactory("destroyself", new CommandFactoryHelper<SkillCommands.DestroySelfCommand>());
            CommandManager.Instance.RegisterCommandFactory("colliderdamage", new CommandFactoryHelper<SkillCommands.ColliderDamageCommand>());
            CommandManager.Instance.RegisterCommandFactory("damage", new CommandFactoryHelper<SkillCommands.DamageCommand>());

            CommandManager.Instance.RegisterCommandFactory("selfbuff", new CommandFactoryHelper<SkillCommands.SelfBuffCommand>());

            CommandManager.Instance.RegisterCommandFactory("disablemoveinput", new CommandFactoryHelper<SkillCommands.DisableMoveInputCommand>());
            CommandManager.Instance.RegisterCommandFactory("disablerotationinput", new CommandFactoryHelper<SkillCommands.DisableRotationInputCommand>());

            CommandManager.Instance.RegisterCommandFactory("breaksection", new CommandFactoryHelper<SkillCommands.BreakSectionCommand>());
        }

        public void Execute()
        {
            long time = (long)(m_Contexts.input.time.Value * 1000);

            var entities = m_SkillEntities.GetEntities();
            foreach (GameEntity entity in entities)
            {
                SkillComponent skillComponent = entity.skill;
                SkillInstanceInfo info = skillComponent.Instance;
                if (null != info)
                {
                    info.SkillInstance.Tick(time);
                    if(info.SkillInstance.IsTerminated)
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
                        if (null != instance && null != instance.SkillInstance)
                        {
                            instance.SkillInstance.Sender = Contexts.sharedInstance.game.GetEntityWithId(skillComponent.StartParam.SenderId);
                            instance.SkillInstance.Target = entity;
                            instance.SkillInstance.SenderPosition = skillComponent.StartParam.SenderPosition; ;
                            instance.SkillInstance.SenderDirection = skillComponent.StartParam.SenderDirection;
                            instance.SkillInstance.Context = null;
                            instance.SkillInstance.GlobalVariables = m_GlobalVariables;
                            instance.SkillInstance.Start();

                            entity.ReplaceSkill(instance, null);

                        }
                        else
                        {
                            entity.ReplaceSkill(null, null);
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

            SkillConfig config = SkillConfigProvider.Instance.GetSkillConfig(skillId);
            if(null == config)
            {
                LogUtil.Error("SkillSystem.StartSkill can not find skill config {0}.", skillId);
                return;
            }

            StartSkillParam param = new StartSkillParam
            {
                SkillId = skillId,
                SenderId = sender.id.value,
                SenderPosition = senderPosition,
                SenderDirection = direction
            };
            if (target.hasSkill)
            {
                target.skill.StartParam = param;
            }

            // break
            float time = Contexts.sharedInstance.input.time.Value;
            if(CanBreakCurSkill(target, config.BreakType, (long)(time * 1000)))
            {
                BreakSkill(target);
            }
        }
        public void BreakSkill(GameEntity entity)
        {
            if(entity.hasSkill && entity.skill.Instance != null)
            {
                entity.skill.Instance.SkillInstance.SendMessage("onbreak");
            }
        }
        public Vector3 GetSkillVelocity(GameEntity entity)
        {
            Vector3 velocity = Vector3.zero;
            if(entity.hasSkill && null != entity.skill.Instance)
            {
                velocity = entity.skill.Instance.SkillInstance.Velocity;
            }
            return velocity;
        }
        public bool IsDisableMoveInput(GameEntity entity)
        {
            if(entity.hasSkill && null != entity.skill.Instance)
            {
                return entity.skill.Instance.SkillInstance.DisableMoveInput;
            }
            return false;
        }
        public bool IsDisableRotationInput(GameEntity entity)
        {
            if(entity.hasSkill && null != entity.skill.Instance)
            {
                return entity.skill.Instance.SkillInstance.DisableRotationInput;
            }
            return false;
        }
        private bool CanBreakCurSkill(GameEntity entity, int breakType, long time)
        {
            if (!entity.hasSkill)
                return false;

            if (null == entity.skill.Instance)
                return true;

            foreach(var bs in entity.skill.Instance.BreakSections)
            {
                if (bs.BreakType == breakType && time >= bs.StartTime && time <= bs.EndTime)
                    return true;
            }
            return false;
        }
        private void PlayUseSkill(int skillId)
        {
            var mainPlayer = m_Contexts.game.GetGroup(GameMatcher.MainPlayer).GetSingleEntity();
            if(null != mainPlayer)
                StartSkill(mainPlayer, mainPlayer, skillId, mainPlayer.position.Value, mainPlayer.rotation.Value);
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
                SkillInstanceInfo res = new SkillInstanceInfo
                {
                    SkillId = skillId,
                    SkillInstance = instance,
                    BreakSections = new List<BreakSection>(),
                    IsUsed = true
                };

                AddStoryInstanceInfoToPool(skillId, res);
                return res;
            }
            else
            {
                instanceInfo.IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleSkillInstance(SkillInstanceInfo info)
        {
            info.SkillInstance.Reset();
            info.BreakSections.Clear();
            info.IsUsed = false;
        }
        private void AddStoryInstanceInfoToPool(int skillId, SkillInstanceInfo info)
        {
            if(m_SkillInstancePool.TryGetValue(skillId, out List<SkillInstanceInfo> infos))
            {
                infos.Add(info);
            }
            else
            {
                infos = new List<SkillInstanceInfo> { info };
                m_SkillInstancePool.Add(skillId, infos);
            }
        }
        private SkillInstanceInfo GetUnusedSkillInstanceInfoFromPool(int skillId)
        {
            SkillInstanceInfo info = null;
            if(m_SkillInstancePool.TryGetValue(skillId, out List<SkillInstanceInfo> infos))
            {
                foreach(var skillInfo in infos)
                {
                    if (!skillInfo.IsUsed)
                    {
                        info = skillInfo;
                        break;
                    }
                }
            }
            return info;
        }
        private readonly Dictionary<string, object> m_GlobalVariables = new Dictionary<string, object>();
        private Dictionary<int, List<SkillInstanceInfo>> m_SkillInstancePool = new Dictionary<int, List<SkillInstanceInfo>>();

        private readonly IGroup<GameEntity> m_SkillEntities;
        private readonly Contexts m_Contexts;
    }
}
