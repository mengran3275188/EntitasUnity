﻿using System;
using ScriptableSystem;
using ScriptableData;
using UnityClient;

namespace SkillCommands
{
    public class SkillCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                m_Target.InitFromDsl(callData.GetParam(0));
                m_SkillId = int.Parse(callData.GetParamId(1));
            }
        }
        protected override void UpdateArguments(object iterator, object[] args)
        {
            m_Target.Evaluate(iterator, args);
        }
        protected override void UpdateVariables(IInstance instance)
        {
            m_Target.Evaluate(instance);
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;
            GameEntity skillEntity = Contexts.sharedInstance.game.GetEntityWithId(m_Target.Value);
            if (null == skillEntity)
                return ExecResult.Finished;

            SkillSystem.Instance.StartSkill(target, skillEntity, m_SkillId, -1, target.position.Value, target.rotation.Value);

            return ExecResult.Finished;
        }

        private IValue<uint> m_Target = new SkillValue<uint>();
        private int m_SkillId = 0;
    }
}