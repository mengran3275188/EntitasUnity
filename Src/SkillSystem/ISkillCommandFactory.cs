using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill
{
    public interface ISkillCommandFactory
    {
        ISkillCommand Create(ScriptableData.ISyntaxComponent commandConfig);
    }
    public class SkillCommandFactoryHelper<T> : ISkillCommandFactory where T : ISkillCommand, new()
    {
        public ISkillCommand Create(ScriptableData.ISyntaxComponent commandConfig)
        {
            T t = new T();
            t.Init(commandConfig);
            return t;
        }
    }
}
