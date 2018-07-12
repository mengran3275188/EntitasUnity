using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptableSystem;
using ScriptableData;
using Entitas.Data;
using UnityClient;

namespace SkillCommands
{
    internal class DamageCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 1)
            {
                m_Damage = float.Parse(callData.GetParamId(0));
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            GameEntity sender = instance.Sender as GameEntity;
            if(null != obj && null != sender)
            {
                float damage = AttrSystem.CalcDamage();

                obj.ReplaceHp(obj.hp.Value - damage);
            }
            return ExecResult.Finished;
        }
        private float m_Damage;
    }
}
