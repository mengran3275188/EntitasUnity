using System.Collections.Generic;
using Util;

namespace ScriptableData
{
  public interface IValueFactory
  {
    IValue<object> Build(ScriptableData.ISyntaxComponent param);
  }
  public sealed class ValueFactoryHelper<C> : IValueFactory where C : IValue<object>, new()
  {
    public IValue<object> Build(ScriptableData.ISyntaxComponent param)
    {
      C c = new C();
      c.InitFromDsl(param);
      return c;
    }
  }
  /// <summary>
  /// 这个类不加锁，约束条件：所有值注册必须在程序启动时完成。
  /// </summary>
  public class ValueManager : Singleton<ValueManager>
  {
    public void RegisterValueHandler(string name, IValueFactory handler)
    {
      if (!m_ValueHandlers.ContainsKey(name)) {
        m_ValueHandlers.Add(name, handler);
      } else {
        //error
      }
    }
    public IValue<object> CalcValue(ScriptableData.ISyntaxComponent param)
    {
      if (param.IsValid() && param.GetId().Length == 0) {
        //处理括弧
        ScriptableData.CallData callData = param as ScriptableData.CallData;
        if (null != callData && callData.GetParamNum() > 0) {
          int ct = callData.GetParamNum();
          return CalcValue(callData.GetParam(ct - 1));
        } else {
          //不支持的语法
          return null;
        }
      } else {
        IValue<object> ret = null;
        string id = param.GetId();
        IValueFactory factory;
        if (m_ValueHandlers.TryGetValue(id, out factory)) {
          ret = factory.Build(param);
        }
        return ret;
      }
    }
    private Dictionary<string, IValueFactory> m_ValueHandlers = new Dictionary<string, IValueFactory>();
  }
}
