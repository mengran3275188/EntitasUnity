using System;
using Util;
using System.Collections.Generic;

namespace Skill.Commands
{
    public class MoveSectionInfo
    {
        public MoveSectionInfo Clone()
        {
            MoveSectionInfo copy = new MoveSectionInfo();
            copy.moveTime = moveTime;
            copy.speedVect = speedVect;
            copy.accelVect = accelVect;
            return copy;
        }

        public float moveTime;
        public Vector3 speedVect;
        public Vector3 accelVect;

        public float startTime = 0;
        public float lastUpdateTime = 0;
        public Vector3 curSpeedVect = Vector3.zero;
    }

    public class CurveMoveCommand : AbstractSkillCommand
    {
        public override ISkillCommand Clone()
        {
            CurveMoveCommand copy = new CurveMoveCommand();
            copy.m_IsLockRotate = m_IsLockRotate;
            copy.m_SectionList.AddRange(m_SectionList);
            copy.m_IsCurveMoving = true;
            return copy;
        }
        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() > 0)
            {
                m_IsLockRotate = bool.Parse(callData.GetParamId(0));
            }
            m_SectionList.Clear();
            int section_num = 0;
            while (callData.GetParamNum() >= 7 * (section_num + 1) + 1)
            {
                MoveSectionInfo section = new MoveSectionInfo();
                section.moveTime = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 1));
                section.speedVect.x = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 2));
                section.speedVect.y = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 3));
                section.speedVect.z = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 4));
                section.accelVect.x = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 5));
                section.accelVect.y = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 6));
                section.accelVect.z = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 7));
                m_SectionList.Add(section);
                section_num++;
            }
            if (m_SectionList.Count == 0)
            {
                return;
            }
            m_IsCurveMoving = true;
        }

        protected override ExecResult ExecCommand(SkillInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (obj == null)
            {
                return ExecResult.Finished;
            }
            if (!m_IsCurveMoving)
            {
                return ExecResult.Finished;
            }
            if (!m_IsInited)
            {
                Init(obj, instance);
            }
            if (m_SectionListCopy.Count == 0)
            {
                m_IsCurveMoving = false;
                return ExecResult.Finished;
            }

            m_ElapsedTime += delta;
            float realTime = m_ElapsedTime / 1000.0f;
            MoveSectionInfo cur_section = m_SectionListCopy[0];
            if (realTime - cur_section.startTime > cur_section.moveTime)
            {
                float end_time = cur_section.startTime + cur_section.moveTime;
                float used_time = end_time - cur_section.lastUpdateTime;
                cur_section.curSpeedVect = Move(obj, cur_section.curSpeedVect, cur_section.accelVect, used_time);
                m_SectionListCopy.RemoveAt(0);
                if (m_SectionListCopy.Count > 0)
                {
                    cur_section = m_SectionListCopy[0];
                    cur_section.startTime = end_time;
                    cur_section.lastUpdateTime = end_time;
                    cur_section.curSpeedVect = cur_section.speedVect;
                }
                else
                {
                    m_IsCurveMoving = false;
                }
            }
            else
            {
                cur_section.curSpeedVect = Move(obj, cur_section.curSpeedVect, cur_section.accelVect, realTime - cur_section.lastUpdateTime);
                cur_section.lastUpdateTime = realTime;
            }
            return ExecResult.Parallel;
        }

        private void Init(GameEntity obj, SkillInstance instance)
        {
            CopySectionList();
            m_ElapsedTime = 0;
            m_SectionListCopy[0].startTime = m_ElapsedTime;
            m_SectionListCopy[0].lastUpdateTime = m_ElapsedTime;
            m_SectionListCopy[0].curSpeedVect = m_SectionListCopy[0].speedVect;
            m_IsInited = true;
        }

        private void CopySectionList()
        {
            m_SectionListCopy.Clear();
            for (int i = 0; i < m_SectionList.Count; i++)
            {
                m_SectionListCopy.Add(m_SectionList[i].Clone());
            }
        }
        private Vector3 Move(GameEntity obj, Vector3 speed_vect, Vector3 accel_vect, float time)
        {
            if (!m_IsLockRotate)
            {
            }

            Vector3 local_motion = speed_vect * time + accel_vect * time * time / 2;
            Vector3 object_motion = Quaternion.CreateFromYawPitchRoll(obj.rotation.RotateDir, 0, 0) * local_motion;
            Vector3 word_target_pos = new Vector3(obj.position.x + object_motion.x, obj.position.y + object_motion.y, obj.position.z + object_motion.z);
            obj.ReplacePosition(word_target_pos.x, word_target_pos.y, word_target_pos.z);
            return (speed_vect + accel_vect * time);
        }

        private bool m_IsLockRotate = true;
        private List<MoveSectionInfo> m_SectionList = new List<MoveSectionInfo>();
        private List<MoveSectionInfo> m_SectionListCopy = new List<MoveSectionInfo>();
        private bool m_IsCurveMoving = false;

        private bool m_IsInited = false;
        private float m_ElapsedTime;
        private static float m_MaxMoveStep = 3;
    }
}
