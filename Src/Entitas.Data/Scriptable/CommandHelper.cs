using System;
using System.Collections.Generic;

namespace ScriptableData
{
  /// <summary>
  /// 简单的函数值基类，简化实现IValue需要写的代码行数(当前值类只支持CallData样式)
  /// </summary>
  public abstract class SimpleValueBase<SubClassType, ValueParamType> : IValue<object>
    where SubClassType : SimpleValueBase<SubClassType, ValueParamType>, new()
    where ValueParamType : IValueParam, new()
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      m_Params.InitFromDsl(param, 0);
    }
    public IValue<object> Clone()
    {
      SubClassType val = new SubClassType();
      val.m_Params = m_Params.Clone();
      val.m_Result = m_Result.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_Params.Evaluate(iterator, args);
      TryUpdateValue(null);
    }
    public void Evaluate(IInstance instance)
    {
      m_Params.Evaluate(instance);
      TryUpdateValue(instance);
    }
    public void Analyze(IInstance instance)
    {
      m_Params.Analyze(instance);
    }
    public bool HaveValue
    {
      get
      {
        return m_Result.HaveValue;
      }
    }
    public object Value
    {
      get
      {
        return m_Result.Value;
      }
    }
    protected abstract void UpdateValue(IInstance instance, ValueParamType _params, ValueResult result);

    private void TryUpdateValue(IInstance instance)
    {
      if (m_Params.HaveValue) {
        UpdateValue(instance, (ValueParamType)m_Params, m_Result);
      }
    }

    private IValueParam m_Params = new ValueParamType();
    private ValueResult m_Result = new ValueResult();
  }
  /// <summary>
  /// 简单的命令基类，简化实现IValue需要写的代码行数（通常这样的命令是一个CallData样式的命令）
  /// </summary>
  public abstract class SimpleSkillCommandBase<SubClassType, ValueParamType> : ICommand
    where SubClassType : SimpleSkillCommandBase<SubClassType, ValueParamType>, new()
    where ValueParamType : IValueParam, new()
  {
    public void Init(ScriptableData.ISyntaxComponent config)
    {
      m_Params.InitFromDsl(config, 0);
    }
    public ICommand Clone()
    {
      SubClassType cmd = new SubClassType();
      cmd.m_Params = m_Params.Clone();
      return cmd;
    }
    public void Reset()
    {
      m_LastExecResult = ExecResult.Unknown;
      ResetState();
    }
    public void Prepare(IInstance instance, object iterator, object[] args)
    {
      m_Params.Evaluate(iterator, args);
    }
    public ExecResult Execute(IInstance instance, long delta)
    {
      if (m_LastExecResult == ExecResult.Unknown) {
        //重复执行时不需要每个tick都更新变量值，每个命令每次执行，变量值只读取一次。
        m_Params.Evaluate(instance);
      }
      m_LastExecResult = ExecCommand(instance, (ValueParamType)m_Params, delta);
      return m_LastExecResult;
    }
    public void Analyze(IInstance instance)
    {
      SemanticAnalyze(instance);
    }

    protected virtual void ResetState() { }
    protected virtual ExecResult ExecCommand(IInstance instance, ValueParamType _params, long delta)
    {
      return ExecResult.Finished;
    }
    protected virtual void SemanticAnalyze(IInstance instance) { }

    private ExecResult m_LastExecResult = ExecResult.Unknown;    
    private IValueParam m_Params = new ValueParamType();
  }
}
