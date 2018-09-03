using System;
using System.Collections.Generic;
using ScriptableData;
using ScriptableData.CommonCommands;
using SkillCommands;
using SceneCommand;
using UnityEngine;
using UnityClient;

namespace ScriptableData.SkillScripts
{
    public enum DirectionType
    {
        Target = 0,
        SenderTarget = 1,
        Sender = 2,
        TargetSender = 3,
        SenderOpposite = 4,
    }
    public interface ISkillScript
    {
        IInstance GetSkillInstance();
    }
    public abstract class BaseSkillScript : ISkillScript
    {
        public IInstance GetSkillInstance()
        {
            CSharpInstance instance = new CSharpInstance();
            instance.AddMessageHandler("start", this.Start);
            instance.AddMessageHandler("onbreak", this.Break);
            instance.AddMessageHandler("oncollider", this.Collide);
            return instance;
        }

        private void Start(CSharpInstance instance, long curTime, long deltaTime)
        {
            Enter(instance, curTime, deltaTime);
            StartHandler();
            Leave();
        }
        private void Break(CSharpInstance instance, long curTime, long deltaTime)
        {
            Enter(instance, curTime, deltaTime);
            BreakHandler();
            Leave();
        }
        private void Collide(CSharpInstance instance, long curTime, long deltaTime)
        {
            Enter(instance, curTime, deltaTime);
            CollideHandler();
            Leave();
        }
        protected virtual void StartHandler() { }
        protected virtual void BreakHandler() { }
        protected virtual void CollideHandler() { }
        protected bool ShouldCall(long callTime)
        {
            return m_CurTime > callTime && callTime >= m_CurTime - m_DeltaTime;
        }
        private void Enter(CSharpInstance instance, long curTime, long deltaTime)
        {
            m_Instance = instance;
            m_CurTime = curTime;
            m_DeltaTime = deltaTime;
        }
        private void Leave()
        {
            m_Instance = null;
            m_CurTime = 0;
            m_DeltaTime = 0;
        }


        // helper
        internal void Terminate()
        {
            TerminateCommand cmd = CommandManager.Instance.CreateCommand("terminate") as TerminateCommand;
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal static void Log(string format, params object[] args)
        {
            Util.LogUtil.Info(format, args);
        }
        internal void Animation(string animName)
        {
            AnimationCommand cmd = CommandManager.Instance.CreateCommand("animation") as AnimationCommand;
            cmd.Load(animName);

            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void AnimationSpeed(string animName, float speed)
        {
            AnimationSpeedCommand cmd = CommandManager.Instance.CreateCommand("animationspeed") as AnimationSpeedCommand;
            cmd.Load(animName, speed);

            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void Effect(string path, float deleteTime, string attachPath, bool isAttach, Vector3 position)
        {
            EffectCommand cmd = CommandManager.Instance.CreateCommand("effect") as EffectCommand;
            cmd.Load(path, deleteTime, attachPath, isAttach, position);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void LineEffect(Vector3 startPos, Vector3 endPos, float width, long remainTime)
        {
            VolumetricLineCommand cmd = CommandManager.Instance.CreateCommand("linefeect") as VolumetricLineCommand;
            cmd.Load(startPos, endPos, width, remainTime);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void Curvemove(DirectionType direction, params float[] args)
        {
            CurveMoveCommand cmd = CommandManager.Instance.CreateCommand("curvemove") as CurveMoveCommand;
            cmd.Load((int)direction, args);
            if(cmd.Execute(m_Instance, m_DeltaTime) == ExecResult.Parallel)
            {
                m_Instance.ParallelCommands.Add(cmd);
            }
        }
        internal void Circlemove(float startDistance, float startAngle, params float[] args)
        {
            CircleCommand cmd = CommandManager.Instance.CreateCommand("circlemove") as CircleCommand;
            cmd.Load(startDistance, startAngle, args);
            if(cmd.Execute(m_Instance, m_DeltaTime) == ExecResult.Parallel)
            {
                m_Instance.ParallelCommands.Add(cmd);
            }
        }
        internal void Physicsmove(Vector3 speed)
        {
            PhysicsMoveCommand cmd = CommandManager.Instance.CreateCommand("physicsmove") as PhysicsMoveCommand;
            cmd.Load(speed);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void AreaDamage(Vector3 offset, float radius, params StateBuff[] stateBuffs)
        {
            AreaDamageCommand cmd = CommandManager.Instance.CreateCommand("areadamage") as AreaDamageCommand;
            cmd.Load(offset, radius, stateBuffs);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void ColliderDamage_BoxShape(int layer, long remainTime, long damageInterval, Vector3 offset, Vector3 size, params StateBuff[] stateBuffs) {
            ColliderDamageCommand cmd = CommandManager.Instance.CreateCommand("colliderdamage") as ColliderDamageCommand;
            cmd.Load(layer, remainTime, damageInterval, offset, size, stateBuffs);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void ColliderDamage_LineShape(int layer, long remainTime, long damageInterval, Vector3 startPos, Vector3 endPos, float radius, params StateBuff[] stateBuffs) {
            ColliderDamageCommand cmd = CommandManager.Instance.CreateCommand("colliderdamage") as ColliderDamageCommand;
            cmd.Load(layer, remainTime, damageInterval, startPos, endPos, radius, stateBuffs);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void FindTarget(Vector3 offset, float radius, string retValueName)
        {
            FindTargetCommand cmd = CommandManager.Instance.CreateCommand("findtarget") as FindTargetCommand;
            cmd.Load(offset, radius, retValueName);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void Children(string retValueName)
        {
            ChildrenCommand cmd = CommandManager.Instance.CreateCommand("children") as ChildrenCommand;
            cmd.Load(retValueName);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void ChangeLayer(string layerName)
        {
            ChangeLayerCommand cmd = CommandManager.Instance.CreateCommand("changelayer") as ChangeLayerCommand;
            cmd.Load(layerName);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void MoveChild(string childName, string nodeName)
        {
            MoveChildTrigger cmd = CommandManager.Instance.CreateCommand("movechild") as MoveChildTrigger;
            cmd.Load(childName, nodeName);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void CreateCharacter(int characterId, Vector3 position, Vector3 rotation, bool isMainPlayer = false, int skillId = -1)
        {
            CreateCharacterCommand cmd = CommandManager.Instance.CreateCommand("createcharacter") as CreateCharacterCommand;
            cmd.Load(characterId, position, rotation, isMainPlayer, skillId);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void Visible(bool visible)
        {
            VisibleCommand cmd = CommandManager.Instance.CreateCommand("visible") as VisibleCommand;
            cmd.Load(visible);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void Skill(uint entityId, int skillId)
        {
            SkillCommand cmd = CommandManager.Instance.CreateCommand("skill") as SkillCommand;
            cmd.Load(entityId, skillId);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void Camp(uint entityId, int campId)
        {
            CSharpCampCommand cmd = CommandManager.Instance.CreateCommand("csharpcamp") as CSharpCampCommand;
            cmd.Load(entityId, campId);
            cmd.Execute(m_Instance, m_DeltaTime);
        }

        internal void DisableMoveInputCmd(bool enable)
        {
            DisableMoveInputCommand cmd = CommandManager.Instance.CreateCommand("disablemoveinput") as DisableMoveInputCommand;
            cmd.Load(enable);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void DisableRotationInputCmd(bool enable)
        {
            DisableRotationInputCommand cmd = CommandManager.Instance.CreateCommand("disablerotationinput") as DisableRotationInputCommand;
            cmd.Load(enable);
            cmd.Execute(m_Instance, m_DeltaTime);
        }
        internal void BreakSection(int breakType, long startTime, long endTime)
        {
            BreakSectionCommand cmd = CommandManager.Instance.CreateCommand("breaksection") as BreakSectionCommand;
            cmd.Load(breakType, startTime, endTime);
            cmd.Execute(m_Instance, m_DeltaTime);
        }

        private CSharpInstance m_Instance;
        private long m_CurTime = 0;
        private long m_DeltaTime = 0;
    }
}
