using System;
using System.Collections.Generic;
using System.IO;
using Entitas.Data;
using UnityEngine;
using Util;

namespace UnityDelegate
{
    public class GfxMoudle : Singleton<GfxMoudle>
    {
        public PublishSubscribeSystem EventForLogic
        {
            get { return m_EventForLogic; }
        }
        public void OnStart(IActionQueue processor)
        {
            LogUtil.OnOutput += (Log_Type type, string msg) =>
            {
                switch (type)
                {
                    case Log_Type.LT_Debug:
                    case Log_Type.LT_Info:
                        Debug.Log(msg);
                        break;
                    case Log_Type.LT_Warn:
                        Debug.LogWarning(msg);
                        break;
                    case Log_Type.LT_Error:
                    case Log_Type.LT_Assert:
                        Debug.LogError(msg);
                        break;
                }
            };

            HomePath.Instance.SetHomePath(Application.streamingAssetsPath);


            m_LogicInvoker = processor;
        }
        public void OnTick()
        {
            InputManager.Instance.HandleInput();
        }
        public void OnQuit()
        {
        }
        public void SetLogicInvoker(IActionQueue processor)
        {
            m_LogicInvoker = processor;
        }
        public void Log(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        public void Instantiate(uint resId, string path)
        {
            var obj = ResourceSystem.NewObject(path) as UnityEngine.GameObject;
            m_IdMapper[resId] = obj.GetInstanceID();
        }
        public void DestroyResource(uint resId)
        {
            int id;
            if(m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject obj = ResourceSystem.GetObject(id) as GameObject;
                if (null != obj)
                    ResourceSystem.RecycleObject(obj);
            }
            else
            {
                LogUtil.Error("CreateAndAttachGameObject : can not find obj with resId {0}.", resId);
            }
        }
        public void CreateAndAttachGameObject(uint resId, string resource, uint parentId, string path, float recycleTime, bool isAttach)
        {
            int id = 0;
            if(m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject parent = ResourceSystem.GetObject(id) as GameObject;
                if(null != parent)
                {
                    Transform t = FindChildRecursive(parent.transform, path);
                    if(null != t)
                    {
                        GameObject obj = ResourceManager.Instance.NewObject(resource, recycleTime) as GameObject;
                        if(null != obj)
                        {
                            m_IdMapper[resId] = obj.GetInstanceID();
                            obj.transform.parent = t;
                            obj.transform.localPosition = UnityEngine.Vector3.zero;
                            if (isAttach)
                                obj.transform.parent = null;
                        }
                        else
                        {
                            LogUtil.Error("CreateAndAttachGameObject: new object failed. resId = {0}, resource = {1}.", resId, resource);
                        }
                    }
                    else
                    {
                        LogUtil.Error("CreateAndAttachGameObject: parent bone path not found. parentId = {0}, path = {1}.", parentId, path);
                    }
                }
                else
                {
                        LogUtil.Error("CreateAndAttachGameObject: parent  object not found. parentId = {0}.", parentId);
                }
            }
            else
            {
                LogUtil.Error("CreateAndAttachGameObject : can not find obj with resId {0}.", resId);
            }
        }
        public void UpdatePosition(uint resId, float x, float y, float z)
        {
            int id = 0;
            if (m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject target = ResourceSystem.GetObject(id) as UnityEngine.GameObject;
                if (null != target)
                {
                    target.transform.position = new UnityEngine.Vector3(x, y, z);
                }
            }
            else
            {
                LogUtil.Error("UpdatePostion : can not find obj with resId {0}.", resId);
            }
        }
        public void UpdateRotation(uint resId, float rotation)
        {
            int id = 0;
            if (m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject target = ResourceSystem.GetObject(id) as UnityEngine.GameObject;
                if (null != target)
                {
                    target.transform.rotation = UnityEngine.Quaternion.Euler(0, rotation / UnityEngine.Mathf.PI * 180f, 0);
                }
            }
            else
            {
                LogUtil.Error("UpdateRotation : can not find obj with resId {0}.", resId);
            }
        }
        public void PlayAnimation(uint resId, string animName)
        {
            int id = 0;
            if (m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject target = ResourceSystem.GetObject(id) as UnityEngine.GameObject;
                if (null != target)
                {
                    Animation animation = target.GetComponent<Animation>();
                    if (null != animation)
                    {
                        if (!animation.Play(animName))
                        {
                            LogUtil.Error("PlayAnimation failed. Target {0} animName {1}.", target.name, animName);
                        }
                    }
                }
            }
            else
            {
                LogUtil.Error("PlayAnimation : can not find obj with resId {0}.", resId);
            }
        }
        public void PlayAnimation(uint resId, string animName, float speed, float weight, int layer, int wrapMode, int blendMode, int playMode, long crossFadeTime)
        {
            int id = 0;
            if (m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject target = ResourceSystem.GetObject(id) as UnityEngine.GameObject;
                if (null != target)
                {
                    Animation animation = target.GetComponent<Animation>();
                    if (null != animation)
                    {
                        AnimationState state = animation[animName];
                        if (null != state)
                        {
                            state.speed = speed;
                            state.weight = weight;
                            state.layer = layer;
                            state.wrapMode = (WrapMode)wrapMode;
                            state.normalizedTime = 0;
                            state.blendMode = (blendMode == 0) ? AnimationBlendMode.Blend : AnimationBlendMode.Additive;
                            if (playMode == 0)
                                animation.Play(animName);
                            else
                                animation.CrossFade(animName, crossFadeTime / 1000.0f);
                        }
                        animation.Play(animName);
                    }
                }
            }
            else
            {
                LogUtil.Error("PlayAnimation : can not find obj with resId {0}.", resId);
            }
        }
        public bool IsKeyPressed(Keyboard.Code c)
        {
            return InputManager.Instance.IsKeyPressed(c);
        }
        public void ListenKeyPressState(Keyboard.Code[] codes)
        {
            InputManager.Instance.ListenKeyPressState(codes);
        }
        public void UpdateCamera(float x, float y, float z)
        {
            CameraManager.Instance.UpdateCamera(x, y, z);
        }
        public void SetAnimationSpeed(uint resId, string animName, float speed)
        {
            int id = 0;
            if (m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject target = ResourceSystem.GetObject(id) as UnityEngine.GameObject;
                if (null != target)
                {
                    Animation animation = target.GetComponent<Animation>();
                    if (null != animation)
                    {
                        AnimationState state = animation[animName];
                        if (null != state)
                        {
                            state.speed = speed;
                        }
                    }
                }
            }
            else
            {
                LogUtil.Error("PlayAnimation : can not find obj with resId {0}.", resId);
            }
        }
        public void MoveChildToBone(uint resId, string childName, string boneName)
        {
            int id = 0;
            if (m_IdMapper.TryGetValue(resId, out id))
            {
                GameObject target = ResourceSystem.GetObject(id) as UnityEngine.GameObject;
                if (null != target)
                {
                    Transform child = FindChildRecursive(target.transform, childName);
                    if (null == child)
                    {
                        LogUtil.Error("GfxMoudle.MoveChildToBone : child {0} not found.", childName);
                        return;
                    }
                    Transform bone = FindChildRecursive(target.transform, boneName);
                    if(null == bone)
                    {
                        LogUtil.Error("GfxMoudle.MoveChildToBone : bone {0} not found.", boneName);
                        return;
                    }
                    child.parent = bone;
                    child.localRotation = UnityEngine.Quaternion.identity;
                    child.localPosition = UnityEngine.Vector3.zero;
                }
            }
            else
            {
                LogUtil.Error("PlayAnimation : can not find obj with resId {0}.", resId);
            }
        }
        public bool GetJoyStickDir(out float dir)
        {
            dir = 0;
            if (Util.Mathf.Approximately(m_JoystickX, 0) && Util.Mathf.Approximately(m_JoystickY, 0))
                return false;
            dir = Util.Mathf.Atan2(m_JoystickX, m_JoystickY);
            return true;
        }

        public float GetCameraYaw()
        {
            return 0;
        }
        public int GetMainPlayerSkill()
        {
            return m_InputSkillId;
        }


        public void SetJoystickXY(float x, float y)
        {
            m_JoystickX = x;
            m_JoystickY = y;
        }
        private Transform FindChildRecursive(Transform parent, string bonePath)
        {
          Transform t = parent.Find(bonePath);
          if (null != t) {
            return t;
          } else {
            int ct = parent.childCount;
            for (int i = 0; i < ct; ++i) {
              t = FindChildRecursive(parent.GetChild(i), bonePath);
              if (null != t) {
                return t;
              }
            }
          }
          return null;
        }

        public void PublishLogicEvent<T1>(string evt, string group, T1 t1)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1);
            }
        }

        public void PublishLogicEvent<T1, T2>(string evt, string group, T1 t1, T2 t2)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2);
            }
        }

        public void PublishLogicEvent<T1, T2, T3>(string evt, string group, T1 t1, T2 t2, T3 t3)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            }
        }

        public void PublishLogicEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueAction(PublishLogicEventImpl, evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            }
        }

        public void PublishLogicEvent(string evt, string group, object[] args)
        {
            if (null != m_LogicInvoker)
            {
                m_LogicInvoker.QueueActionWithDelegation((MyAction<string, string, object[]>)PublishLogicEventImpl, evt, group, args);
            }
        }
        //游戏逻辑层执行的函数，供Gfx线程异步调用
        private void PublishLogicEventImpl(string evt, string group)
        {
            m_EventForLogic.Publish(evt, group);
        }

        private void PublishLogicEventImpl<T1>(string evt, string group, T1 t1)
        {
            m_EventForLogic.Publish(evt, group, t1);
        }

        private void PublishLogicEventImpl<T1, T2>(string evt, string group, T1 t1, T2 t2)
        {
            m_EventForLogic.Publish(evt, group, t1, t2);
        }

        private void PublishLogicEventImpl<T1, T2, T3>(string evt, string group, T1 t1, T2 t2, T3 t3)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
        }

        private void PublishLogicEventImpl<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string evt, string group, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14)
        {
            m_EventForLogic.Publish(evt, group, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
        }

        private void PublishLogicEventImpl(string evt, string group, object[] args)
        {
            m_EventForLogic.Publish(evt, group, args);
        }

        private static float m_JoystickX = 0;
        private static float m_JoystickY = 0;
        private static int m_InputSkillId = 0;

        private PublishSubscribeSystem m_EventForLogic = new PublishSubscribeSystem();
        private IActionQueue m_LogicInvoker;
        private Dictionary<uint, int> m_IdMapper = new Dictionary<uint, int>();
    }
}
