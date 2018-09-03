using System;
using System.Collections.Generic;

namespace ScriptableData.CommonCommands
{
    /// <summary>
    /// terminate();
    /// </summary>
    public class TerminateCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            TerminateCommand cmd = new TerminateCommand();
            return cmd;
        }

        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            instance.IsTerminated = true;
            return ExecResult.Finished;
        }
        protected override void Load(ScriptableData.CallData callData)
        { }
    }
}
