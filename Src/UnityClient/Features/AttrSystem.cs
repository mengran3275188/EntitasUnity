using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;
using Util;

namespace UnityClient
{
    public class AttrSystem : IExecuteSystem
    {
        public AttrSystem(Contexts contexts)
        {
            m_Context = contexts.game;
            m_AttrGroup = m_Context.GetGroup(GameMatcher.BuffAttrChanged);
        }
        public void Execute()
        {
            var entities = m_AttrGroup.GetEntities();
            foreach(var entity in entities)
            {
                entity.isBuffAttrChanged = false;

                if (!entity.hasAttr || !entity.hasBuff)
                    continue;

                AttributeConfig attrConfig = AttributeConfigProvider.Instance.GetAttributeConfig(entity.attr.Id);
                if(null != attrConfig)
                {
                    AttributeData attrData = entity.attr.Value;
                    attrData.SetAbsoluteByConfig(attrConfig);

                    foreach(var pair in entity.buff.InstanceInfos)
                    {
                        int buffId = pair.Key;
                        List<BuffInstanceInfo> buffInstances = pair.Value;
                        BuffConfig buffConfig = BuffConfigProvider.Instance.GetBuffConfig(buffId);
                        if(null != buffConfig)
                        {
                            foreach(var instanceInfo in buffInstances)
                            {
                                AttributeConfig buffAttrConfig = AttributeConfigProvider.Instance.GetAttributeConfig(buffConfig.AttrId);
                                if (null != buffAttrConfig)
                                    attrData.SetRelativeByConfig(buffAttrConfig);
                            }
                        }
                    }
                }
            }
        }

        public static float CalcDamage()
        {
            return 1.0f;
        }

        private GameContext m_Context;
        private IGroup<GameEntity> m_AttrGroup;
    }
}
