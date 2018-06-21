using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill
{
  /// <summary>
  /// 简单的函数值基类，简化实现ISkillValue需要写的代码行数(当前值类只支持CallData样式)
  /// </summary>
  public abstract class SimpleSkillValueBase<SubClassType, ValueParamType> : ISkillValue<object>
    where SubClassType : SimpleSkillValueBase<SubClassType, ValueParamType>, new()
    where ValueParamType : ISkillValueParam, new()
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      m_Params.InitFromDsl(param, 0);
    }
    public ISkillValue<object> Clone()
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
    public void Evaluate(SkillInstance instance)
    {
      m_Params.Evaluate(instance);
      TryUpdateValue(instance);
    }
    public void Analyze(SkillInstance instance)
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
    protected abstract void UpdateValue(SkillInstance instance, ValueParamType _params, SkillValueResult result);

    private void TryUpdateValue(SkillInstance instance)
    {
      if (m_Params.HaveValue) {
        UpdateValue(instance, (ValueParamType)m_Params, m_Result);
      }
    }

    private ISkillValueParam m_Params = new ValueParamType();
    private SkillValueResult m_Result = new SkillValueResult();
  }
  /// <summary>
  /// 简单的命令基类，简化实现ISkillValue需要写的代码行数（通常这样的命令是一个CallData样式的命令）
  /// </summary>
  public abstract class SimpleSkillCommandBase<SubClassType, ValueParamType> : ISkillCommand
    where SubClassType : SimpleSkillCommandBase<SubClassType, ValueParamType>, new()
    where ValueParamType : ISkillValueParam, new()
  {
    public void Init(ScriptableData.ISyntaxComponent config)
    {
      m_Params.InitFromDsl(config, 0);
    }
    public ISkillCommand Clone()
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
    public void Prepare(SkillInstance instance, object iterator, object[] args)
    {
      m_Params.Evaluate(iterator, args);
    }
    public ExecResult Execute(SkillInstance instance, long delta)
    {
      if (m_LastExecResult == ExecResult.Unknown) {
        //重复执行时不需要每个tick都更新变量值，每个命令每次执行，变量值只读取一次。
        m_Params.Evaluate(instance);
      }
      m_LastExecResult = ExecCommand(instance, (ValueParamType)m_Params, delta);
      return m_LastExecResult;
    }
    public void Analyze(SkillInstance instance)
    {
      SemanticAnalyze(instance);
    }

    protected virtual void ResetState() { }
    protected virtual ExecResult ExecCommand(SkillInstance instance, ValueParamType _params, long delta)
    {
      return ExecResult.Finished;
    }
    protected virtual void SemanticAnalyze(SkillInstance instance) { }

    private ExecResult m_LastExecResult = ExecResult.Unknown;    
    private ISkillValueParam m_Params = new ValueParamType();
  }
}
