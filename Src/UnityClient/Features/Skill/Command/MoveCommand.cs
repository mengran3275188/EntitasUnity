using System;
using Util;
using System.Collections.Generic;
using ScriptableSystem;
using ScriptableData;

namespace SkillCommands
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

    public class CurveMoveCommand : AbstractCommand
    {
        private enum DirectionType
        {
            Target,
            SenderTarget,
            Sender,                 // 通常用在召唤物技能
            TargetSender,
            SenderOpposite,
        }
        public override ICommand Clone()
        {
            CurveMoveCommand copy = new CurveMoveCommand();
            copy.m_SectionList.AddRange(m_SectionList);
            copy.m_DirectionType = m_DirectionType;
            copy.m_IsCurveMoving = true;
            return copy;
        }
        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() > 0)
            {
               // m_IsLockRotate = bool.Parse(callData.GetParamId(0));
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
        protected override void Load(FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if(null != callData)
            {
                Load(callData);
                foreach(ScriptableData.ISyntaxComponent statement in funcData.Statements)
                {
                    ScriptableData.CallData stCall = statement as ScriptableData.CallData;
                    string id = stCall.GetId();
                    string param = stCall.GetParamId(0);

                    if(id == "direction")
                    {
                        if(stCall.GetParamNum() >= 1)
                        {
                            m_DirectionType = (DirectionType)int.Parse(stCall.GetParamId(0));
                        }
                    }
                }
            }
        }

        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (target == null)
            {
                return ExecResult.Finished;
            }
            GameEntity sender = instance.Sender as GameEntity;
            if(sender == null)
            {
                return ExecResult.Finished;
            }
            if (!m_IsCurveMoving)
            {
                instance.Velocity = Vector3.zero;
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
            MoveSectionInfo cur_section = m_SectionListCopy[0];
            if (realTime - cur_section.startTime > cur_section.moveTime)
            {
                float end_time = cur_section.startTime + cur_section.moveTime;
                float used_time = end_time - cur_section.lastUpdateTime;
                cur_section.curSpeedVect = Move(instance, target, cur_section.curSpeedVect, cur_section.accelVect, used_time);
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
                cur_section.curSpeedVect = Move(instance, target, cur_section.curSpeedVect, cur_section.accelVect, realTime - cur_section.lastUpdateTime);
                cur_section.lastUpdateTime = realTime;
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

            switch(m_DirectionType)
            {
                case DirectionType.Sender:
                    m_RotateDir = instance.SenderDirection;
                    break;
                case DirectionType.SenderTarget:
                    m_RotateDir = Util.Mathf.Atan2(target.position.Value.x - instance.SenderPosition.x, target.position.Value.z - instance.SenderPosition.z);
                    break;
                case DirectionType.Target:
                    m_RotateDir = target.rotation.Value;
                    break;
                case DirectionType.TargetSender:
                    m_RotateDir = Util.Mathf.Atan2(instance.SenderPosition.x - target.position.Value.x, instance.SenderPosition.z - target.position.Value.z);
                    break;
                case DirectionType.SenderOpposite:
                    m_RotateDir = instance.SenderDirection + Mathf.PI;
                    break;
            }

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
        private Vector3 Move(Instance instance, GameEntity obj, Vector3 speed_vect, Vector3 accel_vect, float time)
        {

            Vector3 speed = speed_vect + accel_vect * time / 2;
            Vector3 object_speed = Quaternion.CreateFromYawPitchRoll(m_RotateDir, 0, 0) * speed;
            instance.Velocity = object_speed;
            obj.ReplaceRotation(m_RotateDir);

            /*
            Vector3 local_motion = speed_vect * time + accel_vect * time * time / 2;
            Vector3 object_motion = Quaternion.CreateFromYawPitchRoll(m_RotateDir, 0, 0) * local_motion;
            Vector3 word_target_pos = obj.position.Value + object_motion;
            obj.ReplacePosition(word_target_pos);
            obj.ReplaceRotation(Entitas.Data.RotateState.SkillRotate, m_RotateDir);
            */
            return (speed_vect + accel_vect * time);
        }

        private DirectionType m_DirectionType = DirectionType.Target;
        private List<MoveSectionInfo> m_SectionList = new List<MoveSectionInfo>();
        private List<MoveSectionInfo> m_SectionListCopy = new List<MoveSectionInfo>();
        private bool m_IsCurveMoving = false;
        private float m_RotateDir = 0;

        private bool m_IsInited = false;
        private float m_ElapsedTime;
    }

    public class DisableMoveInputCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if(null != target)
            {
                instance.DisableMoveInput = m_Value;
                target.ReplaceMovement(Vector3.zero);
            }
            return ExecResult.Finished;
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 1)
            {
                m_Value = bool.Parse(callData.GetParamId(0));
            }
        }
        private bool m_Value;
    }

    public class DisableRotationInputCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if(null != target)
            {
                instance.DisableRotationInput = m_Value;
            }
            return ExecResult.Finished;
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 1)
            {
                m_Value = bool.Parse(callData.GetParamId(0));
            }
        }
        private bool m_Value;
    }
}
