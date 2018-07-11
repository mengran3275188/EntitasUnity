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
            m_AttrGroup = m_Context.GetGroup(GameMatcher.Attr);
        }
        public void Execute()
        {
            var entities = m_AttrGroup.GetEntities();
            foreach(var entity in entities)
            {
                AttributeData attrData = entity.attr.Value;
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
