using System;
using System.Collections.Generic;

namespace ScriptableData.CommonValues
{
  internal sealed class AddOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "+") {
        if (callData.GetParamNum() == 1) {
          m_X.InitFromDsl(new ScriptableData.ValueData("0"));
          m_Y.InitFromDsl(callData.GetParam(0));
        } else if (callData.GetParamNum() == 2) {
          m_X.InitFromDsl(callData.GetParam(0));
          m_Y.InitFromDsl(callData.GetParam(1));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      AddOperator val = new AddOperator();
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
        object objX = m_X.Value;
        object objY = m_Y.Value;
        if (objX is string || objY is string) {
          string x = ValueHelper.CastTo<string>(objX);
          string y = ValueHelper.CastTo<string>(objY);
          m_Value = x + y;
        } else {
          if (objX is int && objY is int) {
            int x = ValueHelper.CastTo<int>(objX);
            int y = ValueHelper.CastTo<int>(objY);
            m_Value = x + y;
          } else {
            float x = ValueHelper.CastTo<float>(objX);
            float y = ValueHelper.CastTo<float>(objY);
            m_Value = x + y;
          }
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class SubOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "-") {
        if (callData.GetParamNum() == 1) {
          m_X.InitFromDsl(new ScriptableData.ValueData("0"));
          m_Y.InitFromDsl(callData.GetParam(0));
        } else if (callData.GetParamNum() == 2) {
          m_X.InitFromDsl(callData.GetParam(0));
          m_Y.InitFromDsl(callData.GetParam(1));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      SubOperator val = new SubOperator();
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
        object objX = m_X.Value;
        object objY = m_Y.Value;
        if (objX is int && objY is int) {
          int x = ValueHelper.CastTo<int>(objX);
          int y = ValueHelper.CastTo<int>(objY);
          m_Value = x - y;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          float y = ValueHelper.CastTo<float>(objY);
          m_Value = x - y;
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class MulOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "*" && callData.GetParamNum() == 2) {
        m_X.InitFromDsl(callData.GetParam(0));
        m_Y.InitFromDsl(callData.GetParam(1));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      MulOperator val = new MulOperator();
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
        object objX = m_X.Value;
        object objY = m_Y.Value;
        if (objX is int && objY is int) {
          int x = ValueHelper.CastTo<int>(objX);
          int y = ValueHelper.CastTo<int>(objY);
          m_Value = x * y;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          float y = ValueHelper.CastTo<float>(objY);
          m_Value = x * y;
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class DivOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "/" && callData.GetParamNum() == 2) {
        m_X.InitFromDsl(callData.GetParam(0));
        m_Y.InitFromDsl(callData.GetParam(1));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      DivOperator val = new DivOperator();
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
        object objX = m_X.Value;
        object objY = m_Y.Value;
        if (objX is int && objY is int) {
          int x = ValueHelper.CastTo<int>(objX);
          int y = ValueHelper.CastTo<int>(objY);
          m_Value = x / y;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          float y = ValueHelper.CastTo<float>(objY);
          m_Value = x / y;
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class ModOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "%" && callData.GetParamNum() == 2) {
        m_X.InitFromDsl(callData.GetParam(0));
        m_Y.InitFromDsl(callData.GetParam(1));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      ModOperator val = new ModOperator();
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
        object objX = m_X.Value;
        object objY = m_Y.Value;
        if (objX is int && objY is int) {
          int x = ValueHelper.CastTo<int>(objX);
          int y = ValueHelper.CastTo<int>(objY);
          m_Value = x % y;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          float y = ValueHelper.CastTo<float>(objY);
          m_Value = x % y;
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class AbsOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "abs") {
        if (callData.GetParamNum() == 1) {
          m_X.InitFromDsl(callData.GetParam(0));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      AbsOperator val = new AbsOperator();
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
        object objX = m_X.Value;
        if (objX is int) {
          int x = ValueHelper.CastTo<int>(objX);
          m_Value = Math.Abs(x);
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          m_Value = Math.Abs(x);
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class FloorOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "floor") {
        if (callData.GetParamNum() == 1) {
          m_X.InitFromDsl(callData.GetParam(0));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      FloorOperator val = new FloorOperator();
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
        object objX = m_X.Value;
        if (objX is int) {
          int x = ValueHelper.CastTo<int>(objX);
          m_Value = x;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          m_Value = (int)Math.Floor(x);
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class CeilingOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "ceiling") {
        if (callData.GetParamNum() == 1) {
          m_X.InitFromDsl(callData.GetParam(0));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      CeilingOperator val = new CeilingOperator();
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
        object objX = m_X.Value;
        if (objX is int) {
          int x = ValueHelper.CastTo<int>(objX);
          m_Value = x;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          m_Value = (int)Math.Ceiling(x);
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class RoundOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "round") {
        if (callData.GetParamNum() == 1) {
          m_X.InitFromDsl(callData.GetParam(0));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      RoundOperator val = new RoundOperator();
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
        object objX = m_X.Value;
        if (objX is int) {
          int x = ValueHelper.CastTo<int>(objX);
          m_Value = x;
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          m_Value = (int)Math.Round(x);
        }
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class PowOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "pow") {
        m_ParamNum = callData.GetParamNum();
        if (1 == m_ParamNum) {
          m_X.InitFromDsl(callData.GetParam(0));
        } else if (2 == m_ParamNum) {
          m_X.InitFromDsl(callData.GetParam(0));
          m_Y.InitFromDsl(callData.GetParam(1));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      PowOperator val = new PowOperator();
      val.m_X = m_X.Clone();
      val.m_Y = m_Y.Clone();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      if(m_ParamNum>0)
        m_X.Evaluate(iterator, args);
      if(m_ParamNum>1)
        m_Y.Evaluate(iterator, args);
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      if (m_ParamNum > 0)
        m_X.Evaluate(instance);
      if (m_ParamNum > 1)
        m_Y.Evaluate(instance);
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      if (m_ParamNum > 0)
        m_X.Analyze(instance);
      if (m_ParamNum > 1)
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
      if (1 == m_ParamNum && m_X.HaveValue) {
        m_HaveValue = true;
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Exp(x);
      } else if (2 == m_ParamNum && m_X.HaveValue && m_Y.HaveValue) {
        m_HaveValue = true;
        object objX = m_X.Value;
        object objY = m_Y.Value;
        if (objX is int && objY is int) {
          int x = ValueHelper.CastTo<int>(objX);
          int y = ValueHelper.CastTo<int>(objY);
          m_Value = (int)Math.Pow(x, y);
        } else {
          float x = ValueHelper.CastTo<float>(objX);
          float y = ValueHelper.CastTo<float>(objY);
          m_Value = (float)Math.Pow(x, y);
        }
      }
    }

    private int m_ParamNum = 0;
    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class LogOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "log") {
        m_ParamNum = callData.GetParamNum();
        if (1 == m_ParamNum) {
          m_X.InitFromDsl(callData.GetParam(0));
        } else if (2 == m_ParamNum) {
          m_X.InitFromDsl(callData.GetParam(0));
          m_Y.InitFromDsl(callData.GetParam(1));
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      LogOperator val = new LogOperator();
      val.m_X = m_X.Clone();
      val.m_Y = m_Y.Clone();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      if (m_ParamNum > 0)
        m_X.Evaluate(iterator, args);
      if (m_ParamNum > 1)
        m_Y.Evaluate(iterator, args);
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      if (m_ParamNum > 0)
        m_X.Evaluate(instance);
      if (m_ParamNum > 1)
        m_Y.Evaluate(instance);
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      if (m_ParamNum > 0)
        m_X.Analyze(instance);
      if (m_ParamNum > 1)
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
      if (1 == m_ParamNum && m_X.HaveValue) {
        m_HaveValue = true;
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Log(x);
      } else if (2 == m_ParamNum && m_X.HaveValue && m_Y.HaveValue) {
        m_HaveValue = true;
        object objX = m_X.Value;
        object objY = m_Y.Value;
        float x = ValueHelper.CastTo<float>(objX);
        float y = ValueHelper.CastTo<float>(objY);
        m_Value = (float)Math.Log(x, y);
      }
    }

    private int m_ParamNum = 0;
    private IValue<object> m_X = new SkillValue();
    private IValue<object> m_Y = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class SqrtOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "sqrt" && callData.GetParamNum() == 1) {
        m_X.InitFromDsl(callData.GetParam(0));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      SqrtOperator val = new SqrtOperator();
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
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Sqrt(x);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class SinOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "sin" && callData.GetParamNum() == 1) {
        m_X.InitFromDsl(callData.GetParam(0));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      SinOperator val = new SinOperator();
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
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Sin(x);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class CosOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "cos" && callData.GetParamNum() == 1) {
        m_X.InitFromDsl(callData.GetParam(0));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      CosOperator val = new CosOperator();
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
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Cos(x);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class SinhOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "sinh" && callData.GetParamNum() == 1) {
        m_X.InitFromDsl(callData.GetParam(0));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      SinhOperator val = new SinhOperator();
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
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Sinh(x);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class CoshOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "cosh" && callData.GetParamNum() == 1) {
        m_X.InitFromDsl(callData.GetParam(0));
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      CoshOperator val = new CoshOperator();
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
        object objX = m_X.Value;
        float x = ValueHelper.CastTo<float>(objX);
        m_Value = (float)Math.Cosh(x);
      }
    }

    private IValue<object> m_X = new SkillValue();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class MinOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "min") {
        for (int i = 0; i < callData.GetParamNum(); ++i) {
          ScriptableData.ISyntaxComponent arg = callData.GetParam(i);
          SkillValue val = new SkillValue();
          val.InitFromDsl(arg);
          m_List.Add(val);
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      MinOperator val = new MinOperator();
      for (int i = 0; i < m_List.Count; i++) {
        val.m_List.Add(m_List[i]);
      }
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      for (int i = 0; i < m_List.Count; i++) {
        m_List[i].Evaluate(iterator, args);
      }
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      for (int i = 0; i < m_List.Count; i++) {
        m_List[i].Evaluate(instance);
      }
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      for (int i = 0; i < m_List.Count; i++) {
        m_List[i].Analyze(instance);
      }
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
      bool isInt = true;
      bool canCalc = true;
      for (int i = 0; i < m_List.Count; i++) {
        if (m_List[i].HaveValue) {
          if (!(m_List[i].Value is int)) {
            isInt = false;
          }
        } else {
          canCalc = false;
          break;
        }
      }
      if (canCalc) {
        m_HaveValue = true;
        if (isInt) {
          int minV = int.MaxValue;
          for (int i = 0; i < m_List.Count; i++) {
            int v = ValueHelper.CastTo<int>(m_List[i].Value);
            if (minV > v)
              minV = v;
          }
          m_Value = minV;
        } else {
          float minV = float.MaxValue;
          for (int i = 0; i < m_List.Count; i++) {
            float v = ValueHelper.CastTo<float>(m_List[i].Value);
            if (minV > v)
              minV = v;
          }
          m_Value = minV;
        }
      }
    }

    private List<IValue<object>> m_List = new List<IValue<object>>();
    private bool m_HaveValue;
    private object m_Value;
  }
  internal sealed class MaxOperator : IValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetId() == "max") {
        for (int i = 0; i < callData.GetParamNum(); ++i) {
          ScriptableData.ISyntaxComponent arg = callData.GetParam(i);
          SkillValue val = new SkillValue();
          val.InitFromDsl(arg);
          m_List.Add(val);
        }
        TryUpdateValue();
      }
    }
    public IValue<object> Clone()
    {
      MaxOperator val = new MaxOperator();
      for (int i = 0; i < m_List.Count; i++) {
        val.m_List.Add(m_List[i]);
      }
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      for (int i = 0; i < m_List.Count; i++) {
        m_List[i].Evaluate(iterator, args);
      }
      TryUpdateValue();
    }
    public void Evaluate(IInstance instance)
    {
      for (int i = 0; i < m_List.Count; i++) {
        m_List[i].Evaluate(instance);
      }
      TryUpdateValue();
    }
    public void Analyze(IInstance instance)
    {
      for (int i = 0; i < m_List.Count; i++) {
        m_List[i].Analyze(instance);
      }
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
      bool isInt = true;
      bool canCalc = true;
      for (int i = 0; i < m_List.Count; i++) {
        if (m_List[i].HaveValue) {
          if (!(m_List[i].Value is int)) {
            isInt = false;
          }
        } else {
          canCalc = false;
          break;
        }
      }
      if (canCalc) {
        m_HaveValue = true;
        if (isInt) {
          int maxV = int.MinValue;
          for (int i = 0; i < m_List.Count; i++) {
            int v = ValueHelper.CastTo<int>(m_List[i].Value);
            if (maxV < v)
              maxV = v;
          }
          m_Value = maxV;
        } else {
          float maxV = float.MinValue;
          for (int i = 0; i < m_List.Count; i++) {
            float v = ValueHelper.CastTo<float>(m_List[i].Value);
            if (maxV < v)
              maxV = v;
          }
          m_Value = maxV;
        }
      }
    }

    private List<IValue<object>> m_List = new List<IValue<object>>();
    private bool m_HaveValue;
    private object m_Value;
  }
}
