using System;
using ScriptableData;

namespace SkillCommands
{
    internal class MoveChildTrigger : AbstractCommand
    {
        internal void Load(string childName, string nodeName)
        {
            m_ChildName = childName;
            m_NodeName = nodeName;
        }
        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() > 0)
            {
                m_ChildName = callData.GetParamId(0);
            }
            if (callData.GetParamNum() > 1)
            {
                m_NodeName = callData.GetParamId(1);
            }
        }

        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            if (instance.Target is GameEntity entity)
            {
                UnityClient.GfxSystem.MoveChildToBone(entity.resource.Value, m_ChildName, m_NodeName);
            }
            return ExecResult.Finished;
        }

        private string m_ChildName = "";
        private string m_NodeName = "";
    }
}
