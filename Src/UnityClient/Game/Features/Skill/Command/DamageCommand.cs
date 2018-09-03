using System;
using ScriptableData;
using UnityClient;

namespace SkillCommands
{
    internal class DamageCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 1)
            {
                m_Damage = float.Parse(callData.GetParamId(0));
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity sender = Contexts.sharedInstance.game.GetEntityWithId(instance.SenderId);
            if (instance.Target is GameEntity obj && null != sender)
            {
                if (obj.hasHp)
                {
                    float damage = AttrSystem.CalcDamage();
                    damage = m_Damage;

                    obj.ReplaceHp(obj.hp.Value - damage);

                    Services.Instance.HudService.AddDamageText(obj, "-10", 3000);
                }
            }
            return ExecResult.Finished;
        }
        private float m_Damage;
    }
}
