using System;
using System.Collections.Generic;

namespace ScriptableData
{
    public interface ICommandFactory
    {
        ICommand Create(ScriptableData.ISyntaxComponent commandConfig);
        ICommand Create();
    }
    public class CommandFactoryHelper<T> : ICommandFactory where T : ICommand, new()
    {
        public ICommand Create(ScriptableData.ISyntaxComponent commandConfig)
        {
            T t = new T();
            t.Init(commandConfig);
            return t;
        }
        public ICommand Create()
        {
            T t = new T();
            return t;
        }
    }
}
