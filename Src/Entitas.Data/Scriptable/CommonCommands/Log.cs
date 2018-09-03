using System;
using System.Collections;
using System.Collections.Generic;
using Util;

namespace ScriptableData.CommonCommands
{
  /// <summary>
  /// log(format,arg1,arg2,...);
  /// </summary>
  internal class LogCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      LogCommand cmd = new LogCommand();
      cmd.m_Format = m_Format.Clone();
      for (int i = 0; i < m_FormatArgs.Count; i++)
      {
        cmd.m_FormatArgs.Add(m_FormatArgs[i].Clone());
      }
      /*
      foreach (SkillValue val in m_FormatArgs) {
        cmd.m_FormatArgs.Add(val.Clone());
      }*/
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
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
    }

    protected override void UpdateVariables(IInstance instance)
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
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
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
      LogUtil.Info(m_Format.Value, args);
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      if (num > 0) {
        m_Format.InitFromDsl(callData.GetParam(0));
      }
      for (int i = 1; i < callData.GetParamNum(); ++i) {
        SkillValue val = new SkillValue();
        val.InitFromDsl(callData.GetParam(i));
        m_FormatArgs.Add(val);
      }
    }

    private IValue<string> m_Format = new SkillValue<string>();
    private List<IValue<object>> m_FormatArgs = new List<IValue<object>>();
  }
}
