using System;
using System.Collections.Generic;
using Util;

namespace ScriptableData
{
    public sealed class CommandManager : Singleton<CommandManager>
    {
        public void RegisterCommandFactory(string type, ICommandFactory factory)
        {
            if (!m_StoryCommandFactories.ContainsKey(type))
            {
                m_StoryCommandFactories.Add(type, factory);
            }
            else
            {
                // error
            }
        }
        public ICommand CreateCommand(ScriptableData.ISyntaxComponent commandConfig)
        {
            ICommand command = null;
            string type = commandConfig.GetId();
            ICommandFactory factory = GetFactory(type);
            if (null != factory)
            {
                command = factory.Create(commandConfig);
            }
            else
            {
                LogUtil.Debug("CreateCommand failed, unknown type:{0}.", type);
            }
            return command;
        }
        public ICommand CreateCommand(string type)
        {
            ICommand command = null;
            ICommandFactory factory = GetFactory(type);
            if (null != factory)
            {
                command = factory.Create();
            }
            else
            {
                LogUtil.Debug("CreateCommand failed, unknown type:{0}.", type);
            }
            return command;
        }
        public CommandManager()
        {
            RegisterCommandFactory("=", new CommandFactoryHelper<CommonCommands.AssignCommand>());
            RegisterCommandFactory("assign", new CommandFactoryHelper<CommonCommands.AssignCommand>());
            RegisterCommandFactory("inc", new CommandFactoryHelper<CommonCommands.IncCommand>());
            RegisterCommandFactory("dec", new CommandFactoryHelper<CommonCommands.DecCommand>());
            RegisterCommandFactory("propset", new CommandFactoryHelper<CommonCommands.PropSetCommand>());
            RegisterCommandFactory("foreach", new CommandFactoryHelper<CommonCommands.ForeachCommand>());
            RegisterCommandFactory("looplist", new CommandFactoryHelper<CommonCommands.LoopListCommand>());
            RegisterCommandFactory("loop", new CommandFactoryHelper<CommonCommands.LoopCommand>());
            RegisterCommandFactory("wait", new CommandFactoryHelper<CommonCommands.SleepCommand>());
            RegisterCommandFactory("sleep", new CommandFactoryHelper<CommonCommands.SleepCommand>());
            RegisterCommandFactory("terminate", new CommandFactoryHelper<CommonCommands.TerminateCommand>());
            RegisterCommandFactory("localmessage", new CommandFactoryHelper<CommonCommands.LocalMessageCommand>());
            RegisterCommandFactory("clearmessage", new CommandFactoryHelper<CommonCommands.ClearMessageCommand>());
            RegisterCommandFactory("while", new CommandFactoryHelper<CommonCommands.WhileCommand>());
            RegisterCommandFactory("if", new CommandFactoryHelper<CommonCommands.IfElseCommand>());
            RegisterCommandFactory("log", new CommandFactoryHelper<CommonCommands.LogCommand>());
            RegisterCommandFactory("listset", new CommandFactoryHelper<CommonCommands.ListSetCommand>());

            //注册通用值与内部函数
            //object
            ValueManager.Instance.RegisterValueHandler("propget", new ValueFactoryHelper<CommonValues.PropGetValue>());
            ValueManager.Instance.RegisterValueHandler("rndint", new ValueFactoryHelper<CommonValues.RandomIntValue>());
            ValueManager.Instance.RegisterValueHandler("rndfloat", new ValueFactoryHelper<CommonValues.RandomFloatValue>());
            ValueManager.Instance.RegisterValueHandler("vector2", new ValueFactoryHelper<CommonValues.Vector2Value>());
            ValueManager.Instance.RegisterValueHandler("vector3", new ValueFactoryHelper<CommonValues.Vector3Value>());
            ValueManager.Instance.RegisterValueHandler("vector4", new ValueFactoryHelper<CommonValues.Vector4Value>());
            ValueManager.Instance.RegisterValueHandler("quaternion", new ValueFactoryHelper<CommonValues.QuaternionValue>());
            ValueManager.Instance.RegisterValueHandler("eular", new ValueFactoryHelper<CommonValues.EularValue>());
            ValueManager.Instance.RegisterValueHandler("stringlist", new ValueFactoryHelper<CommonValues.StringListValue>());
            ValueManager.Instance.RegisterValueHandler("intlist", new ValueFactoryHelper<CommonValues.IntListValue>());
            ValueManager.Instance.RegisterValueHandler("floatlist", new ValueFactoryHelper<CommonValues.FloatListValue>());
            ValueManager.Instance.RegisterValueHandler("vector2list", new ValueFactoryHelper<CommonValues.Vector2ListValue>());
            ValueManager.Instance.RegisterValueHandler("vector3list", new ValueFactoryHelper<CommonValues.Vector3ListValue>());
            ValueManager.Instance.RegisterValueHandler("list", new ValueFactoryHelper<CommonValues.ListValue>());
            ValueManager.Instance.RegisterValueHandler("rndfromlist", new ValueFactoryHelper<CommonValues.RandomFromListValue>());
            ValueManager.Instance.RegisterValueHandler("listget", new ValueFactoryHelper<CommonValues.ListGetValue>());
            ValueManager.Instance.RegisterValueHandler("listsize", new ValueFactoryHelper<CommonValues.ListSizeValue>());
            ValueManager.Instance.RegisterValueHandler("vector2dist", new ValueFactoryHelper<CommonValues.Vector2DistanceValue>());
            ValueManager.Instance.RegisterValueHandler("vector3dist", new ValueFactoryHelper<CommonValues.Vector3DistanceValue>());
            ValueManager.Instance.RegisterValueHandler("vector2to3", new ValueFactoryHelper<CommonValues.Vector2To3Value>());
            ValueManager.Instance.RegisterValueHandler("vector3to2", new ValueFactoryHelper<CommonValues.Vector3To2Value>());
            ValueManager.Instance.RegisterValueHandler("+", new ValueFactoryHelper<CommonValues.AddOperator>());
            ValueManager.Instance.RegisterValueHandler("-", new ValueFactoryHelper<CommonValues.SubOperator>());
            ValueManager.Instance.RegisterValueHandler("*", new ValueFactoryHelper<CommonValues.MulOperator>());
            ValueManager.Instance.RegisterValueHandler("/", new ValueFactoryHelper<CommonValues.DivOperator>());
            ValueManager.Instance.RegisterValueHandler("%", new ValueFactoryHelper<CommonValues.ModOperator>());
            ValueManager.Instance.RegisterValueHandler("abs", new ValueFactoryHelper<CommonValues.AbsOperator>());
            ValueManager.Instance.RegisterValueHandler("floor", new ValueFactoryHelper<CommonValues.FloorOperator>());
            ValueManager.Instance.RegisterValueHandler("ceiling", new ValueFactoryHelper<CommonValues.CeilingOperator>());
            ValueManager.Instance.RegisterValueHandler("round", new ValueFactoryHelper<CommonValues.RoundOperator>());
            ValueManager.Instance.RegisterValueHandler("pow", new ValueFactoryHelper<CommonValues.PowOperator>());
            ValueManager.Instance.RegisterValueHandler("log", new ValueFactoryHelper<CommonValues.LogOperator>());
            ValueManager.Instance.RegisterValueHandler("sqrt", new ValueFactoryHelper<CommonValues.SqrtOperator>());
            ValueManager.Instance.RegisterValueHandler("sin", new ValueFactoryHelper<CommonValues.SinOperator>());
            ValueManager.Instance.RegisterValueHandler("cos", new ValueFactoryHelper<CommonValues.CosOperator>());
            ValueManager.Instance.RegisterValueHandler("sinh", new ValueFactoryHelper<CommonValues.SinhOperator>());
            ValueManager.Instance.RegisterValueHandler("cosh", new ValueFactoryHelper<CommonValues.CoshOperator>());
            ValueManager.Instance.RegisterValueHandler("min", new ValueFactoryHelper<CommonValues.MinOperator>());
            ValueManager.Instance.RegisterValueHandler("max", new ValueFactoryHelper<CommonValues.MaxOperator>());
            ValueManager.Instance.RegisterValueHandler(">", new ValueFactoryHelper<CommonValues.GreaterThanOperator>());
            ValueManager.Instance.RegisterValueHandler(">=", new ValueFactoryHelper<CommonValues.GreaterEqualThanOperator>());
            ValueManager.Instance.RegisterValueHandler("==", new ValueFactoryHelper<CommonValues.EqualOperator>());
            ValueManager.Instance.RegisterValueHandler("!=", new ValueFactoryHelper<CommonValues.NotEqualOperator>());
            ValueManager.Instance.RegisterValueHandler("<", new ValueFactoryHelper<CommonValues.LessThanOperator>());
            ValueManager.Instance.RegisterValueHandler("<=", new ValueFactoryHelper<CommonValues.LessEqualThanOperator>());
            ValueManager.Instance.RegisterValueHandler("&&", new ValueFactoryHelper<CommonValues.AndOperator>());
            ValueManager.Instance.RegisterValueHandler("||", new ValueFactoryHelper<CommonValues.OrOperator>());
            ValueManager.Instance.RegisterValueHandler("!", new ValueFactoryHelper<CommonValues.NotOperator>());
            ValueManager.Instance.RegisterValueHandler("format", new ValueFactoryHelper<CommonValues.FormatValue>());
            ValueManager.Instance.RegisterValueHandler("substring", new ValueFactoryHelper<CommonValues.SubstringValue>());
            ValueManager.Instance.RegisterValueHandler("time", new ValueFactoryHelper<CommonValues.TimeValue>());
            ValueManager.Instance.RegisterValueHandler("isnull", new ValueFactoryHelper<CommonValues.IsNullOperator>());
        }

        private ICommandFactory GetFactory(string type)
        {
            ICommandFactory factory;
            m_StoryCommandFactories.TryGetValue(type, out factory);
            return factory;
        }

        private Dictionary<string, ICommandFactory> m_StoryCommandFactories = new Dictionary<string, ICommandFactory>();
    }
}
