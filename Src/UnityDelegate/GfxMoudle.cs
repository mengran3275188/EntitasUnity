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
            Application.targetFrameRate = 30;

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

            HudMgr.Instance.OnStart();
        }
        public void OnTick()
        {
            InputManager.Instance.HandleInput();
            ResourceManager.Instance.Tick();
            HudMgr.Instance.Tick();
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

        public void DrawLine(float startx, float starty, float startz, float endx, float endy, float endz)
        {
            Debug.DrawLine(new UnityEngine.Vector3(startx, starty, startz), new UnityEngine.Vector3(endx, endy, endz));
        }
        public void DrawPoint(float pointx, float pointy, float pointz, float radius)
        {
            // Gizmos.DrawSphere(new UnityEngine.Vector3(pointx, pointy, pointz), radius);
        }
        public void Instantiate(uint resId, string path)
        {
            var obj = ResourceSystem.NewObject(path) as UnityEngine.GameObject;
            if (null != obj)
            {
                RememberGameObject(resId, obj);
            }
            else
            {
                LogUtil.Error("GfxMoudle.Instantiate new object failed. Resource path is {0}.", path);
            }
        }
        public void CreateGameObject(uint resId, string resource, float positionX, float positionY, float positionZ, 
                                     float eularX, float eularY, float eularZ, 
                                     float scaleX, float scaleY, float scaleZ,  float recycleTime)
        {
            var obj = ResourceSystem.NewObject(resource, recycleTime) as GameObject;
            if(null != obj)
            {
                UnityEngine.Quaternion rotation = UnityEngine.Quaternion.Euler(eularX, eularY, eularZ);

                obj.transform.position = new UnityEngine.Vector3(positionX, positionY, positionZ);
                obj.transform.rotation = rotation;
                obj.transform.localScale = new UnityEngine.Vector3(scaleX, scaleY, scaleZ);
                RememberGameObject(resId, obj);
            }
            else
            {
                LogUtil.Error("GfxMoudle.CreateGameObject new object failed. Resource path is {0}.", resource);
            }
        }
        public void CreateAndAttachGameObject(uint resId, string resource, uint parentId, string path, bool isAttach,
                                              float positionX, float positionY, float positionZ, float eularX, float eularY, float eularZ,
                                              float scaleX, float scaleY, float scaleZ, float recycleTime)
        {
            GameObject parent = GetGameObject(parentId);
            if (null != parent)
            {
                Transform t = FindChildRecursive(parent.transform, path);
                if (null != t)
                {
                    GameObject obj = ResourceManager.Instance.NewObject(resource, recycleTime) as GameObject;
                    if (null != obj)
                    {
                        obj.transform.parent = t;
                        obj.transform.localPosition = new UnityEngine.Vector3(positionX, positionY, positionZ);
                        obj.transform.localRotation = UnityEngine.Quaternion.Euler(eularX, eularY, eularZ);
                        obj.transform.localScale = new UnityEngine.Vector3(scaleX, scaleY, scaleZ);
                        if (!isAttach)
                            obj.transform.parent = null;

                        RememberGameObject(resId, obj);
                    }
                    else
                    {
                        LogUtil.Error("GfxMoudle.CreateAndAttachGameObject: new object failed. resId = {0}, resource = {1}.", resId, resource);
                    }
                }
                else
                {
                    LogUtil.Error("GfxMoudle.CreateAndAttachGameObject: parent bone path not found. parentId = {0}, path = {1}.", parentId, path);
                }
            }
            else
            {
                LogUtil.Error("GfxMoudle.CreateAndAttachGameObject: can not find parent  object with resId. parentId = {0}.", parentId);
            }
        }
        public void DestroyResource(uint resId)
        {
            GameObject obj = GetGameObject(resId);
            if (null != obj)
            {
                ForgetGameObject(resId, obj);
                if(!ResourceManager.Instance.RecycleObject(obj))
                    GameObject.Destroy(obj);
                else
                    ResourceManager.Instance.SetActiveOptim(obj, false);
            }
            else
            {
                LogUtil.Error("GfxMoudle.DestroyResource : can not find obj with resId {0}.", resId);
            }
        }
        public void UpdatePosition(uint resId, float x, float y, float z)
        {
            GameObject target = GetGameObject(resId);
            if (null != target)
            {
                target.transform.position = new UnityEngine.Vector3(x, y, z);
            }
            else
            {
                LogUtil.Error("GfxMoudle.UpdatePostion : can not find obj with resId {0}.", resId);
            }
        }
        public void UpdateRotation(uint resId, float rotation)
        {
            GameObject target = GetGameObject(resId);
            if (null != target)
            {
                target.transform.rotation = UnityEngine.Quaternion.Euler(0, rotation / UnityEngine.Mathf.PI * 180f, 0);
            }
            else
            {
                LogUtil.Error("GfxMoudle.UpdateRotation : can not find obj with resId {0}.", resId);
            }
        }
        public void PlayAnimation(uint resId, string animName)
        {
            GameObject target = GetGameObject(resId);
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
            else
            {
                LogUtil.Error("GfxMoudle.PlayAnimation : can not find obj with resId {0}.", resId);
            }
        }
        public void CrossFadeAnimation(uint resId, string animName, float fadeLength = 0.3f)
        {
            GameObject target = GetGameObject(resId);
            if (null != target)
            {
                Animation animation = target.GetComponent<Animation>();
                if (null != animation)
                {
                    animation.CrossFade(animName, 0.3f);
                }
            }
            else
            {
                LogUtil.Error("GfxMoudle.CrossFadeAnimation : can not find obj with resId {0}.", resId);
            }
        }
        public void PlayAnimation(uint resId, string animName, float speed, float weight, int layer, int wrapMode, int blendMode, int playMode, long crossFadeTime)
        {
            GameObject target = GetGameObject(resId);
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
            else
            {
                LogUtil.Error("GfxMoudle.PlayAnimation : can not find obj with resId {0}.", resId);
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
            GameObject target = GetGameObject(resId);
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
            else
            {
                LogUtil.Error("GfxMoudle.SetAnimationSpeed : can not find obj with resId {0}.", resId);
            }
        }
        public void MoveChildToBone(uint resId, string childName, string boneName)
        {
            GameObject target = GetGameObject(resId);
            if (null != target)
            {
                Transform child = FindChildRecursive(target.transform, childName);
                if (null == child)
                {
                    LogUtil.Error("GfxMoudle.MoveChildToBone : child {0} not found.", childName);
                    return;
                }
                Transform bone = FindChildRecursive(target.transform, boneName);
                if (null == bone)
                {
                    LogUtil.Error("GfxMoudle.MoveChildToBone : bone {0} not found.", boneName);
                    return;
                }
                child.parent = bone;
                child.localRotation = UnityEngine.Quaternion.identity;
                child.localPosition = UnityEngine.Vector3.zero;
            }
            else
            {
                LogUtil.Error("GfxMoudle.MoveChildToBone : can not find obj with resId {0}.", resId);
            }
        }
        public void CreateHudText(uint resId, string text, long remainTime)
        {
            GameObject target = GetGameObject(resId);
            if (null != target)
            {
                HudMgr.Instance.AddHudText(text, target.transform, remainTime);
            }
            else
            {
                LogUtil.Error("GfxMoudle.CreateHudText : can not find obj with resId {0}.", resId);
            }
        }
        public void CreateHudHead(uint resId)
        {
            GameObject target = GetGameObject(resId);
            if (null != target)
            {
                HudMgr.Instance.AddHudHead(resId, target.transform);
            }
            else
            {
                LogUtil.Error("GfxMoudle.CreateHudText : can not find obj with resId {0}.", resId);
            }
        }
        public void UpdateHudHead(uint resId, float curHp, float maxHp)
        {
            HudMgr.Instance.UpdateHudHead(resId, curHp, maxHp);
        }
        public void RemoveHudHead(uint resId)
        {
            HudMgr.Instance.RemoveHudHead(resId);
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
        public float GetTime()
        {
            return Time.time;
        }


        public void SetJoystickXY(float x, float y)
        {
            m_JoystickX = x;
            m_JoystickY = y;
        }

        private GameObject GetGameObject(uint id)
        {
            GameObject ret = null;
            m_GameObjects.TryGetValue(id, out ret);
            return ret;
        }
        private void RememberGameObject(uint id, GameObject obj)
        {
            GameObject oldObj = null;
            if (m_GameObjects.TryGetValue(id, out oldObj))
            {
                oldObj.SetActive(false);
                m_GameObjectIds.Remove(oldObj);
                if (oldObj != obj)
                    GameObject.Destroy(oldObj);
            }
            else
            {
                m_GameObjects.AddLast(id, obj);
            }
            m_GameObjectIds[obj] = id;
        }
        private void ForgetGameObject(uint id, GameObject obj)
        {
            m_GameObjects.Remove(id);
            m_GameObjectIds.Remove(obj);
        }
        private Transform FindChildRecursive(Transform parent, string bonePath)
        {
            Transform t = parent.Find(bonePath);
            if (null != t)
            {
                return t;
            }
            else
            {
                int ct = parent.childCount;
                for (int i = 0; i < ct; ++i)
                {
                    t = FindChildRecursive(parent.GetChild(i), bonePath);
                    if (null != t)
                    {
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

        private LinkedListDictionary<uint, GameObject> m_GameObjects = new LinkedListDictionary<uint, GameObject>();
        private Dictionary<GameObject, uint> m_GameObjectIds = new Dictionary<GameObject, uint>();
    }
}
