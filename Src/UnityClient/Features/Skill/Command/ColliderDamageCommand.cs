using System;
using System.Collections.Generic;
using ScriptableSystem;
using ScriptableData;
using Util;

namespace SkillCommands
{
    public class ColliderDamageCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            ColliderDamageCommand copy = new ColliderDamageCommand();
            copy.m_BuffId = m_BuffId;
            copy.m_Offset = m_Offset;
            copy.m_Size = m_Size;

            copy.m_HaveObjId = m_HaveObjId;
            copy.m_ObjIdVarName = m_ObjIdVarName.Clone();

            return copy;
        }
        protected override void UpdateArguments(object iterator, object[] args)
        {
            if (m_HaveObjId)
                m_ObjIdVarName.Evaluate(iterator, args);
        }
        protected override void UpdateVariables(Instance instance)
        {
            if (m_HaveObjId)
                m_ObjIdVarName.Evaluate(instance);
        }
        protected override void Load(CallData callData)
        {
            int num = callData.GetParamNum();
            if(num >= 3)
            {
                m_BuffId = int.Parse(callData.GetParamId(0));
                m_Offset = ScriptableDataUtility.CalcVector3(callData.GetParam(1) as ScriptableData.CallData);
                m_Size = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
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
                    Load(first.Call);
                    LoadVarName(second.Call);
                }
            }
        }
        private void LoadVarName(CallData callData)
        {
            if(callData.GetId() == "objid" && callData.GetParamNum() == 1)
            {
                m_ObjIdVarName.InitFromDsl(callData.GetParam(0));
                m_HaveObjId = true;
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            m_Instance = instance;
            m_Target = target;

            SpatialSystem.BoxCollider collider = new SpatialSystem.BoxCollider(Vector3.zero, m_Size);

            target.AddCollision(OnCollision, collider, m_Offset);

            if(m_HaveObjId)
            {
                int objId = 10086;
                string varName = m_ObjIdVarName.Value;
                if(varName.StartsWith("@") && !varName.StartsWith("@@"))
                {
                    instance.AddLocalVariable(varName, objId);
                }
                else
                {
                    instance.AddGlobalVariable(varName, objId);
                }
            }

            return ExecResult.Finished;
        }

        private void OnCollision(uint targetEntityId)
        {
            GameEntity target = m_Instance.Target as GameEntity;
            GameEntity collideTarget = Contexts.sharedInstance.game.GetEntityWithId(targetEntityId);
            if(null !=  target && null != collideTarget)
            {
                UnityClient.BuffSystem.Instance.StartBuff(target, collideTarget, 1, target.position.Value, target.rotation.RotateDir);
                m_Instance.SendMessage("oncollision");
            }
        }


        // config
        private int m_BuffId = 0;
        private Vector3 m_Offset = Vector3.zero;
        private Vector3 m_Size = Vector3.one;

        private IValue<string> m_ObjIdVarName = new SkillValue<string>();
        private bool m_HaveObjId = false;

        private GameEntity m_Target = null;

        private Instance m_Instance = null;
    }

    internal class RemoveColliderCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if(null != obj && obj.hasCollision)
            {
                obj.RemoveCollision();
            }
            return ExecResult.Finished;
        }
    }
}
