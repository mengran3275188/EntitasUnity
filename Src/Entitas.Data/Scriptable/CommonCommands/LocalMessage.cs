using System;
using System.Collections.Generic;
using System.Collections;

namespace ScriptableData.CommonCommands
{
  /// <summary>
  /// firemessage(msgid,arg1,arg2,...);
  /// </summary>
  internal class LocalMessageCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      LocalMessageCommand cmd = new LocalMessageCommand();
      cmd.m_MsgId = m_MsgId.Clone();
      for (int i = 0; i < m_MsgArgs.Count; i++ )
      {
        cmd.m_MsgArgs.Add(m_MsgArgs[i].Clone());
      }
      /*
      foreach (SkillValue val in m_MsgArgs) {
        cmd.m_MsgArgs.Add(val.Clone());
      }*/
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_MsgId.Evaluate(iterator, args);
      for (int i = 0; i < m_MsgArgs.Count; i++)
      {
        m_MsgArgs[i].Evaluate(iterator, args);
      }
      /*
      foreach (SkillValue val in m_MsgArgs) {
        val.Evaluate(iterator, args);
      }*/
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_MsgId.Evaluate(instance);
      for (int i = 0; i < m_MsgArgs.Count; i++)
      {
        m_MsgArgs[i].Evaluate(instance);
      }
      /*
      foreach (SkillValue val in m_MsgArgs) {
        val.Evaluate(instance);
      }*/
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      string msgId = m_MsgId.Value;
      ArrayList arglist = new ArrayList();
      for (int i = 0; i < m_MsgArgs.Count; i++)
      {
        arglist.Add(m_MsgArgs[i].Value);
      }
      /*
      foreach (SkillValue val in m_MsgArgs) {
        arglist.Add(val.Value);
      }*/
      object[] args = arglist.ToArray();
      instance.SendMessage(msgId, args);
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      if (num > 0) {
        m_MsgId.InitFromDsl(callData.GetParam(0));
      }
      for (int i = 1; i < num; ++i) {
        SkillValue val = new SkillValue();
        val.InitFromDsl(callData.GetParam(i));
        m_MsgArgs.Add(val);
      }
    }

    private IValue<string> m_MsgId = new SkillValue<string>();
    private List<IValue<object>> m_MsgArgs = new List<IValue<object>>();
  }
  /// <summary>
  /// clearmessage(msgid1,msgid2,...);
  /// </summary>
  internal class ClearMessageCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      ClearMessageCommand cmd = new ClearMessageCommand();
      for (int i = 0; i < m_MsgIds.Count; i++) {
        cmd.m_MsgIds.Add(m_MsgIds[i].Clone());
      }
      return cmd;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      for (int i = 0; i < m_MsgIds.Count; i++) {
        m_MsgIds[i].Evaluate(iterator, args);
      }
    }

    protected override void UpdateVariables(IInstance instance)
    {
      for (int i = 0; i < m_MsgIds.Count; i++) {
        m_MsgIds[i].Evaluate(instance);
      }
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      string[] arglist = new string[m_MsgIds.Count];
      for (int i = 0; i < m_MsgIds.Count; i++) {
        arglist[i] = m_MsgIds[i].Value;
      }
      instance.ClearMessage(arglist);
      return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      for (int i = 0; i < num; ++i) {
        IValue<string> val = new SkillValue<string>();
        val.InitFromDsl(callData.GetParam(i));
        m_MsgIds.Add(val);
      }
    }

    private List<IValue<string>> m_MsgIds = new List<IValue<string>>();
  }
}
