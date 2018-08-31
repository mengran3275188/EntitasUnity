using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class DestoryEntitySystem : IExecuteSystem
    {
        public DestoryEntitySystem(Contexts contexts)
        {
            m_NeedDestroyEntities = contexts.game.GetGroup(GameMatcher.Destory);
        }
        public void Execute()
        {
            var entities = m_NeedDestroyEntities.GetEntities();
            foreach (var entity in entities)
            {
                if (entity.hasSkill && null != entity.skill.Instance)
                {
                    SkillSystem.Instance.ForceBreakSkill(entity);
                }
                if (entity.hasBuff)
                {
                    foreach (var pair in entity.buff.InstanceInfos)
                    {
                        int buffId = pair.Key;
                        var infos = pair.Value;
                        for (int i = infos.Count - 1; i >= 0; i--)
                        {
                            var info = infos[i];
                            info.BuffInstance.IsTerminated = true;
                            info.BuffInstance.SendMessage("onbreak");
                        }
                    }
                }

                // 一些资源的处理临时放在这里
                if (entity.hasView)
                {
                    Services.Instance.ViewService.RecylceAsset(entity);
                }
                if (entity.hasAttr)
                {
                    Services.Instance.HudService.RemoveHudHead(entity);
                }

                entity.Destroy();
            }
        }
        private readonly IGroup<GameEntity> m_NeedDestroyEntities;
    }
}
