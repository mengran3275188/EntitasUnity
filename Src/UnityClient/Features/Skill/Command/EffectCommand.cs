using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityClient;
using ScriptableSystem;
using ScriptableData;
using Util;

namespace SkillCommands
{
    /// <summary>
    /// charactereffect(effect_path,delete_time,attach_bone[,start_time]);
    /// 
    /// or
    /// 
    /// charactereffect(effect_path,delete_time,attach_bone[,start_time])
    /// {
    ///   transform(vector3(0,1,0)[,eular(0,0,0)[,vector3(1,1,1)]]);
    /// };
    /// </summary>
    internal class EffectCommand : AbstractCommand
    {

        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null != obj)
            {
                string effectPath = m_EffectPath;
                uint resId = IdSystem.Instance.GenId(IdEnum.Resource);

                if(string.IsNullOrEmpty(m_AttachPath))
                {
                    GfxSystem.CreateGameObject(resId, effectPath, obj.position.Value + m_Pos, m_Euler + new Vector3(0, (obj.rotation.RotateDir * Mathf.Rad2Deg), 0), m_Scale, m_DeleteTime);
                }
                else
                {
                    GfxSystem.CreateAndAttachGameObject(resId, effectPath, obj.resource.Value, m_AttachPath, m_IsAttach, m_Pos, m_Euler, m_Scale, m_DeleteTime);
                }
            }
            return ExecResult.Finished;
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_EffectPath = callData.GetParamId(0);
            }
            if (num > 1)
            {
                m_DeleteTime = float.Parse(callData.GetParamId(1)) / 1000.0f;
            }
            if (num > 2)
            {
                m_AttachPath = callData.GetParamId(2);
            }
            if (num > 3)
            {
                m_IsAttach = bool.Parse(callData.GetParamId(3));
            }
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null != callData)
            {
                Load(callData);

                for (int i = 0; i < funcData.Statements.Count; ++i)
                {
                    ScriptableData.CallData stCall = funcData.Statements[i] as ScriptableData.CallData;
                    if (null != stCall)
                    {
                        string id = stCall.GetId();
                        if (id == "transform")
                        {
                            if (stCall.GetParamNum() > 0)
                            {
                                ScriptableData.CallData param0 = stCall.GetParam(0) as ScriptableData.CallData;
                                if (null != param0)
                                    m_Pos = ScriptableDataUtility.CalcVector3(param0);
                            }
                            if (stCall.GetParamNum() > 1)
                            {
                                ScriptableData.CallData param1 = stCall.GetParam(1) as ScriptableData.CallData;
                                if (null != param1)
                                    m_Euler = ScriptableDataUtility.CalcEularRotation(param1);
                            }
                            if (stCall.GetParamNum() > 2)
                            {
                                ScriptableData.CallData param2 = stCall.GetParam(2) as ScriptableData.CallData;
                                if (null != param2)
                                    m_Scale = ScriptableDataUtility.CalcVector3(param2);
                            }
                        }
                    }
                }
            }
        }

        private string m_EffectPath = "";
        private string m_AttachPath = "";
        private float m_DeleteTime = 0;
        private bool m_IsAttach = true;
        private bool m_SpeedInfuence = false;

        private Vector3 m_Pos = Vector3.zero;
        private Vector3 m_Euler = Vector3.zero;
        private Vector3 m_Scale = Vector3.one;
        private bool useAsync = true;
    }
}
