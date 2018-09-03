using System;
using System.Collections.Generic;
using ScriptableData;
using UnityEngine;

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
            copy.m_AlwaysUpdateDirection = m_AlwaysUpdateDirection;
            copy.m_IsCurveMoving = true;
            return copy;
        }
        public void Load(int direction, params float[] args)
        {
            m_DirectionType = (DirectionType)direction;

            m_SectionList.Clear();
            for(int i = 0; i < args.Length; i+=7)
            {
                MoveSectionInfo section = new MoveSectionInfo();
                section.moveTime = args[i];
                section.speedVect.x = args[i + 1];
                section.speedVect.y = args[i + 2];
                section.speedVect.z = args[i + 3];
                section.accelVect.x = args[i + 4];
                section.accelVect.y = args[i + 5];
                section.accelVect.z = args[i + 6];
                m_SectionList.Add(section);
            }
            m_IsCurveMoving = true;
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
                        if(stCall.GetParamNum() >= 2)
                        {
                            m_AlwaysUpdateDirection = bool.Parse(stCall.GetParamId(1));
                        }
                    }
                }
            }
        }

        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (target == null)
            {
                return ExecResult.Finished;
            }
            GameEntity sender = Contexts.sharedInstance.game.GetEntityWithId(instance.SenderId);
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

            if (m_AlwaysUpdateDirection)
                m_RotateDir = UpdateRotation(instance, sender, target, m_DirectionType);


            m_ElapsedTime += delta;
            float realTime = m_ElapsedTime / 1000.0f;
            MoveSectionInfo cur_section = m_SectionListCopy[0];
            if (realTime - cur_section.startTime > cur_section.moveTime)
            {
                float end_time = cur_section.startTime + cur_section.moveTime;
                float used_time = end_time - cur_section.lastUpdateTime;
                cur_section.curSpeedVect = Move(instance, target, m_RotateDir, cur_section.curSpeedVect, cur_section.accelVect, used_time);
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
                cur_section.curSpeedVect = Move(instance, target, m_RotateDir, cur_section.curSpeedVect, cur_section.accelVect, realTime - cur_section.lastUpdateTime);
                cur_section.lastUpdateTime = realTime;
            }
            return ExecResult.Parallel;
        }
        protected override void ResetState()
        {
            m_IsCurveMoving = true;
            m_IsInited = false;
        }

        private void Init(GameEntity sender, GameEntity target, IInstance instance)
        {
            CopySectionList();
            m_ElapsedTime = 0;
            m_SectionListCopy[0].startTime = m_ElapsedTime;
            m_SectionListCopy[0].lastUpdateTime = m_ElapsedTime;
            m_SectionListCopy[0].curSpeedVect = m_SectionListCopy[0].speedVect;

            m_RotateDir = UpdateRotation(instance, sender, target, m_DirectionType);

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
        private static Vector3 Move(IInstance instance, GameEntity obj, float rotateDir, Vector3 speed_vect, Vector3 accel_vect, float time)
        {

            Vector3 speed = speed_vect + accel_vect * time / 2;
            Vector3 object_speed = Quaternion.Euler(0, Mathf.Rad2Deg * rotateDir, 0) * speed;
            instance.Velocity = object_speed;
            obj.ReplaceRotation(rotateDir);

            return (speed_vect + accel_vect * time);
        }
        private static float UpdateRotation(IInstance instance, GameEntity sender, GameEntity target, DirectionType directionType)
        {
            float rotateDir = 0;
            switch(directionType)
            {
                case DirectionType.Sender:
                    rotateDir = sender.rotation.Value;
                    break;
                case DirectionType.SenderTarget:
                    rotateDir = Mathf.Atan2(target.position.Value.x - sender.position.Value.x, target.position.Value.z - sender.position.Value.z);
                    break;
                case DirectionType.Target:
                    rotateDir = target.rotation.Value;
                    break;
                case DirectionType.TargetSender:
                    rotateDir = Mathf.Atan2(sender.position.Value.x - target.position.Value.x, sender.position.Value.z - target.position.Value.z);
                    break;
                case DirectionType.SenderOpposite:
                    rotateDir = sender.rotation.Value + Mathf.PI;
                    break;
            }
            return rotateDir;
        }

        private DirectionType m_DirectionType = DirectionType.Target;
        private bool m_AlwaysUpdateDirection = false;
        private List<MoveSectionInfo> m_SectionList = new List<MoveSectionInfo>();
        private List<MoveSectionInfo> m_SectionListCopy = new List<MoveSectionInfo>();
        private bool m_IsCurveMoving = false;
        private float m_RotateDir = 0;

        private bool m_IsInited = false;
        private float m_ElapsedTime;
    }

    public class DisableMoveInputCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if(null != target)
            {
                instance.DisableMoveInput = m_Value;
                target.ReplaceMovement(Vector3.zero);
            }
            return ExecResult.Finished;
        }
        public void Load(bool enable)
        {
            m_Value = enable;
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
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if(null != target)
            {
                instance.DisableRotationInput = m_Value;
            }
            return ExecResult.Finished;
        }
        public void Load(bool enable)
        {
            m_Value = enable;
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
