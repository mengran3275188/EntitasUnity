using System;
using System.Collections.Generic;

namespace ScriptableData.CommonCommands
{
  /// <summary>
  /// sleep(milliseconds);
  /// </summary>
  internal class SleepCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      SleepCommand cmd = new SleepCommand();
      cmd.m_Time = m_Time.Clone();
      return cmd;
    }

    protected override void ResetState()
    {
      m_CurTime = 0;
    }

    protected override void UpdateArguments(object iterator, object[] args)
    {
      m_Time.Evaluate(iterator, args);    
    }

    protected override void UpdateVariables(IInstance instance)
    {
      m_Time.Evaluate(instance);
    }

    protected override ExecResult ExecCommand(IInstance instance, long delta)
    {
      int curTime = m_CurTime;
      m_CurTime += (int)delta;
      if (curTime <= m_Time.Value)
        return ExecResult.Blocked;
      else
        return ExecResult.Finished;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      if (num > 0) {
        m_Time.InitFromDsl(callData.GetParam(0));
      }
    }

    private IValue<int> m_Time = new SkillValue<int>();
    private int m_CurTime = 0;
  }
}
