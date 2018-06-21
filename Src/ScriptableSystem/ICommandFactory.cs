using System;
using System.Collections.Generic;

namespace ScriptableSystem
{
    public interface ICommandFactory
    {
        ICommand Create(ScriptableData.ISyntaxComponent commandConfig);
    }
    public class CommandFactoryHelper<T> : ICommandFactory where T : ICommand, new()
    {
        public ICommand Create(ScriptableData.ISyntaxComponent commandConfig)
        {
            T t = new T();
            t.Init(commandConfig);
            return t;
        }
    }
}
