using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Skill
{
    public sealed class SkillCommandManager : Singleton<SkillCommandManager>
    {

        public void RegisterCommandFactory(string type, ISkillCommandFactory factory)
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
        public ISkillCommand CreateCommand(ScriptableData.ISyntaxComponent commandConfig)
        {
            ISkillCommand command = null;
            string type = commandConfig.GetId();
            ISkillCommandFactory factory = GetFactory(type);
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
        public SkillCommandManager()
        {
            RegisterCommandFactory("=", new SkillCommandFactoryHelper<CommonCommands.AssignCommand>());
            RegisterCommandFactory("assign", new SkillCommandFactoryHelper<CommonCommands.AssignCommand>());
            RegisterCommandFactory("inc", new SkillCommandFactoryHelper<CommonCommands.IncCommand>());
            RegisterCommandFactory("dec", new SkillCommandFactoryHelper<CommonCommands.DecCommand>());
            RegisterCommandFactory("propset", new SkillCommandFactoryHelper<CommonCommands.PropSetCommand>());
            RegisterCommandFactory("foreach", new SkillCommandFactoryHelper<CommonCommands.ForeachCommand>());
            RegisterCommandFactory("looplist", new SkillCommandFactoryHelper<CommonCommands.LoopListCommand>());
            RegisterCommandFactory("loop", new SkillCommandFactoryHelper<CommonCommands.LoopCommand>());
            RegisterCommandFactory("wait", new SkillCommandFactoryHelper<CommonCommands.SleepCommand>());
            RegisterCommandFactory("sleep", new SkillCommandFactoryHelper<CommonCommands.SleepCommand>());
            RegisterCommandFactory("terminate", new SkillCommandFactoryHelper<CommonCommands.TerminateCommand>());
            RegisterCommandFactory("localmessage", new SkillCommandFactoryHelper<CommonCommands.LocalMessageCommand>());
            RegisterCommandFactory("clearmessage", new SkillCommandFactoryHelper<CommonCommands.ClearMessageCommand>());
            RegisterCommandFactory("while", new SkillCommandFactoryHelper<CommonCommands.WhileCommand>());
            RegisterCommandFactory("if", new SkillCommandFactoryHelper<CommonCommands.IfElseCommand>());
            RegisterCommandFactory("log", new SkillCommandFactoryHelper<CommonCommands.LogCommand>());
            RegisterCommandFactory("listset", new SkillCommandFactoryHelper<CommonCommands.ListSetCommand>());

            //注册通用值与内部函数
            //object
            SkillValueManager.Instance.RegisterValueHandler("propget", new SkillValueFactoryHelper<CommonValues.PropGetValue>());
            SkillValueManager.Instance.RegisterValueHandler("rndint", new SkillValueFactoryHelper<CommonValues.RandomIntValue>());
            SkillValueManager.Instance.RegisterValueHandler("rndfloat", new SkillValueFactoryHelper<CommonValues.RandomFloatValue>());
            SkillValueManager.Instance.RegisterValueHandler("vector2", new SkillValueFactoryHelper<CommonValues.Vector2Value>());
            SkillValueManager.Instance.RegisterValueHandler("vector3", new SkillValueFactoryHelper<CommonValues.Vector3Value>());
            SkillValueManager.Instance.RegisterValueHandler("vector4", new SkillValueFactoryHelper<CommonValues.Vector4Value>());
            SkillValueManager.Instance.RegisterValueHandler("quaternion", new SkillValueFactoryHelper<CommonValues.QuaternionValue>());
            SkillValueManager.Instance.RegisterValueHandler("eular", new SkillValueFactoryHelper<CommonValues.EularValue>());
            SkillValueManager.Instance.RegisterValueHandler("stringlist", new SkillValueFactoryHelper<CommonValues.StringListValue>());
            SkillValueManager.Instance.RegisterValueHandler("intlist", new SkillValueFactoryHelper<CommonValues.IntListValue>());
            SkillValueManager.Instance.RegisterValueHandler("floatlist", new SkillValueFactoryHelper<CommonValues.FloatListValue>());
            SkillValueManager.Instance.RegisterValueHandler("vector2list", new SkillValueFactoryHelper<CommonValues.Vector2ListValue>());
            SkillValueManager.Instance.RegisterValueHandler("vector3list", new SkillValueFactoryHelper<CommonValues.Vector3ListValue>());
            SkillValueManager.Instance.RegisterValueHandler("list", new SkillValueFactoryHelper<CommonValues.ListValue>());
            SkillValueManager.Instance.RegisterValueHandler("rndfromlist", new SkillValueFactoryHelper<CommonValues.RandomFromListValue>());
            SkillValueManager.Instance.RegisterValueHandler("listget", new SkillValueFactoryHelper<CommonValues.ListGetValue>());
            SkillValueManager.Instance.RegisterValueHandler("listsize", new SkillValueFactoryHelper<CommonValues.ListSizeValue>());
            SkillValueManager.Instance.RegisterValueHandler("vector2dist", new SkillValueFactoryHelper<CommonValues.Vector2DistanceValue>());
            SkillValueManager.Instance.RegisterValueHandler("vector3dist", new SkillValueFactoryHelper<CommonValues.Vector3DistanceValue>());
            SkillValueManager.Instance.RegisterValueHandler("vector2to3", new SkillValueFactoryHelper<CommonValues.Vector2To3Value>());
            SkillValueManager.Instance.RegisterValueHandler("vector3to2", new SkillValueFactoryHelper<CommonValues.Vector3To2Value>());
            SkillValueManager.Instance.RegisterValueHandler("+", new SkillValueFactoryHelper<CommonValues.AddOperator>());
            SkillValueManager.Instance.RegisterValueHandler("-", new SkillValueFactoryHelper<CommonValues.SubOperator>());
            SkillValueManager.Instance.RegisterValueHandler("*", new SkillValueFactoryHelper<CommonValues.MulOperator>());
            SkillValueManager.Instance.RegisterValueHandler("/", new SkillValueFactoryHelper<CommonValues.DivOperator>());
            SkillValueManager.Instance.RegisterValueHandler("%", new SkillValueFactoryHelper<CommonValues.ModOperator>());
            SkillValueManager.Instance.RegisterValueHandler("abs", new SkillValueFactoryHelper<CommonValues.AbsOperator>());
            SkillValueManager.Instance.RegisterValueHandler("floor", new SkillValueFactoryHelper<CommonValues.FloorOperator>());
            SkillValueManager.Instance.RegisterValueHandler("ceiling", new SkillValueFactoryHelper<CommonValues.CeilingOperator>());
            SkillValueManager.Instance.RegisterValueHandler("round", new SkillValueFactoryHelper<CommonValues.RoundOperator>());
            SkillValueManager.Instance.RegisterValueHandler("pow", new SkillValueFactoryHelper<CommonValues.PowOperator>());
            SkillValueManager.Instance.RegisterValueHandler("log", new SkillValueFactoryHelper<CommonValues.LogOperator>());
            SkillValueManager.Instance.RegisterValueHandler("sqrt", new SkillValueFactoryHelper<CommonValues.SqrtOperator>());
            SkillValueManager.Instance.RegisterValueHandler("sin", new SkillValueFactoryHelper<CommonValues.SinOperator>());
            SkillValueManager.Instance.RegisterValueHandler("cos", new SkillValueFactoryHelper<CommonValues.CosOperator>());
            SkillValueManager.Instance.RegisterValueHandler("sinh", new SkillValueFactoryHelper<CommonValues.SinhOperator>());
            SkillValueManager.Instance.RegisterValueHandler("cosh", new SkillValueFactoryHelper<CommonValues.CoshOperator>());
            SkillValueManager.Instance.RegisterValueHandler("min", new SkillValueFactoryHelper<CommonValues.MinOperator>());
            SkillValueManager.Instance.RegisterValueHandler("max", new SkillValueFactoryHelper<CommonValues.MaxOperator>());
            SkillValueManager.Instance.RegisterValueHandler(">", new SkillValueFactoryHelper<CommonValues.GreaterThanOperator>());
            SkillValueManager.Instance.RegisterValueHandler(">=", new SkillValueFactoryHelper<CommonValues.GreaterEqualThanOperator>());
            SkillValueManager.Instance.RegisterValueHandler("==", new SkillValueFactoryHelper<CommonValues.EqualOperator>());
            SkillValueManager.Instance.RegisterValueHandler("!=", new SkillValueFactoryHelper<CommonValues.NotEqualOperator>());
            SkillValueManager.Instance.RegisterValueHandler("<", new SkillValueFactoryHelper<CommonValues.LessThanOperator>());
            SkillValueManager.Instance.RegisterValueHandler("<=", new SkillValueFactoryHelper<CommonValues.LessEqualThanOperator>());
            SkillValueManager.Instance.RegisterValueHandler("&&", new SkillValueFactoryHelper<CommonValues.AndOperator>());
            SkillValueManager.Instance.RegisterValueHandler("||", new SkillValueFactoryHelper<CommonValues.OrOperator>());
            SkillValueManager.Instance.RegisterValueHandler("!", new SkillValueFactoryHelper<CommonValues.NotOperator>());
            SkillValueManager.Instance.RegisterValueHandler("format", new SkillValueFactoryHelper<CommonValues.FormatValue>());
            SkillValueManager.Instance.RegisterValueHandler("substring", new SkillValueFactoryHelper<CommonValues.SubstringValue>());
            SkillValueManager.Instance.RegisterValueHandler("time", new SkillValueFactoryHelper<CommonValues.TimeValue>());
            SkillValueManager.Instance.RegisterValueHandler("isnull", new SkillValueFactoryHelper<CommonValues.IsNullOperator>());
        }

        private ISkillCommandFactory GetFactory(string type)
        {
            ISkillCommandFactory factory;
            m_StoryCommandFactories.TryGetValue(type, out factory);
            return factory;
        }

        private Dictionary<string, ISkillCommandFactory> m_StoryCommandFactories = new Dictionary<string, ISkillCommandFactory>();
    }
}
