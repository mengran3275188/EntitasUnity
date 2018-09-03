using System;
using ScriptableData;
using UnityEngine;
using Entitas.Data;

namespace SkillCommands
{
    internal class VolumetricLineCommand : AbstractCommand
    {
        public void Load(Vector3 startPos, Vector3 endPos, float width, long remainTime)
        {
            m_StartPos = startPos;
            m_EndPos = endPos;
            m_Width = width;
            m_RemianTime = remainTime;
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 4)
            {
                m_StartPos = ScriptableDataUtility.CalcVector3(callData.GetParam(0) as CallData);
                m_EndPos = ScriptableDataUtility.CalcVector3(callData.GetParam(1) as CallData);
                m_Width = float.Parse(callData.GetParamId(2));
                m_RemianTime = long.Parse(callData.GetParamId(3));
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null == obj)
                return ExecResult.Finished;

            Vector3 startPos = obj.position.Value + Quaternion.Euler(0, Mathf.Rad2Deg * obj.rotation.Value, 0) * m_StartPos;
            Vector3 endPos = obj.position.Value + Quaternion.Euler(0, Mathf.Rad2Deg * obj.rotation.Value, 0) * m_EndPos;

            GameObject line = UnityDelegate.ResourceSystem.NewObject("VolumetricLine/VolumetricLinePrefab", m_RemianTime / 1000.0f) as GameObject;
            IVolumetricLine lineComponent = line.GetComponent<IVolumetricLine>();
            if(null != lineComponent)
            {
                line.transform.position = Vector3.zero;
                line.transform.rotation = Quaternion.identity;

                lineComponent.StartPos = startPos;
                lineComponent.EndPos = endPos;
                lineComponent.LineWidth = m_Width;
            }

            return ExecResult.Finished;
        }

        private Vector3 m_StartPos;
        private Vector3 m_EndPos;
        private float m_Width;
        private long m_RemianTime;
    }
}
