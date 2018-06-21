using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skill
{
  public interface ISkillValue<T>
  {
    void InitFromDsl(ScriptableData.ISyntaxComponent param);//从DSL语言初始化值实例
    ISkillValue<T> Clone();//克隆一个新实例，每个值只从DSL语言初始化一次，之后的实例由克隆产生，提升性能
    void Evaluate(object iterator, object[] args);//从引用的参数获取参数值
    void Evaluate(SkillInstance instance);//从引用的变量获取变量值
    void Analyze(SkillInstance instance);//语义分析，配合上下文instance进行语义分析，在执行前收集必要的信息
    bool HaveValue { get; }//是否已经有值，对常量初始化后即产生值，对参数、变量与函数则在Evaluate后产生值
    T Value { get; }//具体的值
  }
  public sealed class SkillValue : ISkillValue<object>
  {
    public const int c_Iterator = -2;
    public const int c_NotArg = -1;
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      string id = param.GetId();
      int idType = param.GetIdType();
      if (idType == ScriptableData.ValueData.ID_TOKEN && id.StartsWith("$")) {
        if (0 == id.CompareTo("$$"))
          SetArgument(c_Iterator);
        else
          SetArgument(int.Parse(id.Substring(1)));
      } else if (idType == ScriptableData.ValueData.ID_TOKEN && id.StartsWith("@")) {
        if (id.StartsWith("@@"))
          SetGlobal(id);
        else
          SetLocal(id);
      } else {
        CalcInitValue(param);
      }
    }
    public ISkillValue<object> Clone()
    {
      SkillValue obj = new SkillValue();
      obj.m_ArgIndex = m_ArgIndex;
      obj.m_LocalName = m_LocalName;
      obj.m_GlobalName = m_GlobalName;
      if (null != m_Proxy) {
        obj.m_Proxy = m_Proxy.Clone();
      }
      obj.m_HaveValue = m_HaveValue;
      obj.m_Value = m_Value;
      return obj;
    }
    public void Evaluate(object iterator, object[] args)
    {
      if (m_ArgIndex >= 0 && m_ArgIndex < args.Length) {
        m_Value = args[m_ArgIndex];
        m_HaveValue = true;
      } else if (m_ArgIndex == c_Iterator) {
        m_Value = iterator;
        m_HaveValue = true;
      } else if (null != m_Proxy) {
        m_Proxy.Evaluate(iterator, args);
        if (m_Proxy.HaveValue) {
          m_Value = m_Proxy.Value;
          m_HaveValue = true;
        }
      }
    }
    public void Evaluate(SkillInstance instance)
    {
      if (null != m_LocalName) {
        Dictionary<string, object> locals = instance.LocalVariables;
        object val;
        if (locals.TryGetValue(m_LocalName, out val)) {
          m_Value = val;
          m_HaveValue = true;
        }
      } else if (null != m_GlobalName) {
        Dictionary<string, object> globals = instance.GlobalVariables;
        if (null != globals) {
          object val;
          if (globals.TryGetValue(m_GlobalName, out val)) {
            m_Value = val;
            m_HaveValue = true;
          }
        }
      } else if (null != m_Proxy) {
        m_Proxy.Evaluate(instance);
        if (m_Proxy.HaveValue) {
          m_Value = m_Proxy.Value;
          m_HaveValue = true;
        }
      }
    }
    public void Analyze(SkillInstance instance)
    {
      if (null != m_Proxy) {
        m_Proxy.Analyze(instance);
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
    private void SetArgument(int index)
    {
      m_HaveValue = false;
      m_ArgIndex = index;
      m_LocalName = null;
      m_GlobalName = null;
      m_Proxy = null;
      m_Value = null;
    }
    private void SetLocal(string name)
    {
      m_HaveValue = false;
      m_ArgIndex = c_NotArg;
      m_LocalName = name;
      m_GlobalName = null;
      m_Proxy = null;
      m_Value = null;
    }
    private void SetGlobal(string name)
    {
      m_HaveValue = false;
      m_ArgIndex = c_NotArg;
      m_LocalName = null;
      m_GlobalName = name;
      m_Proxy = null;
      m_Value = null;
    }
    private void SetProxy(ISkillValue<object> proxy)
    {
      m_HaveValue = false;
      m_ArgIndex = c_NotArg;
      m_LocalName = null;
      m_GlobalName = null;
      m_Proxy = proxy;
      m_Value = null;
    }
    private void SetValue(object val)
    {
      m_HaveValue = true;
      m_ArgIndex = c_NotArg;
      m_LocalName = null;
      m_GlobalName = null;
      m_Proxy = null;
      m_Value = val;
    }
    private void CalcInitValue(ScriptableData.ISyntaxComponent param)
    {
      ISkillValue<object> val = SkillValueManager.Instance.CalcValue(param);
      if (null != val) {
        //对初始化即能求得值的函数，不需要再记录函数表达式，直接转换为常量值。
        if (val.HaveValue) {
          SetValue(val.Value);
        } else {
          SetProxy(val);
        }
      } else {
        string id = param.GetId();
        int idType = param.GetIdType();
        if (idType == ScriptableData.ValueData.NUM_TOKEN) {
          if (id.IndexOf('.') >= 0)
            SetValue(float.Parse(id, System.Globalization.NumberStyles.Float));
          else if (id.StartsWith("0x"))
            SetValue(uint.Parse(id.Substring(2), System.Globalization.NumberStyles.HexNumber));
          else
            SetValue(int.Parse(id, System.Globalization.NumberStyles.Integer));
        } else {
          SetValue(id);
        }
      }
    }

    private bool m_HaveValue = false;
    private int m_ArgIndex = c_NotArg;
    private string m_LocalName = null;
    private string m_GlobalName = null;
    private ISkillValue<object> m_Proxy = null;
    private object m_Value;
  }
  public sealed class SkillValue<T> : ISkillValue<T>
  {
    public const int c_Iterator = -2;
    public const int c_NotArg = -1;
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      string id = param.GetId();
      int idType = param.GetIdType();
      if (idType == ScriptableData.ValueData.ID_TOKEN && id.StartsWith("$")) {
        if (0 == id.CompareTo("$$"))
          SetArgument(c_Iterator);
        else
          SetArgument(int.Parse(id.Substring(1)));
      } else if (idType == ScriptableData.ValueData.ID_TOKEN && id.StartsWith("@")) {
        if (id.StartsWith("@@"))
          SetGlobal(id);
        else
          SetLocal(id);
      } else {
        CalcInitValue(param);
      }
    }
    public ISkillValue<T> Clone()
    {
      SkillValue<T> obj = new SkillValue<T>();
      obj.m_ArgIndex = m_ArgIndex;
      obj.m_LocalName = m_LocalName;
      obj.m_GlobalName = m_GlobalName;
      if (null != m_Proxy) {
        obj.m_Proxy = m_Proxy.Clone();
      }
      obj.m_HaveValue = m_HaveValue;
      obj.m_Value = m_Value;
      return obj;
    }
    public void Evaluate(object iterator, object[] args)
    {
      if (m_ArgIndex >= 0 && m_ArgIndex < args.Length) {
        m_Value = SkillValueHelper.CastTo<T>(args[m_ArgIndex]);
        m_HaveValue = true;
      } else if (m_ArgIndex == c_Iterator) {
        m_Value = SkillValueHelper.CastTo<T>(iterator);
        m_HaveValue = true;
      } else if (null != m_Proxy) {
        m_Proxy.Evaluate(iterator, args);
        if (m_Proxy.HaveValue) {
          m_Value = SkillValueHelper.CastTo<T>(m_Proxy.Value);
          m_HaveValue = true;
        }
      }
    }
    public void Evaluate(SkillInstance instance)
    {
      if (null != m_LocalName) {
        Dictionary<string, object> locals = instance.LocalVariables;
        object val;
        if (locals.TryGetValue(m_LocalName, out val)) {
          m_Value = SkillValueHelper.CastTo<T>(val);
          m_HaveValue = true;
        }
      } else if (null != m_GlobalName) {
        Dictionary<string, object> globals = instance.GlobalVariables;
        if (null != globals) {
          object val;
          if (globals.TryGetValue(m_GlobalName, out val)) {
            m_Value = SkillValueHelper.CastTo<T>(val);
            m_HaveValue = true;
          }
        }
      } else if (null != m_Proxy) {
        m_Proxy.Evaluate(instance);
        if (m_Proxy.HaveValue) {
          m_Value = SkillValueHelper.CastTo<T>(m_Proxy.Value);
          m_HaveValue = true;
        }
      }
    }
    public void Analyze(SkillInstance instance)
    {
      if (null != m_Proxy) {
        m_Proxy.Analyze(instance);
      }
    }
    public bool HaveValue
    {
      get
      {
        return m_HaveValue;
      }
    }
    public T Value
    {
      get
      {
        return m_Value;
      }
    }

    private void SetArgument(int index)
    {
      m_HaveValue = false;
      m_ArgIndex = index;
      m_LocalName = null;
      m_GlobalName = null;
      m_Proxy = null;
      m_Value = default(T);
    }
    private void SetLocal(string name)
    {
      m_HaveValue = false;
      m_ArgIndex = c_NotArg;
      m_LocalName = name;
      m_GlobalName = null;
      m_Proxy = null;
      m_Value = default(T);
    }
    private void SetGlobal(string name)
    {
      m_HaveValue = false;
      m_ArgIndex = c_NotArg;
      m_LocalName = null;
      m_GlobalName = name;
      m_Proxy = null;
      m_Value = default(T);
    }
    private void SetProxy(ISkillValue<object> proxy)
    {
      m_HaveValue = false;
      m_ArgIndex = c_NotArg;
      m_LocalName = null;
      m_GlobalName = null;
      m_Proxy = proxy;
      m_Value = default(T);
    }
    private void SetValue(T val)
    {
      m_HaveValue = true;
      m_ArgIndex = c_NotArg;
      m_LocalName = null;
      m_GlobalName = null;
      m_Proxy = null;
      m_Value = val;
    }
    private void CalcInitValue(ScriptableData.ISyntaxComponent param)
    {
      ISkillValue<object> val = SkillValueManager.Instance.CalcValue(param);
      if (null != val) {
        //对初始化即能求得值的函数，不需要再记录函数表达式，直接转换为常量值。
        if (val.HaveValue) {
          SetValue(SkillValueHelper.CastTo<T>(val.Value));
        } else {
          SetProxy(val);
        }
      } else {
        string id = param.GetId();
        int idType = param.GetIdType();
        if (idType == ScriptableData.ValueData.NUM_TOKEN) {
          if (id.IndexOf('.') >= 0)
            SetValue(SkillValueHelper.CastTo<T>(float.Parse(id, System.Globalization.NumberStyles.Float)));
          else if (id.StartsWith("0x"))
            SetValue(SkillValueHelper.CastTo<T>(uint.Parse(id.Substring(2), System.Globalization.NumberStyles.HexNumber)));
          else
            SetValue(SkillValueHelper.CastTo<T>(int.Parse(id, System.Globalization.NumberStyles.Integer)));
        } else {
          SetValue(SkillValueHelper.CastTo<T>(id));
        }
      }
    }

    private bool m_HaveValue = false;
    private int m_ArgIndex = c_NotArg;
    private string m_LocalName = null;
    private string m_GlobalName = null;
    private ISkillValue<object> m_Proxy = null;
    private T m_Value;
  }
  internal sealed class SkillValueAdapter<T> : ISkillValue<object>
  {
    public void InitFromDsl(ScriptableData.ISyntaxComponent param)
    {
      m_Original.InitFromDsl(param);
    }
    public ISkillValue<object> Clone()
    {
      ISkillValue<T> newOriginal = m_Original.Clone();
      SkillValueAdapter<T> val = new SkillValueAdapter<T>(newOriginal);
      return val;
    }
    public void Evaluate(object iterator, object[] args)
    {
      m_Original.Evaluate(iterator, args);
    }
    public void Evaluate(SkillInstance instance)
    {
      m_Original.Evaluate(instance);
    }
    public void Analyze(SkillInstance instance)
    {
      m_Original.Analyze(instance);
    }
    public bool HaveValue
    {
      get
      {
        return m_Original.HaveValue;
      }
    }
    public object Value
    {
      get
      {
        return m_Original.Value;
      }
    }

    public SkillValueAdapter(ISkillValue<T> original)
    {
      m_Original = original;
    }

    private ISkillValue<T> m_Original = null;
  }
}
