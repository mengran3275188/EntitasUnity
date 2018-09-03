using UnityEngine;

namespace ScriptableData.SkillScripts
{
    public sealed class Skill_Test : BaseSkillScript
    {
        protected override void StartHandler()
        {
            if(ShouldCall(0))
            {
                DisableMoveInputCmd(true);
                DisableRotationInputCmd(true);
                Animation("s03_yunshi00_01");
                Effect("Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_01", 1400, "", false, new Vector3(0, 0, 5));
            }
            if(ShouldCall(100))
            {
                Effect("Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_03", 4500, "", false, new Vector3(0, 0, 5));
            }
            if(ShouldCall(1100))
            {
                Effect("Hero_FX/3_Wizard/3_hero_Wizard_s03_yunshi00_02", 3000, "", false, new Vector3(0, 0, 5));
                Terminate();
            }
        }
    }
}
