using UnityEngine;
using UnityClient;

namespace ScriptableData.SkillScripts
{
    internal class Skill_PuGong_1 : BaseSkillScript
    {
        protected override void StartHandler()
        {
            if(ShouldCall(0))
            {
                BreakSection(102, 0, 300);
                BreakSection(102, 450, 1200);
                BreakSection(101, 500, 1200);
                BreakSection(1, 500, 1000);

                DisableMoveInputCmd(true);
                DisableRotationInputCmd(true);
                Animation("attack_001");
                AnimationSpeed("attack_001", 5);
            }
            if(ShouldCall(100))
            {
                AnimationSpeed("attack_001", 0.67f);
            }
            if(ShouldCall(350))
            {
                AnimationSpeed("attack_001", 2);
                Curvemove(DirectionType.Target, 0.067f, 0, 0, 6, 0, 0, 90);
                Effect("Monster_FX/Campaign_1/6_Npc_Private_Attack_01", 3000, "", false, Vector3.zero);
            }
            if(ShouldCall(383))
            {
                AreaDamage(new Vector3(0, 1.7f, 1.5f), 2.5f, new StateBuff { m_State = StateBuff_State.Default, m_Buffs = { 1 } },
                                                             new StateBuff { m_State = StateBuff_State.Skill, m_Buffs = { 3 } } );
            }
            if(ShouldCall(450))
            {
                AnimationSpeed("attack_001", 1);
            }
            if(ShouldCall(1200))
            {
                Terminate();
            }
        }
        protected override void BreakHandler()
        {
            Log("onbreak");
            Terminate();
        }
    }

    internal class Skill_PuGong_101 : BaseSkillScript
    {
        protected override void StartHandler()
        {
            if(ShouldCall(0))
            {
                BreakSection(102, 0, 350);
                BreakSection(102, 533, 1033);
                BreakSection(101, 583, 1033);
                BreakSection(1, 583, 1033);

                DisableMoveInputCmd(true);
                DisableRotationInputCmd(true);
                Animation("attack_002");
                AnimationSpeed("attack_002", 5);
            }
            if(ShouldCall(100))
            {
                AnimationSpeed("attack_002", 1);
            }
            if(ShouldCall(400))
            {
                AnimationSpeed("attack_002", 2);
                Curvemove(DirectionType.Target, 0.067f, 0, 0, 6, 0, 0, 90);
                Effect("Monster_FX/Campaign_1/6_Npc_Private_Attack_02", 3000, "", false, Vector3.zero);
            }
            if(ShouldCall(433))
            {
                AreaDamage(new Vector3(0, 1.7f, 1.5f), 2.5f, new StateBuff { m_State = StateBuff_State.Default, m_Buffs = { 1 } },
                                                             new StateBuff { m_State = StateBuff_State.Skill, m_Buffs = { 3 } } );
            }
            if(ShouldCall(533))
            {
                AnimationSpeed("attack_002", 1);
            }
            if(ShouldCall(1033))
            {
                Terminate();
            }
        }
    }
    internal class Skill_PuGong_102 : BaseSkillScript
    {
        protected override void StartHandler()
        {
            if(ShouldCall(0))
            {
                BreakSection(102, 200, 1000);
                BreakSection(101, 300, 1000);
                BreakSection(1, 300, 1000);

                DisableMoveInputCmd(true);
                DisableRotationInputCmd(true);
                Animation("attack_01");
                AnimationSpeed("attack_01", 1);
            }
            if(ShouldCall(33))
            {
                AnimationSpeed("attack_01", 4);
                Curvemove(DirectionType.Target, 0.066f, 0, 0, 6, 0, 0, 90);
                Effect("Monster_FX/Campaign_1/6_Npc_Private_Attack_01", 3000, "", false, Vector3.zero);
            }
            if(ShouldCall(66))
            {
                AreaDamage(new Vector3(0, 1.7f, 1.5f), 2.5f, new StateBuff { m_State = StateBuff_State.Default, m_Buffs = { 1 } },
                                                             new StateBuff { m_State = StateBuff_State.Skill, m_Buffs = { 3 } } );
            }
            if(ShouldCall(99))
            {
                AnimationSpeed("attack_01", 1);
                Animation("attack_02");
                AnimationSpeed("attack_02", 4);
            }
            if(ShouldCall(132))
            {
                AnimationSpeed("attack_02", 4);
                Curvemove(DirectionType.Target, 0.066f, 0, 0, 6, 0, 0, 90);
                Effect("Monster_FX/Campaign_1/6_Npc_Private_Attack_02", 3000, "", false, Vector3.zero);
            }
            if(ShouldCall(165))
            {
                AreaDamage(new Vector3(0, 1.7f, 1.5f), 2.5f, new StateBuff { m_State = StateBuff_State.Default, m_Buffs = { 1 } },
                                                             new StateBuff { m_State = StateBuff_State.Skill, m_Buffs = { 3 } } );
            }
            if(ShouldCall(198))
            {
                AnimationSpeed("attack_02", 1);
            }
            if(ShouldCall(631))
            {
                Terminate();
            }
        }
        protected override void BreakHandler()
        {
            Log("onbreak");
            Terminate();
        }
    }
}
