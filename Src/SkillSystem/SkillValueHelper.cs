using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill
{
  public static class SkillValueHelper
  {
    public static T CastTo<T>(object obj)
    {
      if (obj is T) {
        return (T)obj;
      } else {
        try {
          return (T)Convert.ChangeType(obj, typeof(T));
        } catch {
          return default(T);
        }
      }
    }
    public static ISkillValue<object> AdaptFrom<T>(ISkillValue<T> original)
    {
      return new SkillValueAdapter<T>(original);
    }
  }
  public sealed class SkillValueResult
  {
    public SkillValueResult Clone()
    {
      SkillValueResult val = new SkillValueResult();
      val.m_HaveValue = m_HaveValue;
      val.m_Value = m_Value;
      return val;
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
      set
      {
        m_HaveValue = true;
        m_Value = value;
      }
    }

    private bool m_HaveValue;
    private object m_Value;
  }
  public interface ISkillValueParam
  {
    void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex);
    ISkillValueParam Clone();
    void Evaluate(object iterator, object[] args);
    void Evaluate(SkillInstance instance);
    void Analyze(SkillInstance instance);
    bool HaveValue { get; }
  }
  public sealed class SkillValueParam : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {}
    public ISkillValueParam Clone()
    {
      SkillValueParam val = new SkillValueParam();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {}
    public void Evaluate(SkillInstance instance)
    {}
    public void Analyze(SkillInstance instance)
    {}
    public bool HaveValue
    {
      get { return true; }
    }
  }
  public sealed class SkillValueParam<P1> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 1) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1> val = new SkillValueParam<P1>();
      val.m_P1 = m_P1.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
  }
  public sealed class SkillValueParam<P1, P2> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 2) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2> val = new SkillValueParam<P1, P2>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
  }
  public sealed class SkillValueParam<P1, P2, P3> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 3) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
        m_P3.InitFromDsl(callData.GetParam(startIndex+2));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3> val = new SkillValueParam<P1, P2, P3>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 4) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
        m_P3.InitFromDsl(callData.GetParam(startIndex+2));
        m_P4.InitFromDsl(callData.GetParam(startIndex+3));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4> val = new SkillValueParam<P1, P2, P3, P4>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 5) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
        m_P3.InitFromDsl(callData.GetParam(startIndex+2));
        m_P4.InitFromDsl(callData.GetParam(startIndex+3));
        m_P5.InitFromDsl(callData.GetParam(startIndex+4));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5> val = new SkillValueParam<P1, P2, P3, P4, P5>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 6) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
        m_P3.InitFromDsl(callData.GetParam(startIndex+2));
        m_P4.InitFromDsl(callData.GetParam(startIndex+3));
        m_P5.InitFromDsl(callData.GetParam(startIndex+4));
        m_P6.InitFromDsl(callData.GetParam(startIndex+5));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6> val = new SkillValueParam<P1, P2, P3, P4, P5, P6>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 7) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
        m_P3.InitFromDsl(callData.GetParam(startIndex+2));
        m_P4.InitFromDsl(callData.GetParam(startIndex+3));
        m_P5.InitFromDsl(callData.GetParam(startIndex+4));
        m_P6.InitFromDsl(callData.GetParam(startIndex+5));
        m_P7.InitFromDsl(callData.GetParam(startIndex+6));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 8) {
        m_P1.InitFromDsl(callData.GetParam(startIndex+0));
        m_P2.InitFromDsl(callData.GetParam(startIndex+1));
        m_P3.InitFromDsl(callData.GetParam(startIndex+2));
        m_P4.InitFromDsl(callData.GetParam(startIndex+3));
        m_P5.InitFromDsl(callData.GetParam(startIndex+4));
        m_P6.InitFromDsl(callData.GetParam(startIndex+5));
        m_P7.InitFromDsl(callData.GetParam(startIndex+6));
        m_P8.InitFromDsl(callData.GetParam(startIndex+7));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
  }
  /*
  //更多参数的版本在ios上aot编译失败（内部错误，原因不明），先注掉。实际需要超过8个参数的SkillValue或StoryCommand时，直接实现ISkillValue接口与继承AbstractSkillCommand。
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 9) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 10) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 11) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
        m_P11.InitFromDsl(callData.GetParam(startIndex + 10));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      val.m_P11 = m_P11.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
      m_P11.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
      m_P11.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
      m_P11.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue && m_P11.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }
    public P11 Param11Value
    {
      get { return m_P11.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
    private ISkillValue<P11> m_P11 = new SkillValue<P11>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 12) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
        m_P11.InitFromDsl(callData.GetParam(startIndex + 10));
        m_P12.InitFromDsl(callData.GetParam(startIndex + 11));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      val.m_P11 = m_P11.Clone();
      val.m_P12 = m_P12.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
      m_P11.Evaluate(iterator, args);
      m_P12.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
      m_P11.Evaluate(instance);
      m_P12.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
      m_P11.Analyze(instance);
      m_P12.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue && m_P11.HaveValue && m_P12.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }
    public P11 Param11Value
    {
      get { return m_P11.Value; }
    }
    public P12 Param12Value
    {
      get { return m_P12.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
    private ISkillValue<P11> m_P11 = new SkillValue<P11>();
    private ISkillValue<P12> m_P12 = new SkillValue<P12>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 13) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
        m_P11.InitFromDsl(callData.GetParam(startIndex + 10));
        m_P12.InitFromDsl(callData.GetParam(startIndex + 11));
        m_P13.InitFromDsl(callData.GetParam(startIndex + 12));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      val.m_P11 = m_P11.Clone();
      val.m_P12 = m_P12.Clone();
      val.m_P13 = m_P13.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
      m_P11.Evaluate(iterator, args);
      m_P12.Evaluate(iterator, args);
      m_P13.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
      m_P11.Evaluate(instance);
      m_P12.Evaluate(instance);
      m_P13.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
      m_P11.Analyze(instance);
      m_P12.Analyze(instance);
      m_P13.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue && m_P11.HaveValue && m_P12.HaveValue && m_P13.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }
    public P11 Param11Value
    {
      get { return m_P11.Value; }
    }
    public P12 Param12Value
    {
      get { return m_P12.Value; }
    }
    public P13 Param13Value
    {
      get { return m_P13.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
    private ISkillValue<P11> m_P11 = new SkillValue<P11>();
    private ISkillValue<P12> m_P12 = new SkillValue<P12>();
    private ISkillValue<P13> m_P13 = new SkillValue<P13>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 14) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
        m_P11.InitFromDsl(callData.GetParam(startIndex + 10));
        m_P12.InitFromDsl(callData.GetParam(startIndex + 11));
        m_P13.InitFromDsl(callData.GetParam(startIndex + 12));
        m_P14.InitFromDsl(callData.GetParam(startIndex + 13));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      val.m_P11 = m_P11.Clone();
      val.m_P12 = m_P12.Clone();
      val.m_P13 = m_P13.Clone();
      val.m_P14 = m_P14.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
      m_P11.Evaluate(iterator, args);
      m_P12.Evaluate(iterator, args);
      m_P13.Evaluate(iterator, args);
      m_P14.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
      m_P11.Evaluate(instance);
      m_P12.Evaluate(instance);
      m_P13.Evaluate(instance);
      m_P14.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
      m_P11.Analyze(instance);
      m_P12.Analyze(instance);
      m_P13.Analyze(instance);
      m_P14.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue && m_P11.HaveValue && m_P12.HaveValue && m_P13.HaveValue && m_P14.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }
    public P11 Param11Value
    {
      get { return m_P11.Value; }
    }
    public P12 Param12Value
    {
      get { return m_P12.Value; }
    }
    public P13 Param13Value
    {
      get { return m_P13.Value; }
    }
    public P14 Param14Value
    {
      get { return m_P14.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
    private ISkillValue<P11> m_P11 = new SkillValue<P11>();
    private ISkillValue<P12> m_P12 = new SkillValue<P12>();
    private ISkillValue<P13> m_P13 = new SkillValue<P13>();
    private ISkillValue<P14> m_P14 = new SkillValue<P14>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 15) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
        m_P11.InitFromDsl(callData.GetParam(startIndex + 10));
        m_P12.InitFromDsl(callData.GetParam(startIndex + 11));
        m_P13.InitFromDsl(callData.GetParam(startIndex + 12));
        m_P14.InitFromDsl(callData.GetParam(startIndex + 13));
        m_P15.InitFromDsl(callData.GetParam(startIndex + 14));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      val.m_P11 = m_P11.Clone();
      val.m_P12 = m_P12.Clone();
      val.m_P13 = m_P13.Clone();
      val.m_P14 = m_P14.Clone();
      val.m_P15 = m_P15.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
      m_P11.Evaluate(iterator, args);
      m_P12.Evaluate(iterator, args);
      m_P13.Evaluate(iterator, args);
      m_P14.Evaluate(iterator, args);
      m_P15.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
      m_P11.Evaluate(instance);
      m_P12.Evaluate(instance);
      m_P13.Evaluate(instance);
      m_P14.Evaluate(instance);
      m_P15.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
      m_P11.Analyze(instance);
      m_P12.Analyze(instance);
      m_P13.Analyze(instance);
      m_P14.Analyze(instance);
      m_P15.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue && m_P11.HaveValue && m_P12.HaveValue && m_P13.HaveValue && m_P14.HaveValue && m_P15.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }
    public P11 Param11Value
    {
      get { return m_P11.Value; }
    }
    public P12 Param12Value
    {
      get { return m_P12.Value; }
    }
    public P13 Param13Value
    {
      get { return m_P13.Value; }
    }
    public P14 Param14Value
    {
      get { return m_P14.Value; }
    }
    public P15 Param15Value
    {
      get { return m_P15.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
    private ISkillValue<P11> m_P11 = new SkillValue<P11>();
    private ISkillValue<P12> m_P12 = new SkillValue<P12>();
    private ISkillValue<P13> m_P13 = new SkillValue<P13>();
    private ISkillValue<P14> m_P14 = new SkillValue<P14>();
    private ISkillValue<P15> m_P15 = new SkillValue<P15>();
  }
  public sealed class SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData && callData.GetParamNum() >= 16) {
        m_P1.InitFromDsl(callData.GetParam(startIndex + 0));
        m_P2.InitFromDsl(callData.GetParam(startIndex + 1));
        m_P3.InitFromDsl(callData.GetParam(startIndex + 2));
        m_P4.InitFromDsl(callData.GetParam(startIndex + 3));
        m_P5.InitFromDsl(callData.GetParam(startIndex + 4));
        m_P6.InitFromDsl(callData.GetParam(startIndex + 5));
        m_P7.InitFromDsl(callData.GetParam(startIndex + 6));
        m_P8.InitFromDsl(callData.GetParam(startIndex + 7));
        m_P9.InitFromDsl(callData.GetParam(startIndex + 8));
        m_P10.InitFromDsl(callData.GetParam(startIndex + 9));
        m_P11.InitFromDsl(callData.GetParam(startIndex + 10));
        m_P12.InitFromDsl(callData.GetParam(startIndex + 11));
        m_P13.InitFromDsl(callData.GetParam(startIndex + 12));
        m_P14.InitFromDsl(callData.GetParam(startIndex + 13));
        m_P15.InitFromDsl(callData.GetParam(startIndex + 14));
        m_P16.InitFromDsl(callData.GetParam(startIndex + 15));
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16> val = new SkillValueParam<P1, P2, P3, P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16>();
      val.m_P1 = m_P1.Clone();
      val.m_P2 = m_P2.Clone();
      val.m_P3 = m_P3.Clone();
      val.m_P4 = m_P4.Clone();
      val.m_P5 = m_P5.Clone();
      val.m_P6 = m_P6.Clone();
      val.m_P7 = m_P7.Clone();
      val.m_P8 = m_P8.Clone();
      val.m_P9 = m_P9.Clone();
      val.m_P10 = m_P10.Clone();
      val.m_P11 = m_P11.Clone();
      val.m_P12 = m_P12.Clone();
      val.m_P13 = m_P13.Clone();
      val.m_P14 = m_P14.Clone();
      val.m_P15 = m_P15.Clone();
      val.m_P16 = m_P16.Clone();
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_P1.Evaluate(iterator, args);
      m_P2.Evaluate(iterator, args);
      m_P3.Evaluate(iterator, args);
      m_P4.Evaluate(iterator, args);
      m_P5.Evaluate(iterator, args);
      m_P6.Evaluate(iterator, args);
      m_P7.Evaluate(iterator, args);
      m_P8.Evaluate(iterator, args);
      m_P9.Evaluate(iterator, args);
      m_P10.Evaluate(iterator, args);
      m_P11.Evaluate(iterator, args);
      m_P12.Evaluate(iterator, args);
      m_P13.Evaluate(iterator, args);
      m_P14.Evaluate(iterator, args);
      m_P15.Evaluate(iterator, args);
      m_P16.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_P1.Evaluate(instance);
      m_P2.Evaluate(instance);
      m_P3.Evaluate(instance);
      m_P4.Evaluate(instance);
      m_P5.Evaluate(instance);
      m_P6.Evaluate(instance);
      m_P7.Evaluate(instance);
      m_P8.Evaluate(instance);
      m_P9.Evaluate(instance);
      m_P10.Evaluate(instance);
      m_P11.Evaluate(instance);
      m_P12.Evaluate(instance);
      m_P13.Evaluate(instance);
      m_P14.Evaluate(instance);
      m_P15.Evaluate(instance);
      m_P16.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_P1.Analyze(instance);
      m_P2.Analyze(instance);
      m_P3.Analyze(instance);
      m_P4.Analyze(instance);
      m_P5.Analyze(instance);
      m_P6.Analyze(instance);
      m_P7.Analyze(instance);
      m_P8.Analyze(instance);
      m_P9.Analyze(instance);
      m_P10.Analyze(instance);
      m_P11.Analyze(instance);
      m_P12.Analyze(instance);
      m_P13.Analyze(instance);
      m_P14.Analyze(instance);
      m_P15.Analyze(instance);
      m_P16.Analyze(instance);
    }
    public bool HaveValue
    {
      get { return m_P1.HaveValue && m_P2.HaveValue && m_P3.HaveValue && m_P4.HaveValue && m_P5.HaveValue && m_P6.HaveValue && m_P7.HaveValue && m_P8.HaveValue && m_P9.HaveValue && m_P10.HaveValue && m_P11.HaveValue && m_P12.HaveValue && m_P13.HaveValue && m_P14.HaveValue && m_P15.HaveValue && m_P16.HaveValue; }
    }
    public P1 Param1Value
    {
      get { return m_P1.Value; }
    }
    public P2 Param2Value
    {
      get { return m_P2.Value; }
    }
    public P3 Param3Value
    {
      get { return m_P3.Value; }
    }
    public P4 Param4Value
    {
      get { return m_P4.Value; }
    }
    public P5 Param5Value
    {
      get { return m_P5.Value; }
    }
    public P6 Param6Value
    {
      get { return m_P6.Value; }
    }
    public P7 Param7Value
    {
      get { return m_P7.Value; }
    }
    public P8 Param8Value
    {
      get { return m_P8.Value; }
    }
    public P9 Param9Value
    {
      get { return m_P9.Value; }
    }
    public P10 Param10Value
    {
      get { return m_P10.Value; }
    }
    public P11 Param11Value
    {
      get { return m_P11.Value; }
    }
    public P12 Param12Value
    {
      get { return m_P12.Value; }
    }
    public P13 Param13Value
    {
      get { return m_P13.Value; }
    }
    public P14 Param14Value
    {
      get { return m_P14.Value; }
    }
    public P15 Param15Value
    {
      get { return m_P15.Value; }
    }
    public P16 Param16Value
    {
      get { return m_P16.Value; }
    }

    private ISkillValue<P1> m_P1 = new SkillValue<P1>();
    private ISkillValue<P2> m_P2 = new SkillValue<P2>();
    private ISkillValue<P3> m_P3 = new SkillValue<P3>();
    private ISkillValue<P4> m_P4 = new SkillValue<P4>();
    private ISkillValue<P5> m_P5 = new SkillValue<P5>();
    private ISkillValue<P6> m_P6 = new SkillValue<P6>();
    private ISkillValue<P7> m_P7 = new SkillValue<P7>();
    private ISkillValue<P8> m_P8 = new SkillValue<P8>();
    private ISkillValue<P9> m_P9 = new SkillValue<P9>();
    private ISkillValue<P10> m_P10 = new SkillValue<P10>();
    private ISkillValue<P11> m_P11 = new SkillValue<P11>();
    private ISkillValue<P12> m_P12 = new SkillValue<P12>();
    private ISkillValue<P13> m_P13 = new SkillValue<P13>();
    private ISkillValue<P14> m_P14 = new SkillValue<P14>();
    private ISkillValue<P15> m_P15 = new SkillValue<P15>();
    private ISkillValue<P16> m_P16 = new SkillValue<P16>();
  }
  */
  public sealed class SkillValueParams<P> : ISkillValueParam
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param, int startIndex)
    {
      ScriptableData.CallData callData = param as ScriptableData.CallData;
      if (null != callData) {
        for (int i = startIndex; i < callData.GetParamNum(); ++i) {
          SkillValue<P> val = new SkillValue<P>();
          val.InitFromDsl(callData.GetParam(i));
          m_Args.Add(val);
        }
      }
    }
    public ISkillValueParam Clone()
    {
      SkillValueParams<P> val = new SkillValueParams<P>();
      foreach (ISkillValue<P> arg in m_Args) {
        val.m_Args.Add(arg.Clone());
      }
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      foreach (ISkillValue<P> val in m_Args) {
        val.Evaluate(iterator, args);
      }
    }
    public void Evaluate(SkillInstance instance)
    {
      foreach (ISkillValue<P> val in m_Args) {
        val.Evaluate(instance);
      }
    }
    public void Analyze(SkillInstance instance)
    {
      foreach (ISkillValue<P> val in m_Args) {
        val.Analyze(instance);
      }
    }
    public bool HaveValue
    {
      get
      {
        bool ret = true;
        foreach (ISkillValue<P> val in m_Args) {
          if (!val.HaveValue) {
            ret = false;
            break;
          }
        }
        return ret;
      }
    }
    public List<P> Values
    {
      get
      {
        List<P> vals = new List<P>();
        foreach (ISkillValue<P> val in m_Args) {
          vals.Add(val.Value);
        }
        return vals;
      }
    }

    private List<ISkillValue<P>> m_Args = new List<ISkillValue<P>>();
  }
}
