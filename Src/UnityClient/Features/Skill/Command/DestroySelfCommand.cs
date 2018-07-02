using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptableSystem;

namespace SkillCommands
{
    internal class DestroySelfCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            var target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            target.isDestory = true;

            Util.LogUtil.Info("Destroy self ~");
            return ExecResult.Finished;
        }
    }
}
