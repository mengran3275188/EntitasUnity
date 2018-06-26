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
    internal class CharacterEffectCommand : AbstractCommand
    {

        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            GameEntity obj = instance.Target as GameEntity;
            if (null != obj)
            {
                string effectPath = m_EffectPath;
                GfxSystem.CreateAndAttachGameObject(IdSystem.Instance.GenId(IdEnum.Resource), effectPath, obj.resource.ResourceId, m_AttachPath, m_DeleteTime, m_IsAttach);
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
                                //ScriptableData.CallData param1 = stCall.GetParam(1) as ScriptableData.CallData;
                                //if (null != param1)
                                //    m_Dir = ScriptableDataUtility.CalcEularRotation(param1);
                            }
                            if (stCall.GetParamNum() > 2)
                            {
                                ScriptableData.CallData param2 = stCall.GetParam(2) as ScriptableData.CallData;
                                if (null != param2)
                                    m_Scale = ScriptableDataUtility.CalcVector3(param2);
                            }
                        }
                        else if (id == "speedinfuence")
                        {
                            if (stCall.GetParamNum() > 0)
                            {
                                m_SpeedInfuence = bool.Parse(stCall.GetParamId(0));
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
        private Quaternion m_Dir = Quaternion.identity;
        private Vector3 m_Scale = Vector3.one;
        private bool useAsync = true;
    }
    /// <summary>
    /// sceneeffect(effect_path,delete_time[,vector3(x,y,z)[,start_time[,eular(rx,ry,rz)[,vector3(sx,sy,sz)]]]]);
    /// </summary>
    internal class SceneEffectCommand : AbstractCommand
    {
        public override ICommand Clone()
        {
            SceneEffectCommand triger = new SceneEffectCommand();
            triger.m_EffectPath = m_EffectPath;
            triger.m_Pos = m_Pos;
            triger.m_Dir = m_Dir;
            triger.m_Scale = m_Scale;
            triger.m_DeleteTime = m_DeleteTime;
            triger.m_IsRotateRelativeUser = m_IsRotateRelativeUser;
            triger.m_SpeedInfuence = m_SpeedInfuence;
            return triger;
        }

        //void afterEffectLoad(GameObject obj, Instance instance, UnityEngine.Object prefabObj, bool isAsync = true)
        //{
        //    if (prefabObj == null)
        //        return;
        //    GameObject effectObj = null;
        //    if (isAsync)
        //    {
        //        effectObj = DashFire.ResourceSystem.NewObject(prefabObj, m_DeleteTime) as GameObject;
        //    }
        //    else
        //    {
        //        effectObj = prefabObj as GameObject;
        //    }
        //    if (effectObj != null)
        //    {
        //        TriggerUtil.SetObjVisible(effectObj, true);
        //        effectObj.SetActive(false);
        //        Vector3 pos = obj.transform.position + obj.transform.localRotation * m_Pos;
        //        effectObj.transform.position = pos;
        //        effectObj.transform.localScale = m_Scale;
        //        if (m_IsRotateRelativeUser)
        //        {
        //            effectObj.transform.parent = obj.transform;
        //            effectObj.transform.localRotation = m_Dir;
        //            effectObj.transform.parent = null;
        //        }
        //        else
        //        {
        //            effectObj.transform.localRotation = m_Dir;
        //        }
        //        EffectManager em = instance.CustomDatas.GetData<EffectManager>();
        //        if (em == null)
        //        {
        //            em = new EffectManager();
        //            instance.CustomDatas.AddData<EffectManager>(em);
        //        }
        //        em.AddEffect(effectObj);
        //        if (m_SpeedInfuence)
        //        {
        //            em.SetParticleSpeed(instance.EffectScale * instance.TimeScale);
        //        }
        //        else
        //        {
        //            em.SetParticleSpeed(instance.EffectScale);
        //        }
        //        effectObj.SetActive(true);
        //    }
        //}

        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            //GameEntity obj = instance.Target as GameEntity;
            //if (null != obj)
            //{
            //    string effectPath = m_EffectPath;
            //    ScriptableDataUtility.EffectPathFix(ref effectPath);
            //    if (DashFire.LogicForGfxThread.LoadAsync != null && useAsync)
            //    {
            //        UnityEngine.Object getObj = DashFire.logicEffectManager.Instance.createEffect_Async(effectPath, false, (loadObj) =>
            //        {
            //            afterEffectLoad(obj, instance, loadObj);
            //        }
            //        );
            //    }
            //    else
            //    {
            //        UnityEngine.Object _effectObj = DashFire.ResourceSystem.NewObject(effectPath, m_DeleteTime);
            //        if (null != _effectObj)
            //        {
            //            afterEffectLoad(obj, instance, _effectObj, false);
            //        }
            //    }

            //    return false;
            //}
            //else
            //{
            //    return false;
            //}
            return ExecResult.Finished;
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            //int num = callData.GetParamNum();
            //if (num > 0)
            //{
            //    m_EffectPath = callData.GetParamId(0);
            //}
            //if (num > 1)
            //{
            //    m_DeleteTime = float.Parse(callData.GetParamId(1)) / 1000.0f;
            //}
            //if (num > 2)
            //{
            //    m_Pos = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
            //}
            //if (num > 3)
            //{
            //    m_Dir = ScriptableDataUtility.CalcEularRotation(callData.GetParam(3) as ScriptableData.CallData);
            //}
            //if (num > 4)
            //{
            //    m_Scale = ScriptableDataUtility.CalcVector3(callData.GetParam(4) as ScriptableData.CallData);
            //}
            //if (num > 5)
            //{
            //    m_IsRotateRelativeUser = bool.Parse(callData.GetParamId(5));
            //}
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
                        if (id == "speedinfuence")
                        {
                            if (stCall.GetParamNum() > 0)
                            {
                                m_SpeedInfuence = bool.Parse(stCall.GetParamId(0));
                            }
                        }
                    }
                }
            }
        }

        private string m_EffectPath = "";
        private Vector3 m_Pos = Vector3.zero;
        private Quaternion m_Dir = Quaternion.identity;
        private Vector3 m_Scale = Vector3.one;
        private bool m_SpeedInfuence = false;
        private float m_DeleteTime = 0;
        private bool m_IsRotateRelativeUser = false;
        private bool useAsync = true;
    }
}
