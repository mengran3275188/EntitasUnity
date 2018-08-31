using System;
using System.Collections.Generic;
using ScriptableSystem;
using ScriptableSystem.CommonCommands;
using SkillCommands;
using SceneCommand;
using UnityEngine;
using UnityClient;

namespace SkillScripts
{
    public enum DirectionType
    {
        Target = 0,
        SenderTarget = 1,
        Sender = 2,
        TargetSender = 3,
        SenderOpposite = 4,
    }
    public static class Cmd
    {
        internal static void Terminate(IInstance instance, long deltaTime)
        {
            TerminateCommand cmd = CommandManager.Instance.CreateCommand("terminate") as TerminateCommand;
            cmd.Execute(instance, deltaTime);
        }
        internal static void Log(string format, params object[] args)
        {
            Util.LogUtil.Info(format, args);
        }
        internal static void Animation(IInstance instance, long deltaTime, string animName)
        {
            AnimationCommand cmd = CommandManager.Instance.CreateCommand("animation") as AnimationCommand;
            cmd.Load(animName);

            cmd.Execute(instance, deltaTime);
        }
        internal static void AnimationSpeed(IInstance instance, long deltaTime, string animName, float speed)
        {
            AnimationSpeedCommand cmd = CommandManager.Instance.CreateCommand("animationspeed") as AnimationSpeedCommand;
            cmd.Load(animName, speed);

            cmd.Execute(instance, deltaTime);
        }
        internal static void Effect(IInstance instance, long deltaTime, string path, float deleteTime, string attachPath, bool isAttach, Vector3 position)
        {
            EffectCommand cmd = CommandManager.Instance.CreateCommand("effect") as EffectCommand;
            cmd.Load(path, deleteTime, attachPath, isAttach, position);
            cmd.Execute(instance, deltaTime);
        }
        internal static void LineEffect(IInstance instance, long deltaTime, Vector3 startPos, Vector3 endPos, float width, long remainTime)
        {
            VolumetricLineCommand cmd = CommandManager.Instance.CreateCommand("linefeect") as VolumetricLineCommand;
            cmd.Load(startPos, endPos, width, remainTime);
            cmd.Execute(instance, deltaTime);
        }
        internal static void Curvemove(IInstance instance, long deltaTime, DirectionType direction, params float[] args)
        {
            CurveMoveCommand cmd = CommandManager.Instance.CreateCommand("curvemove") as CurveMoveCommand;
            cmd.Load((int)direction, args);
            if(cmd.Execute(instance, deltaTime) == ExecResult.Parallel)
            {
                instance.ParallelCommands.Add(cmd);
            }
        }
        internal static void Circlemove(IInstance instance, long deltaTime, float startDistance, float startAngle, params float[] args)
        {
            CircleCommand cmd = CommandManager.Instance.CreateCommand("circlemove") as CircleCommand;
            cmd.Load(startDistance, startAngle, args);
            if(cmd.Execute(instance, deltaTime) == ExecResult.Parallel)
            {
                instance.ParallelCommands.Add(cmd);
            }
        }
        internal static void Physicsmove(IInstance instance, long deltaTime, Vector3 speed)
        {
            PhysicsMoveCommand cmd = CommandManager.Instance.CreateCommand("physicsmove") as PhysicsMoveCommand;
            cmd.Load(speed);
            cmd.Execute(instance, deltaTime);
        }
        internal static void AreaDamage(IInstance instance, long deltaTime, Vector3 offset, float radius, params StateBuff[] stateBuffs)
        {
            AreaDamageCommand cmd = CommandManager.Instance.CreateCommand("areadamage") as AreaDamageCommand;
            cmd.Load(offset, radius, stateBuffs);
            cmd.Execute(instance, deltaTime);
        }
        internal static void ColliderDamage_BoxShape(IInstance instance, long deltaTime, int layer, long remainTime, long damageInterval, Vector3 offset, Vector3 size, params StateBuff[] stateBuffs) {
            ColliderDamageCommand cmd = CommandManager.Instance.CreateCommand("colliderdamage") as ColliderDamageCommand;
            cmd.Load(layer, remainTime, damageInterval, offset, size, stateBuffs);
            cmd.Execute(instance, deltaTime);
        }
        internal static void ColliderDamage_LineShape(IInstance instance, long deltaTime, int layer, long remainTime, long damageInterval, Vector3 startPos, Vector3 endPos, float radius, params StateBuff[] stateBuffs) {
            ColliderDamageCommand cmd = CommandManager.Instance.CreateCommand("colliderdamage") as ColliderDamageCommand;
            cmd.Load(layer, remainTime, damageInterval, startPos, endPos, radius, stateBuffs);
            cmd.Execute(instance, deltaTime);
        }
        internal static void FindTarget(IInstance instance, long deltaTime, Vector3 offset, float radius, string retValueName)
        {
            FindTargetCommand cmd = CommandManager.Instance.CreateCommand("findtarget") as FindTargetCommand;
            cmd.Load(offset, radius, retValueName);
            cmd.Execute(instance, deltaTime);
        }
        internal static void Children(IInstance instance, long deltaTime, string retValueName)
        {
            ChildrenCommand cmd = CommandManager.Instance.CreateCommand("children") as ChildrenCommand;
            cmd.Load(retValueName);
            cmd.Execute(instance, deltaTime);
        }
        internal static void ChangeLayer(IInstance instance, long deltaTime, string layerName)
        {
            ChangeLayerCommand cmd = CommandManager.Instance.CreateCommand("changelayer") as ChangeLayerCommand;
            cmd.Load(layerName);
            cmd.Execute(instance, deltaTime);
        }
        internal static void MoveChild(IInstance instance, long deltaTime, string childName, string nodeName)
        {
            MoveChildTrigger cmd = CommandManager.Instance.CreateCommand("movechild") as MoveChildTrigger;
            cmd.Load(childName, nodeName);
            cmd.Execute(instance, deltaTime);
        }
        internal static void CreateCharacter(IInstance instance, long deltaTime, int characterId, Vector3 position, Vector3 rotation, bool isMainPlayer = false, int skillId = -1)
        {
            CreateCharacterCommand cmd = CommandManager.Instance.CreateCommand("createcharacter") as CreateCharacterCommand;
            cmd.Load(characterId, position, rotation, isMainPlayer, skillId);
            cmd.Execute(instance, deltaTime);
        }
        internal static void Visible(IInstance instance, long deltaTime, bool visible)
        {
            VisibleCommand cmd = CommandManager.Instance.CreateCommand("visible") as VisibleCommand;
            cmd.Load(visible);
            cmd.Execute(instance, deltaTime);
        }
        internal static void Skill(IInstance instance, long deltaTime, uint entityId, int skillId)
        {
            SkillCommand cmd = CommandManager.Instance.CreateCommand("skill") as SkillCommand;
            cmd.Load(entityId, skillId);
            cmd.Execute(instance, deltaTime);
        }
        internal static void Camp(IInstance instance, long deltaTime, uint entityId, int campId)
        {
            CSharpCampCommand cmd = CommandManager.Instance.CreateCommand("csharpcamp") as CSharpCampCommand;
            cmd.Load(entityId, campId);
            cmd.Execute(instance, deltaTime);
        }

        internal static void DisableMoveInputCmd(IInstance instance, long deltaTime, bool enable)
        {
            DisableMoveInputCommand cmd = CommandManager.Instance.CreateCommand("disablemoveinput") as DisableMoveInputCommand;
            cmd.Load(enable);
            cmd.Execute(instance, deltaTime);
        }
        internal static void DisableRotationInputCmd(IInstance instance, long deltaTime, bool enable)
        {
            DisableRotationInputCommand cmd = CommandManager.Instance.CreateCommand("disablerotationinput") as DisableRotationInputCommand;
            cmd.Load(enable);
            cmd.Execute(instance, deltaTime);
        }
        internal static void BreakSection(IInstance instance, long deltaTime, bool enable)
        {
        }
    }
}
