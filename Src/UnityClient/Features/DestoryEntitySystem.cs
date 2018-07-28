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
            foreach(var entity in entities)
            {

                // 一些资源的处理临时放在这里
                if(entity.hasView)
                {
                    Services.Instance.ViewService.RecylceAsset(entity);
                }
                if(entity.hasAttr)
                {
                    GfxSystem.RemoveHudHead(entity.resource.Value);
                }

                entity.Destroy();
            }
        }
        private readonly IGroup<GameEntity> m_NeedDestroyEntities;
    }
}
