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
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            var gameContext = Contexts.sharedInstance.game;

            uint resId = IdGenerater.Instance.NextId();
            GfxSystem.Instantiate(resId, "");

            GameEntity entity = gameContext.CreateEntity();
            entity.isMainPlayer = true;
            entity.AddMovement(MoveState.Idle, 0, 0);
            entity.AddResource(resId);
            Vector3 pos = new Vector3(5, 0, 5);//SpatialSystem.Instance.GetNearestWalkablePos(new Vector3(0, 0, 0));
            entity.AddPosition(pos.x, pos.y, pos.z);
            entity.AddRotation(RotateState.UserRotate, 0);
            entity.AddSkill(null);

            return ExecResult.Finished;
        }

        private int m_CharacterId;
        private Vector3 m_LocalPosition;
        private Vector3 m_LocalRotation;
    }
}
