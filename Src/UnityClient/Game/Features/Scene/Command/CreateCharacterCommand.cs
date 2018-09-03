using System;
using Entitas;
using Entitas.Data;
using UnityClient;
using Util;
using UnityEngine;
using ScriptableData;

namespace SceneCommand
{
    internal class CreateCharacterCommand : AbstractCommand
    {
        public void Load(int characterId, Vector3 position, Vector3 rotation, bool isMainPlayer, int skillId)
        {
            m_CharacterId = characterId;
            m_LocalPosition = position;
            m_LocalRotation = rotation;
            m_MainPlayer = isMainPlayer;
            m_SkillId = skillId;
        }
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
        protected override void Load(StatementData statementData)
        {
            if(statementData.Functions.Count == 2)
            {
                FunctionData first = statementData.First;
                FunctionData second = statementData.Second;
                if(null != first && null != first.Call && null != second && null != second.Call)
                {
                    Load(first);
                    LoadVarName(second.Call);
                }
            }
        }
        private void LoadVarName(CallData callData)
        {
            if(callData.GetId() == "ret" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName = callData.GetParamId(0);
            }
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
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

            Vector3 position = target.position.Value + Quaternion.Euler(0, Mathf.Rad2Deg * target.rotation.Value, 0) * m_LocalPosition;
            float rotation = target.rotation.Value + m_LocalRotation.y * Mathf.Deg2Rad;

            if(m_MainPlayer)
                Services.Instance.CreateCharacterService.CreatePlayer(entityId, target.id.value, m_CharacterId, campId, position, rotation);
            else
                Services.Instance.CreateCharacterService.CreateNpc(entityId, target.id.value, m_CharacterId, campId, position, rotation);

            if (m_SkillId > 0)
            {
                var entity = gameContext.GetEntityWithId(entityId);
                SkillSystem.Instance.StartSkill(target, entity, m_SkillId, -1, target.position.Value, target.rotation.Value);
            }

            if (!string.IsNullOrEmpty(m_ObjIdVarName))
                instance.AddVariable(m_ObjIdVarName, entityId);

            return ExecResult.Finished;
        }
        private int m_CharacterId;
        private bool m_MainPlayer = false;
        private Vector3 m_LocalPosition;
        private Vector3 m_LocalRotation;
        private int m_SkillId = 0;
        private string m_ObjIdVarName = string.Empty;
    }
}
