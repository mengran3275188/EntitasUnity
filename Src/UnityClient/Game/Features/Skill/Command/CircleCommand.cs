using System;
using UnityEngine;
using System.Collections.Generic;
using ScriptableData;
using ScriptableSystem;

namespace SkillCommands
{
    public class CircleCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            CircleCommand copy = new CircleCommand();
            copy.m_SectionList.AddRange(m_SectionList);
            return copy;
        }
        protected override void Load(CallData callData)
        {
            int sectionNum = 0;
            while (callData.GetParamNum() >= 5 * (sectionNum + 1) + 1)
            {
                MoveSectionInfo section = new MoveSectionInfo();
                section.moveTime = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 1));
                section.speedVect.x = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 2));
                section.speedVect.z = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 3));
                section.accelVect.x = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 4));
                section.accelVect.x = (float)System.Convert.ToDouble(callData.GetParamId(sectionNum * 5 + 5));
                m_SectionList.Add(section);
                sectionNum++;
            }
        }
        protected override void ResetState()
        {
            m_IsInited = false;
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
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


            m_ElapsedTime = delta;
            float realTime = m_ElapsedTime / 1000.0f;
            MoveSectionInfo curSection = m_SectionListCopy[0];
            if (realTime - curSection.startTime > curSection.moveTime)
            {
                float endTime = curSection.startTime + curSection.moveTime;
                float usedTime = endTime - curSection.lastUpdateTime;
                curSection.curSpeedVect = Move(instance, sender, target, m_LastPos, curSection.curSpeedVect, curSection.accelVect, usedTime);
                m_SectionListCopy.RemoveAt(0);
                if (m_SectionListCopy.Count > 0)
                {
                    curSection = m_SectionListCopy[0];
                    curSection.startTime = endTime;
                    curSection.lastUpdateTime = endTime;
                    curSection.curSpeedVect = curSection.speedVect;
                }
                else
                {
                    m_IsCurveMoving = false;
                }
            }
            else
            {
                curSection.curSpeedVect = Move(instance, sender, target, m_LastPos, curSection.curSpeedVect, curSection.accelVect, realTime - curSection.lastUpdateTime);
                curSection.lastUpdateTime = realTime;
            }
            return ExecResult.Parallel;
        }


        private void Init(GameEntity sender, GameEntity target, Instance instance)
        {
            CopySectionList();
            m_ElapsedTime = 0;
            m_SectionListCopy[0].startTime = m_ElapsedTime;
            m_SectionListCopy[0].lastUpdateTime = m_ElapsedTime;
            m_SectionListCopy[0].curSpeedVect = m_SectionListCopy[0].speedVect;

            m_LastPos = CalculatePos(sender, target);

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
        private Vector3 Move(Instance instance, GameEntity sender, GameEntity target, Vector3 lastPos, Vector3 speed_vect, Vector3 accel_vect, float time)
        {
            if (time > 0)
            {
                Vector3 nowPos = lastPos + (speed_vect + accel_vect * time / 2) * time;

                m_LastPos = nowPos;

                Vector3 centerPos = sender.position.Value;

                Vector3 targetPos = centerPos + Quaternion.Euler(0, nowPos.z, 0) * (Vector3.forward * nowPos.x);

                Vector3 speed = (targetPos - target.position.Value) / time;
                speed.y = 0;

                instance.Velocity = speed;
            }

            return (speed_vect + accel_vect * time);
        }
        private static Vector3 CalculatePos(GameEntity sender, GameEntity target)
        {
            float distance = sender.position.Value.DistanceXZ(target.position.Value);
            float rotateAngle = Mathf.Rad2Deg * Mathf.Atan2(target.position.Value.x - sender.position.Value.x, target.position.Value.z - sender.position.Value.z);

            return new Vector3(distance, 0, rotateAngle);
        }

        private Vector3 m_LastPos = Vector3.zero;
        private float m_ElapsedTime = 0;

        private List<MoveSectionInfo> m_SectionList = new List<MoveSectionInfo>();
        private List<MoveSectionInfo> m_SectionListCopy = new List<MoveSectionInfo>();

        private bool m_IsCurveMoving = false;
        private bool m_IsInited = false;
    }
}
