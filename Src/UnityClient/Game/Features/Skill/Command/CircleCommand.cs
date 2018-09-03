using System;
using UnityEngine;
using System.Collections.Generic;
using ScriptableData;

namespace SkillCommands
{
    public class CircleCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            CircleCommand copy = new CircleCommand();
            copy.m_SectionList.AddRange(m_SectionList);
            copy.m_StartPos = m_StartPos;
            return copy;
        }
        public void Load(float startDistance, float startAngle, params float[] args)
        {
            m_StartPos = new Vector3(startDistance, 0, startAngle);

            for(int i = 0; i < args.Length; i+=5)
            {
                MoveSectionInfo section = new MoveSectionInfo();
                section.moveTime = args[i];
                section.speedVect.x = args[i + 1];
                section.speedVect.z = args[i + 2];
                section.accelVect.x = args[i + 3];
                section.accelVect.z = args[i + 4];
                m_SectionList.Add(section);
            }
        }
        protected override void Load(CallData callData)
        {
            int sectionNum = 0;
            if(callData.GetParamNum() >= 2)
            {
                float startDistance = (float)System.Convert.ToDouble(callData.GetParamId(0));
                float startAngle = (float)System.Convert.ToDouble(callData.GetParamId(1));

                m_StartPos = new Vector3(startDistance, 0, startAngle);
            }


            while (callData.GetParamNum() >= 5 * (sectionNum + 1) + 2)
            {
                MoveSectionInfo section = new MoveSectionInfo();
                section.moveTime = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 2));
                section.speedVect.x = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 3));
                section.speedVect.z = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 4));
                section.accelVect.x = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 5));
                section.accelVect.z = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 6));
                m_SectionList.Add(section);
                sectionNum++;
            }
        }
        protected override void ResetState()
        {
            m_IsInited = false;
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (target == null)
            {
                return ExecResult.Finished;
            }
            GameEntity sender = Contexts.sharedInstance.game.GetEntityWithId(instance.SenderId);
            if (sender == null)
            {
                return ExecResult.Finished;
            }
            if (!m_IsInited)
            {
                Init(sender, target, instance);
            }
            if (m_SectionListCopy.Count == 0)
            {
                m_IsCurveMoving = false;
                instance.Velocity = Vector3.zero;
                return ExecResult.Finished;
            }


            m_ElapsedTime += delta;
            float realTime = m_ElapsedTime / 1000.0f;
            float remainTime = realTime;
            Vector3 distance = Vector3.zero;
            for (int i = 0; i < m_SectionListCopy.Count; ++i)
            {
                if (remainTime > m_SectionListCopy[i].moveTime)
                {
                    distance += m_SectionListCopy[i].speedVect * m_SectionListCopy[i].moveTime + 0.5f * m_SectionListCopy[i].accelVect * m_SectionListCopy[i].moveTime * m_SectionListCopy[i].moveTime;
                    remainTime -= m_SectionListCopy[i].moveTime;

                    if (i == m_SectionListCopy.Count - 1)
                        return ExecResult.Finished;
                }
                else
                {
                    distance += m_SectionListCopy[i].speedVect * remainTime + 0.5f * m_SectionListCopy[i].accelVect * remainTime * remainTime;
                    break;
                }
            }

            Move(instance, sender, target, m_StartPos, distance);
            return ExecResult.Parallel;
        }


        private void Init(GameEntity sender, GameEntity target, IInstance instance)
        {
            CopySectionList();
            m_ElapsedTime = 0;

            m_IsInited = true;
        }
        private void CopySectionList()
        {
            m_SectionListCopy.Clear();
            for (int i = 0; i < m_SectionList.Count; ++i)
            {
                m_SectionListCopy.Add(m_SectionList[i].Clone());
            }
        }
        private void Move(IInstance instance, GameEntity sender, GameEntity target, Vector3 lastPos, Vector3 distance)
        {
            Vector3 nowPos = lastPos + distance;

            Vector3 centerPos = sender.position.Value;

            Vector3 targetPos = centerPos + Quaternion.Euler(0, nowPos.z, 0) * (Vector3.forward * nowPos.x);

            target.ReplaceRotation((targetPos - target.physics.Rigid.Position).ToDir());
            target.physics.Rigid.Position = targetPos;
        }

        private Vector3 m_StartPos = Vector3.zero;
        private float m_ElapsedTime = 0;

        private List<MoveSectionInfo> m_SectionList = new List<MoveSectionInfo>();
        private List<MoveSectionInfo> m_SectionListCopy = new List<MoveSectionInfo>();

        private bool m_IsCurveMoving = false;
        private bool m_IsInited = false;
    }
}
