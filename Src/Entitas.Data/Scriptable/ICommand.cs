using System;
using System.Collections.Generic;

namespace ScriptableData
{
    public enum ExecResult
    {
        Finished,
        Blocked,
        Parallel,
        Unknown,
    }
    public interface ICommand
    {
        void Init(ScriptableData.ISyntaxComponent config);
        ICommand Clone();
        void Reset();
        void Prepare(IInstance instance, object iterator, object[] args);
        ExecResult Execute(IInstance instance, long delta);
        void Analyze(IInstance instance);
    }
    public abstract class AbstractCommand : ICommand
    {
        public bool IsCompositeCommand
        {
            get { return m_IscompositeCommand; }
            protected set { m_IscompositeCommand = value; }
        }
        public void Init(ScriptableData.ISyntaxComponent config)
        {
            ScriptableData.CallData callData = config as ScriptableData.CallData;
            if(null != callData)
            {
                Load(callData);
            }
            else
            {
                ScriptableData.FunctionData funcData = config as ScriptableData.FunctionData;
                if(null != funcData)
                {
                    Load(funcData);
                }
                else
                {
                    ScriptableData.StatementData statementData = config as ScriptableData.StatementData;
                    if(null != statementData)
                    {
                        Load(statementData);
                    }
                    else
                    {
                        //error
                    }
                }
            }
        }
        public void Reset()
        {
            ResetState();
        }
        public void Prepare(IInstance instance, object iterator, object[] args)
        {
            UpdateArguments(iterator, args);
        }
        public ExecResult Execute(IInstance instance, long delta)
        {
            if(m_LastExecResult != ExecResult.Finished  || m_IscompositeCommand)
            {
                // 重复执行时不需要的每个tick都更新变量值，每个命令执行一次，变量值只读取一次。
                UpdateVariables(instance);
            }
            m_LastExecResult = ExecCommand(instance, delta);
            return m_LastExecResult;
        }
        public void Analyze(IInstance instance)
        {
            SemanticAnalyze(instance);
        }
        public virtual ICommand Clone()
        {
            return this.MemberwiseClone() as ICommand;
        }
        protected virtual void ResetState() { }
        protected virtual void UpdateArguments(object iterator, object[] args) { }
        protected virtual void UpdateVariables(IInstance instance) { }
        protected virtual ExecResult ExecCommand(IInstance instance, long delta)
        {
            return ExecResult.Finished;
        }
        protected virtual void SemanticAnalyze(IInstance instance) { }
        protected virtual void Load(ScriptableData.CallData callData) { }
        protected virtual void Load(ScriptableData.FunctionData funcData) { }
        protected virtual void Load(ScriptableData.StatementData statementData) { }

        private ExecResult m_LastExecResult = ExecResult.Unknown;
        private bool m_IscompositeCommand = false;
    }
}
