using System;
using System.Collections.Generic;
using Entitas;
using Util;

namespace UnityClient
{
    public enum IdEnum
    {
        Entity = 0,
        Resource = 1,
        Max = 2,
    }
    public class IdSystem : Singleton<IdSystem>, IInitializeSystem
    {
        public IdSystem()
        {
            m_Generators = new Dictionary<IdEnum, IdGenerator>();
        }
        public void Initialize()
        {
            for(int i = 0; i < (int)IdEnum.Max; ++i)
                m_Generators.Add((IdEnum)i, new IdGenerator());
        }

        public uint GenId(IdEnum idType)
        {
            IdGenerator generator;
            if(m_Generators.TryGetValue(idType, out generator))
            {
                return generator.NextId();
            }

            LogUtil.Error("IdSystem.GenId generator for {0} not found!", idType);
            return 0;
        }

        private  readonly Dictionary<IdEnum, IdGenerator> m_Generators;
    }
}
