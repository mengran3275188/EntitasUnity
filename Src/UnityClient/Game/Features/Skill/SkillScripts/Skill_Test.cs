using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptableSystem;
using ScriptableSystem.CommonCommands;
using SkillCommands;
using UnityEngine;

namespace SkillScripts
{
    public sealed class Skill_Test : ScriptableSystem.CSharpInstance
    {
        public Skill_Test()
        {
            AddMessageHandler("start", Message_Start);
        }
        public static int GetId()
        {
            return 100 * GetSkillId();
        }
        private static int GetSkillId()
        {
            return 900;
        }
        private static void Message_Start(CSharpInstance instance, long time, long deltaTime)
        {
            if(ShouldCall(0, time, deltaTime))
            {
                Util.LogUtil.Debug("OnStart {0},  time {1} deltaTime {2}.", instance.Id, time, deltaTime);
                DisableMoveInputCmd(instance, deltaTime, true);
                DisableRotationInputCmd(instance, deltaTime, true);
                Animation(instance, deltaTime, "s03_yunshi00_01");
                Effect(instance, deltaTime, "Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_01", 1400, "", false, new Vector3(0, 0, 5));
            }
            if(ShouldCall(100, time, deltaTime))
            {
                Effect(instance, deltaTime, "Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_03", 4500, "", false, new Vector3(0, 0, 5));
            }
            if(ShouldCall(1100, time, deltaTime))
            {
                Effect(instance, deltaTime, "Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_02", 3000, "", false, new Vector3(0, 0, 5));
                Terminate(instance, deltaTime);
            }
        }
        private static void Animation(IInstance instance, long deltaTime, string animName)
        {
            AnimationCommand cmd = CommandManager.Instance.CreateCommand("animation") as AnimationCommand;
            cmd.Load(animName);

            cmd.Execute(instance, deltaTime);
        }
        private static void DisableMoveInputCmd(IInstance instance, long deltaTime, bool enable)
        {
            DisableMoveInputCommand cmd = CommandManager.Instance.CreateCommand("disablemoveinput") as DisableMoveInputCommand;
            cmd.Load(enable);
            cmd.Execute(instance, deltaTime);
        }
        private static void DisableRotationInputCmd(IInstance instance, long deltaTime, bool enable)
        {
            DisableRotationInputCommand cmd = CommandManager.Instance.CreateCommand("disablerotationinput") as DisableRotationInputCommand;
            cmd.Load(enable);
            cmd.Execute(instance, deltaTime);
        }
        private static void Effect(IInstance instance, long deltaTime, string path, float deleteTime, string attachPath, bool isAttach, Vector3 position)
        {
            EffectCommand cmd = CommandManager.Instance.CreateCommand("effect") as EffectCommand;
            cmd.Load(path, deleteTime, attachPath, isAttach, position);
            cmd.Execute(instance, deltaTime);
        }
        private static void Terminate(IInstance instance, long deltaTime)
        {
            TerminateCommand cmd = CommandManager.Instance.CreateCommand("terminate") as TerminateCommand;
            cmd.Execute(instance, deltaTime);
        }
        private static void BreakSection(IInstance instance, long deltaTime, bool enable)
        {
        }
        private static void Effect(IInstance instance)
        {
        }
    }
}
