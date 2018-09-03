using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptableData;
using UnityClient;

namespace SkillCommands
{
    internal class AnimationCommand : AbstractCommand
    {
        public void Load(string animName)
        {
            m_AnimName = animName;
        }
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null != obj)
            {
                //if(obj.isAnimation)
                {
                    Services.Instance.ViewService.PlayAnimation(obj, m_AnimName, m_Speed, m_Weight, m_Layer, m_WrapMode, m_BlendMode, m_PlayMode, m_CrossFadeTime);
                }
            }
            return ExecResult.Finished;
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_AnimName = callData.GetParamId(0);
            }
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null != callData)
            {
                Load(callData);

                foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements)
                {
                    ScriptableData.CallData stCall = statement as ScriptableData.CallData;
                    if (null != stCall && stCall.GetParamNum() > 0)
                    {
                        string id = stCall.GetId();
                        string param = stCall.GetParamId(0);

                        if (id == "speed")
                        {
                            m_Speed = float.Parse(param);
                            if (stCall.GetParamNum() >= 2)
                            {
                                m_IsEffectSkillTime = bool.Parse(stCall.GetParamId(1));
                            }
                        }
                        else if (id == "weight")
                        {
                            m_Weight = float.Parse(param);
                        }
                        else if (id == "layer")
                        {
                            m_Layer = int.Parse(param);
                        }
                        else if (id == "playmode")
                        {
                            m_PlayMode = int.Parse(param);
                            if (stCall.GetParamNum() >= 2)
                            {
                                m_CrossFadeTime = long.Parse(stCall.GetParamId(1));
                            }
                        }
                        else if (id == "blendmode")
                        {
                            m_BlendMode = int.Parse(param);
                        }
                        else if (id == "mixingnode")
                        {
                            m_MixingNode = param;
                        }
                        else if (id == "wrapmode")
                        {
                            m_WrapMode = int.Parse(param);
                        }
                    }
                }
            }
        }

        private string m_AnimName = "";

        private float m_Speed = 1.0f;
        private bool m_IsEffectSkillTime = false;
        private float m_Weight = 1.0f;
        private int m_Layer = 0;
        private int m_WrapMode = 8; //WrapMode.ClampForever
        private int m_PlayMode = 0;
        private int m_BlendMode = 0;
        private string m_MixingNode = "";
        private long m_CrossFadeTime = 300;
    }
    public class AnimationSpeedCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(IInstance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (obj != null)
            {
                Services.Instance.ViewService.SetAnimationSpeed(obj, m_AnimName, m_Speed);
            }
            return ExecResult.Finished;
        }

        public void Load(string animationName, float speed)
        {
            m_AnimName = animationName;
            m_Speed = speed;
        }
        protected override void Load(ScriptableData.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 2)
            {
                m_AnimName = callData.GetParamId(0);
                m_Speed = float.Parse(callData.GetParamId(1));
            }
        }

        private string m_AnimName = "";
        private float m_Speed = 1.0f;
    }
}
