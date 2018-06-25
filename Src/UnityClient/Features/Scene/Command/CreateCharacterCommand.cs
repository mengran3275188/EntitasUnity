using System;
using Entitas;
using Entitas.Data;
using ScriptableData;
using ScriptableSystem;
using UnityClient;
using Util;

namespace SceneCommand
{
    internal class CreateCharacterCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            if(callData.GetParamNum() >= 3)
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
            for(int i = 0; i < funcData.Statements.Count; ++i)
            {
                ScriptableData.CallData stCall = funcData.Statements[i] as ScriptableData.CallData;
                if (null == stCall)
                    continue;
                if(stCall.GetId() == "mainplayer")
                {
                    m_MainPlayer = true;
                }
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            var gameContext = Contexts.sharedInstance.game;

            CharacterConfig config = CharacterConfigProvider.Instance.GetCharacterConfig(m_CharacterId);
            if(null != config)
            {
                uint resId = IdGenerater.Instance.NextId();
                GfxSystem.Instantiate(resId, config.Model);

                GameEntity entity = gameContext.CreateEntity();
                entity.isMainPlayer = m_MainPlayer;
                entity.AddMovement(MoveState.Idle, 0, 0);
                entity.AddResource(resId);
                //SpatialSystem.Instance.GetNearestWalkablePos(new Vector3(0, 0, 0));
                entity.AddPosition(m_LocalPosition.x, m_LocalPosition.y, m_LocalPosition.z);
                entity.AddRotation(RotateState.UserRotate, 0);
                entity.AddSkill(null);
                entity.AddImpact(null);
            }
            else
            {
                LogUtil.Error("CreateCharacterCommand.ExecCommand : character config {0} not found!", m_CharacterId);
            }

            return ExecResult.Finished;
        }

        private int m_CharacterId;
        private bool m_MainPlayer = false;
        private Vector3 m_LocalPosition;
        private Vector3 m_LocalRotation;
    }
}
