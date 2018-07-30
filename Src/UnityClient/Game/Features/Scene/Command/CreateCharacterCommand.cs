using System;
using Entitas;
using Entitas.Data;
using ScriptableData;
using ScriptableSystem;
using UnityClient;
using Util;
using UnityEngine;

namespace SceneCommand
{
    internal class CreateCharacterCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            if (callData.GetParamNum() >= 3)
            {
                m_CharacterId = int.Parse(callData.GetParamId(0));

                m_LocalPosition = ScriptableDataUtility.CalcVector3(callData.GetParam(1) as ScriptableData.CallData);
                m_LocalRotation = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
            }
        }
        protected override void Load(FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null == callData)
                return;
            Load(callData);
            for (int i = 0; i < funcData.Statements.Count; ++i)
            {
                ScriptableData.CallData stCall = funcData.Statements[i] as ScriptableData.CallData;
                if (null == stCall)
                    continue;
                if (stCall.GetId() == "mainplayer")
                {
                    m_MainPlayer = true;
                }
                if (stCall.GetId() == "skill")
                {
                    if (stCall.GetParamNum() >= 1)
                    {
                        m_SkillId = int.Parse(stCall.GetParamId(0));
                    }
                }
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            var gameContext = Contexts.sharedInstance.game;

            var target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            uint entityId = IdSystem.Instance.GenId(IdEnum.Entity);
            // camp id
            int campId = m_MainPlayer ? (int)CampId.Red : (int)CampId.Blue;
            if (target.hasCamp)
                campId = target.camp.Value;

            Vector3 position = target.position.Value + Quaternion.Euler(0, target.rotation.Value, 0) * m_LocalPosition;
            float rotation = target.rotation.Value + m_LocalRotation.y;

            if(m_MainPlayer)
                Services.Instance.CreateCharacterService.CreatePlayer(entityId, m_CharacterId, campId, position, rotation);
            else
                Services.Instance.CreateCharacterService.CreateNpc(entityId, m_CharacterId, campId, position, rotation);

            if (m_SkillId > 0)
            {
                var entity = gameContext.GetEntityWithId(entityId);
                SkillSystem.Instance.StartSkill(target, entity, m_SkillId, target.position.Value, target.rotation.Value);
            }
            return ExecResult.Finished;
        }
        private int m_CharacterId;
        private bool m_MainPlayer = false;
        private Vector3 m_LocalPosition;
        private Vector3 m_LocalRotation;
        private int m_SkillId = 0;

        private enum PhysicsType
        {
            Invalid,
            Box,
            Capsule,
        }
    }
}
