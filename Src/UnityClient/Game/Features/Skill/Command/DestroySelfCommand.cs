using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptableData;

namespace SkillCommands
{
    internal class DestroySelfCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            var target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            target.isDestory = true;

            return ExecResult.Finished;
        }
    }
}