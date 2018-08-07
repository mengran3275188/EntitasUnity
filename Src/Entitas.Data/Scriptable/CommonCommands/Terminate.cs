using System;
using System.Collections.Generic;

namespace ScriptableSystem.CommonCommands
{
  /// <summary>
  /// terminate();
  /// </summary>
  internal class TerminateCommand : AbstractCommand
  {
    public override ICommand Clone()
    {
      TerminateCommand cmd = new TerminateCommand();
      return cmd;
    }

    protected override ExecResult ExecCommand(Instance instance, long delta)
    {
      instance.IsTerminated = true;
      return ExecResult.Finished;
    }
    protected override void Load(ScriptableData.CallData callData)
    {}
  }
}
