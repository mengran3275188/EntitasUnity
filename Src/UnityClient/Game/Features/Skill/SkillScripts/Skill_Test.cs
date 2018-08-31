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
                Cmd.DisableMoveInputCmd(instance, deltaTime, true);
                Cmd.DisableRotationInputCmd(instance, deltaTime, true);
                Cmd.Animation(instance, deltaTime, "s03_yunshi00_01");
                Cmd.Effect(instance, deltaTime, "Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_01", 1400, "", false, new Vector3(0, 0, 5));
            }
            if(ShouldCall(100, time, deltaTime))
            {
                Cmd.Effect(instance, deltaTime, "Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_03", 4500, "", false, new Vector3(0, 0, 5));
            }
            if(ShouldCall(1100, time, deltaTime))
            {
                Cmd.Effect(instance, deltaTime, "Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_02", 3000, "", false, new Vector3(0, 0, 5));
                Cmd.Terminate(instance, deltaTime);
            }
        }
    }
}
