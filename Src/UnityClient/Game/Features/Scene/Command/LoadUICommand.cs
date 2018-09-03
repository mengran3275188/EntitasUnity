using System;
using System.Collections.Generic;
using ScriptableData;
using UnityClient;

namespace SceneCommand
{
    internal class LoadUICommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            if(callData.GetParamNum() >= 1)
            {
                m_UIPath = callData.GetParamId(0);
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            Services.Instance.UIService.LoadUI(m_UIPath);

            return ExecResult.Finished;
        }

        private string m_UIPath = string.Empty;
    }
}
