using System;
using System.Collections;
using System.Collections.Generic;

namespace ScriptableData.CommonCommands
{
  /// <summary>
  /// assign(@local,value);
  /// or
  /// assign(@@global,value);
  /// </summary>
  internal class AssignCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      AssignCommand cmd = new AssignCommand();
      cmd.m_VarName = m_VarName;
      cmd.m_Value = m_Value.Clone();
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Value.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_Value.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      if (m_Value.HaveValue) {
        if (m_VarName.StartsWith("@@")) {
          if (null != instance.GlobalVariables) {
            if (instance.GlobalVariables.ContainsKey(m_VarName)) {
              instance.GlobalVariables[m_VarName] = m_Value.Value;
            } else {
              instance.GlobalVariables.Add(m_VarName, m_Value.Value);
            }
          }
        } else if (m_VarName.StartsWith("@")) {
          if (instance.LocalVariables.ContainsKey(m_VarName)) {
            instance.LocalVariables[m_VarName] = m_Value.Value;
          } else {
            instance.LocalVariables.Add(m_VarName, m_Value.Value);
          }
        }
      }
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      if (num > 1) {
        m_VarName = callData.GetParamId(0);
        m_Value.InitFromDsl(callData.GetParam(1));
      }
    }

    private string m_VarName = null;
    private IValue<object> m_Value = new SkillValue();
  }
  /// <summary>
  /// inc(@local,value);
  /// or
  /// inc(@@global,value);
  /// </summary>
  internal class IncCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      IncCommand cmd = new IncCommand();
      cmd.m_VarName = m_VarName;
      cmd.m_Value = m_Value.Clone();
      cmd.m_ParamNum = m_ParamNum;
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Value.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_Value.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      if (m_VarName.StartsWith("@@")) {
        if (null != instance.GlobalVariables) {
          if (instance.GlobalVariables.ContainsKey(m_VarName)) {
            object oval = instance.GlobalVariables[m_VarName];
            if (oval is int) {
              int ov = ValueHelper.CastTo<int>(oval);
              if (m_ParamNum > 1 && m_Value.HaveValue) {
                int v = ValueHelper.CastTo<int>(m_Value.Value);
                ov += v;
                instance.GlobalVariables[m_VarName] = ov;
              } else {
                ++ov;
                instance.GlobalVariables[m_VarName] = ov;
              }
            } else {
              float ov = ValueHelper.CastTo<float>(oval);
              if (m_ParamNum > 1 && m_Value.HaveValue) {
                float v = ValueHelper.CastTo<float>(m_Value.Value);
                ov += v;
                instance.GlobalVariables[m_VarName] = ov;
              } else {
                ++ov;
                instance.GlobalVariables[m_VarName] = ov;
              }
            }
          }
        }
      } else if (m_VarName.StartsWith("@")) {
        if (instance.LocalVariables.ContainsKey(m_VarName)) {
          object oval = instance.LocalVariables[m_VarName];
          if (oval is int) {
            int ov = ValueHelper.CastTo<int>(oval);
            if (m_ParamNum > 1 && m_Value.HaveValue) {
              int v = ValueHelper.CastTo<int>(m_Value.Value);
              ov += v;
              instance.LocalVariables[m_VarName] = ov;
            } else {
              ++ov;
              instance.LocalVariables[m_VarName] = ov;
            }
          } else {
            float ov = ValueHelper.CastTo<float>(oval);
            if (m_ParamNum > 1 && m_Value.HaveValue) {
              float v = ValueHelper.CastTo<float>(m_Value.Value);
              ov += v;
              instance.LocalVariables[m_VarName] = ov;
            } else {
              ++ov;
              instance.LocalVariables[m_VarName] = ov;
            }
          }
        }
      }
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      m_ParamNum = num;
      if (num > 0) {
        m_VarName = callData.GetParamId(0);
      }
      if (num > 1) {
        m_Value.InitFromDsl(callData.GetParam(1));
      }
    }

    private int m_ParamNum = 0;
    private string m_VarName = null;
    private IValue<object> m_Value = new SkillValue();
  }
  /// <summary>
  /// dec(@local,value);
  /// or
  /// dec(@@global,value);
  /// </summary>
  internal class DecCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      DecCommand cmd = new DecCommand();
      cmd.m_VarName = m_VarName;
      cmd.m_Value = m_Value.Clone();
      cmd.m_ParamNum = m_ParamNum;
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Value.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_Value.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      if (m_VarName.StartsWith("@@")) {
        if (null != instance.GlobalVariables) {
          if (instance.GlobalVariables.ContainsKey(m_VarName)) {
            object oval = instance.GlobalVariables[m_VarName];
            if (oval is int) {
              int ov = ValueHelper.CastTo<int>(oval);
              if (m_ParamNum > 1 && m_Value.HaveValue) {
                int v = ValueHelper.CastTo<int>(m_Value.Value);
                ov -= v;
                instance.GlobalVariables[m_VarName] = ov;
              } else {
                --ov;
                instance.GlobalVariables[m_VarName] = ov;
              }
            } else {
              float ov = ValueHelper.CastTo<float>(oval);
              if (m_ParamNum > 1 && m_Value.HaveValue) {
                float v = ValueHelper.CastTo<float>(m_Value.Value);
                ov -= v;
                instance.GlobalVariables[m_VarName] = ov;
              } else {
                --ov;
                instance.GlobalVariables[m_VarName] = ov;
              }
            }
          }
        }
      } else if (m_VarName.StartsWith("@")) {
        if (instance.LocalVariables.ContainsKey(m_VarName)) {
          object oval = instance.LocalVariables[m_VarName];
          if (oval is int) {
            int ov = ValueHelper.CastTo<int>(oval);
            if (m_ParamNum > 1 && m_Value.HaveValue) {
              int v = ValueHelper.CastTo<int>(m_Value.Value);
              ov -= v;
              instance.LocalVariables[m_VarName] = ov;
            } else {
              --ov;
              instance.LocalVariables[m_VarName] = ov;
            }
          } else {
            float ov = ValueHelper.CastTo<float>(oval);
            if (m_ParamNum > 1 && m_Value.HaveValue) {
              float v = ValueHelper.CastTo<float>(m_Value.Value);
              ov -= v;
              instance.LocalVariables[m_VarName] = ov;
            } else {
              --ov;
              instance.LocalVariables[m_VarName] = ov;
            }
          }
        }
      }
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      m_ParamNum = num;
      if (num > 0) {
        m_VarName = callData.GetParamId(0);
      }
      if (num > 1) {
        m_Value.InitFromDsl(callData.GetParam(1));
      }
    }

    private int m_ParamNum = 0;
    private string m_VarName = null;
    private IValue<object> m_Value = new SkillValue();
  }
  /// <summary>
  /// propset(name,value);
  /// </summary>
  internal class PropSetCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      PropSetCommand cmd = new PropSetCommand();
      cmd.m_VarName = m_VarName.Clone();
      cmd.m_Value = m_Value.Clone();
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_VarName.Evaluate(iterator, args);
      m_Value.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_VarName.Evaluate(instance);
      m_Value.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      if (m_VarName.HaveValue && m_Value.HaveValue) {
        string varName = m_VarName.Value;
        if (varName.StartsWith("@") && !varName.StartsWith("@@")) {
          if (instance.LocalVariables.ContainsKey(varName) && m_Value.HaveValue) {
            instance.LocalVariables[varName] = m_Value.Value;
          } else {
            instance.LocalVariables.Add(varName, m_Value.Value);
          }
        } else {
          if (null != instance.GlobalVariables && m_Value.HaveValue) {
            if (instance.GlobalVariables.ContainsKey(varName)) {
              instance.GlobalVariables[varName] = m_Value.Value;
            } else {
              instance.GlobalVariables.Add(varName, m_Value.Value);
            }
          }
        }
      }
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      if (num > 1) {
        m_VarName.InitFromDsl(callData.GetParam(0));
        m_Value.InitFromDsl(callData.GetParam(1));
      }
    }

    private IValue<string> m_VarName = new SkillValue<string>();
    private IValue<object> m_Value = new SkillValue();
  }
  /// <summary>
  /// listset(list,index,value);
  /// </summary>
  internal class ListSetCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      ListSetCommand cmd = new ListSetCommand();
      cmd.m_ListValue = m_ListValue.Clone();
      cmd.m_IndexValue = m_IndexValue.Clone();
      cmd.m_Value = m_Value.Clone();
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_ListValue.Evaluate(iterator, args);
      m_IndexValue.Evaluate(iterator, args);
      m_Value.Evaluate(iterator, args);
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_ListValue.Evaluate(instance);
      m_IndexValue.Evaluate(instance);
      m_Value.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      if (m_ListValue.HaveValue && m_IndexValue.HaveValue && m_Value.HaveValue) {
        IList listValue = m_ListValue.Value;
        int index = m_IndexValue.Value;
        object val = m_Value.Value;
        int ct = listValue.Count;
        if (index >= 0 && index < ct) {
          listValue[index] = val;
        } else {
          listValue.Add(val);
        }
      }
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      if (num > 2) {
        m_ListValue.InitFromDsl(callData.GetParam(0));
        m_IndexValue.InitFromDsl(callData.GetParam(1));
        m_Value.InitFromDsl(callData.GetParam(2));
      }
    }

    private IValue<IList> m_ListValue = new SkillValue<IList>();
    private IValue<int> m_IndexValue = new SkillValue<int>();
    private IValue<object> m_Value = new SkillValue();
  }
}
