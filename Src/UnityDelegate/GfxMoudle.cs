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
        public IActionQueue LogicInvoker
        {
            get { return m_LogicInvoker; }
        }
        public void RegisteLog()
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
        }
        public void OnStart(IActionQueue processor)
        {
            Application.targetFrameRate = 60;

            m_LogicInvoker = processor;
        }
        public void Update()
        {
            ResourceManager.Instance.Tick();
        }
        public void FixUpdate()
        {
        }
        public void OnQuit()
        {
        }
        public void SetLogicInvoker(IActionQueue processor)
        {
            m_LogicInvoker = processor;
        }
        public void DrawLine(float startx, float starty, float startz, float endx, float endy, float endz)
        {
            Debug.DrawLine(new UnityEngine.Vector3(startx, starty, startz), new UnityEngine.Vector3(endx, endy, endz));
        }
        public void DrawPoint(float pointx, float pointy, float pointz, float radius)
        {
            // Gizmos.DrawSphere(new UnityEngine.Vector3(pointx, pointy, pointz), radius);
        }
        public void DrawCircle(float centerX, float centerY, float centerZ, float radius, float remainTime, float degreeStep = 0.5f)
        {
            UnityEngine.Vector3 center = new UnityEngine.Vector3(centerX, centerY, centerZ);

            int count = (int)(2 * UnityEngine.Mathf.PI / degreeStep);

            List<UnityEngine.Vector3> points = new List<UnityEngine.Vector3>();

            for(int i = 0; i < count + 1; ++i)
            {
                float degree = 2 * UnityEngine.Mathf.PI / count * i;
                UnityEngine.Vector3 point = center + new UnityEngine.Vector3(UnityEngine.Mathf.Cos(degree), 0, UnityEngine.Mathf.Sin(degree)) * radius;
                points.Add(point);
            }

            if(points.Count > 1)
            {
                UnityEngine.Vector3 firstPoint = points[0];

                for(int i = 0; i < points.Count; ++i)
                {
                    Debug.DrawLine(points[i], points[(i + 1) % points.Count], Color.red, remainTime);
                }
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
        public bool GetJoyStickDir(out float dir)
        {
            dir = 0;
            if (Mathf.Approximately(m_JoystickX, 0) && Mathf.Approximately(m_JoystickY, 0))
                return false;
            dir = Mathf.Atan2(m_JoystickX, m_JoystickY);
            return true;
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
