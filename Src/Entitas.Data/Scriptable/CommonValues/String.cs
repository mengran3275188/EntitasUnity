using System;
using System.Collections;
using System.Collections.Generic;

namespace ScriptableData.CommonValues
{
  internal sealed class FormatValue : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "format") {
        int num = callData.GetParamNum();
        if (num > 0) {
          m_Format.InitFromDsl(callData.GetParam(0));
        }
        for (int i = 1; i < callData.GetParamNum(); ++i) {
          SkillValue val = new SkillValue();
          val.InitFromDsl(callData.GetParam(i));
          m_FormatArgs.Add(val);
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      FormatValue val = new FormatValue();
      val.m_Format = m_Format.Clone();
      for (int i = 0; i < m_FormatArgs.Count; i++)
      {
        val.m_FormatArgs.Add(m_FormatArgs[i].Clone());
      }
      /*
      foreach (SkillValue v in m_FormatArgs) {
        val.m_FormatArgs.Add(v.Clone());
      }*/
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_Format.Evaluate(iterator, args);
      for (int i = 0; i < m_FormatArgs.Count; i++)
      {
        m_FormatArgs[i].Evaluate(iterator, args);
      }
      /*
      foreach (SkillValue val in m_FormatArgs) {
        val.Evaluate(iterator, args);
      }*/
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      m_Format.Evaluate(instance);
      for (int i = 0; i < m_FormatArgs.Count; i++)
      {
        m_FormatArgs[i].Evaluate(instance);
      }
      /*
      foreach (SkillValue val in m_FormatArgs) {
        val.Evaluate(instance);
      }*/
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      m_Format.Analyze(instance);
      for (int i = 0; i < m_FormatArgs.Count; i++)
      {
        m_FormatArgs[i].Analyze(instance);
      }
      /*
      foreach (SkillValue val in m_FormatArgs) {
        val.Analyze(instance);
      }*/
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
      bool canCalc = true;
      if (!m_Format.HaveValue) {
        canCalc = false;
      }
      else
      {
        for (int i = 0; i < m_FormatArgs.Count; i++)
        {
          if (!m_FormatArgs[i].HaveValue)
          {
            canCalc = false;
            break;
          }
        }
        /*
        foreach (SkillValue val in m_FormatArgs) {
          if (!val.HaveValue) {
            canCalc = false;
            break;
          }
        }*/
      }
      if (canCalc) {
        m_HaveValue = true;
        string format = m_Format.Value;
        ArrayList arglist = new ArrayList();
        for (int i = 0; i < m_FormatArgs.Count; i++)
        {
          arglist.Add(m_FormatArgs[i].Value);
        }
        /*
        foreach (SkillValue val in m_FormatArgs) {
          arglist.Add(val.Value);
        }*/
        object[] args = arglist.ToArray();
        m_Value = string.Format(format, args);
      }
    }

    private IValue<string> m_Format = new SkillValue<string>();
    private List<IValue<object>> m_FormatArgs = new List<IValue<object>>();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class SubstringValue : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "substring" && callData.GetParamNum() > 0) {
        m_ParamNum = callData.GetParamNum();
        m_Start.InitFromDsl(callData.GetParam(0));
        if (m_ParamNum > 1)
          m_Start.InitFromDsl(callData.GetParam(1));
        if (m_ParamNum > 2)
          m_Length.InitFromDsl(callData.GetParam(2));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      SubstringValue val = new SubstringValue();
      val.m_ParamNum = m_ParamNum;
      val.m_String = m_String.Clone();
      val.m_Start = m_Start.Clone();
      val.m_Length = m_Length.Clone();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_String.Evaluate(iterator, args);
      if (m_ParamNum > 1)
        m_Start.Evaluate(iterator, args);
      if (m_ParamNum > 2)
        m_Length.Evaluate(iterator, args);
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      m_String.Evaluate(instance);
      if (m_ParamNum > 1)
        m_Start.Evaluate(instance);
      if (m_ParamNum > 2)
        m_Length.Evaluate(instance);
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      if (m_ParamNum > 1)
        m_Start.Analyze(instance);
      if (m_ParamNum > 2)
        m_Length.Analyze(instance);
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
      if (m_String.HaveValue) {
        bool canCalc = true;
        string str = m_String.Value;
        int start = 0;
        int len = 0;
        if (m_ParamNum == 1 && m_String.HaveValue) {
          len = str.Length;
        }
        if (m_ParamNum == 2 && !m_Start.HaveValue) {
          canCalc = false;
        } else {
          start = m_Start.Value;
          len = str.Length - start;
        }
        if (m_ParamNum == 3 && (!m_Start.HaveValue || !m_Length.HaveValue)) {
          canCalc = false;
        } else {
          start = m_Start.Value;
          len = m_Length.Value;
        }
        if (canCalc) {
          m_HaveValue = true;
          m_Value = str.Substring(start, len);
        }
      }
    }

    private int m_ParamNum = 0;
    private IValue<string> m_String = new SkillValue<string>();
    private IValue<int> m_Start = new SkillValue<int>();
    private IValue<int> m_Length = new SkillValue<int>();
    private bool m_HaveValue;
    private object m_Value;
  }
}
