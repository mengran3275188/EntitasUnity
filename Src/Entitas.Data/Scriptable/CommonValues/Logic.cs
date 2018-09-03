using System;
using System.Collections.Generic;

namespace ScriptableData.CommonValues
{
  internal sealed class AndOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "&&") {
        if (callData.GetParamNum() == 2) {
          m_X.InitFromDsl(callData.GetParam(0));
          m_Y.InitFromDsl(callData.GetParam(1));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      AndOperator val = new AndOperator();
      val.m_X = m_X.Clone();
      val.m_Y = m_Y.Clone();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_X.Evaluate(iterator, args);
      m_Y.Evaluate(iterator, args);
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      m_X.Evaluate(instance);
      m_Y.Evaluate(instance);
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      m_X.Analyze(instance);
      m_Y.Analyze(instance);
    }
    public bool HaveValue
    {
      get
      {
        return m_HaveValue;
      }
    }
    public object Value
    {
      get
      {
        return m_Value;
      }
    }

    private void TryUpdateValue()
    {
      if (m_X.HaveValue && m_Y.HaveValue) {
        m_HaveValue = true;
        int x = (int)Convert.ChangeType(m_X.Value, typeof(int));
        int y = (int)Convert.ChangeType(m_Y.Value, typeof(int));
        m_Value = ((x != 0 && y != 0) ? 1 : 0);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class OrOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "||") {
        if (callData.GetParamNum() == 2) {
          m_X.InitFromDsl(callData.GetParam(0));
          m_Y.InitFromDsl(callData.GetParam(1));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      OrOperator val = new OrOperator();
      val.m_X = m_X.Clone();
      val.m_Y = m_Y.Clone();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_X.Evaluate(iterator, args);
      m_Y.Evaluate(iterator, args);
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      m_X.Evaluate(instance);
      m_Y.Evaluate(instance);
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      m_X.Analyze(instance);
      m_Y.Analyze(instance);
    }
    public bool HaveValue
    {
      get
      {
        return m_HaveValue;
      }
    }
    public object Value
    {
      get
      {
        return m_Value;
      }
    }

    private void TryUpdateValue()
    {
      if (m_X.HaveValue && m_Y.HaveValue) {
        m_HaveValue = true;
        int x = (int)Convert.ChangeType(m_X.Value, typeof(int));
        int y = (int)Convert.ChangeType(m_Y.Value, typeof(int));
        m_Value = ((x != 0 || y != 0) ? 1 : 0);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class NotOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "!") {
        if (callData.GetParamNum() > 0) {
          m_X.InitFromDsl(callData.GetParam(0));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      NotOperator val = new NotOperator();
      val.m_X = m_X.Clone();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_X.Evaluate(iterator, args);
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      m_X.Evaluate(instance);
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      m_X.Analyze(instance);
    }
    public bool HaveValue
    {
      get
      {
        return m_HaveValue;
      }
    }
    public object Value
    {
      get
      {
        return m_Value;
      }
    }

    private void TryUpdateValue()
    {
      if (m_X.HaveValue) {
        m_HaveValue = true;
        int x = (int)Convert.ChangeType(m_X.Value, typeof(int));
        m_Value = (x == 0 ? 1 : 0);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
}
