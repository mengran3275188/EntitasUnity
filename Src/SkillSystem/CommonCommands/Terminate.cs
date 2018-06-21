using System;
using System.Collections.Generic;

namespace Skill.CommonCommands
{
  /// <summary>
  /// terminate();
  /// </summary>
  internal class TerminateCommand : AbstractSkillCommand
  {
    public override ISkillCommand Clone()
    {
      TerminateCommand cmd = new TerminateCommand();
      return cmd;
    }

    protected override ExecResult ExecCommand(SkillInstance instance, long delta)
    {
      instance.IsTerminated = true;
      return ExecResult.Finished;
    }
    protected override void Load(ScriptableData.CallData callData)
    {}
  }
}
