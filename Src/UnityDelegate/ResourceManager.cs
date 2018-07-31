using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace UnityDelegate
{
    internal class ResourceManager : Singleton<ResourceManager>
    {
        internal void PreloadResource(string res, int count)
        {
            UnityEngine.Object prefab = GetSharedResource(res);
            PreloadResource(prefab, count);
        }
        internal void PreloadResource(UnityEngine.Object prefab, int count)
        {
            if (null != prefab)
            {
                for (int i = 0; i < count; ++i)
                {
                    UnityEngine.Object obj = UnityEngine.GameObject.Instantiate(prefab);
                    AddToUnusedResources(prefab.GetInstanceID(), obj);
                }
            }
        }
        internal void PreloadSharedResource(string res)
        {
            GetSharedResource(res);
        }

        //优化setActive函数
        internal void SetActiveOptim(UnityEngine.GameObject obj, bool active)
        {
            obj.SetActive(active);
        }
        internal UnityEngine.Object NewObject(string res)
        {
            return NewObject(res, 0);
        }
        internal UnityEngine.Object NewObject(string res, float timeToRecycle)
        {
            UnityEngine.Object prefab = GetSharedResource(res);
            return NewObject(prefab, timeToRecycle);
        }
        internal UnityEngine.Object NewObject(UnityEngine.Object prefab)
        {
            return NewObject(prefab, 0);
        }
        internal UnityEngine.Object NewObject(UnityEngine.Object prefab, float timeToRecycle)
        {
            UnityEngine.Object obj = null;
            if (null != prefab)
            {
                float curTime = UnityEngine.Time.time;
                float time = timeToRecycle;
                if (timeToRecycle > 0)
                    time += curTime;
                int resId = prefab.GetInstanceID();
                obj = NewFromUnusedResources(resId);
                if (null == obj)
                {
                    obj = UnityEngine.GameObject.Instantiate(prefab);
                }
                if (null != obj)
                {
                    AddToUsedResources(obj, resId, time);

                    InitializeObject(obj);
                }
            }
            return obj;
        }
        internal bool RecycleObject(UnityEngine.Object obj)
        {
            bool ret = false;
            if (null != obj)
            {
                int objId = obj.GetInstanceID();
                if (m_UsedResources.Contains(objId))
                {
                    UsedResourceInfo resInfo = m_UsedResources[objId];
                    if (null != resInfo)
                    {
                        FinalizeObject(resInfo.m_Object);
                        RemoveFromUsedResources(objId);
                        AddToUnusedResources(resInfo.m_ResId, obj);
                        resInfo.Recycle();
                        ObjectCache.Instance.Push<UsedResourceInfo>(resInfo);
                        ret = true;
                    }
                }
            }
            return ret;
        }
        internal void Tick()
        {
            float curTime = UnityEngine.Time.time;

            for (LinkedListNode<UsedResourceInfo> node = m_UsedResources.FirstValue; null != node;)
            {
                UsedResourceInfo resInfo = node.Value;
                if (resInfo.m_RecycleTime > 0 && resInfo.m_RecycleTime < curTime)
                {
                    node = node.Next;

                    FinalizeObject(resInfo.m_Object);
                    AddToUnusedResources(resInfo.m_ResId, resInfo.m_Object);
                    RemoveFromUsedResources(resInfo.m_ObjId);
                    resInfo.Recycle();
                }
                else
                {
                    node = node.Next;
                }
            }
        }
        internal UnityEngine.Object GetObject(int instanceId)
        {
            UsedResourceInfo usedResource;
            if (m_UsedResources.TryGetValue(instanceId, out usedResource))
                return usedResource.m_Object;
            return null;
        }

        internal UnityEngine.Object GetSharedResource(string res)
        {
            bool notdestory = s_SharedResourcePath.Contains(res);
            return GetSharedResource(res, notdestory);
        }
        internal void CleanupResourcePool()
        {
            CleanUseResource();
            for (LinkedListNode<UsedResourceInfo> node = m_UsedResources.FirstValue; null != node;)
            {
                UsedResourceInfo resInfo = node.Value;
                node = node.Next;
                RemoveFromUsedResources(resInfo.m_ObjId);
                resInfo.Recycle();
            }

            foreach (KeyValuePair<int, Queue<UnityEngine.Object>> pair in m_UnusedResources)
            {
                int key = pair.Key;
                Queue<UnityEngine.Object> queue = pair.Value;
                queue.Clear();
            }

            foreach (KeyValuePair<string, ObjectEx> pair in m_LoadedPrefabs)
            {
                string key = pair.Key;
                if (pair.Value.notdesotryed)
                    continue;
                m_WaitDeleteLoadedPrefabEntrys.Add(key);
            }
            for (int i = 0; i < m_WaitDeleteLoadedPrefabEntrys.Count; i++)
            {
                m_LoadedPrefabs.Remove(m_WaitDeleteLoadedPrefabEntrys[i]);
            }
            m_WaitDeleteLoadedPrefabEntrys.Clear();

            UnityEngine.Resources.UnloadUnusedAssets();
        }

        private UnityEngine.Object GetSharedResource(string res, bool notdestoryed)
        {
            UnityEngine.Object obj = null;
            if (string.IsNullOrEmpty(res))
                return obj;
            ObjectEx objEx = null;
            if (!m_LoadedPrefabs.TryGetValue(res, out objEx))
            {
                obj = UnityEngine.Resources.Load(res);
                if (null != obj)
                {
                    objEx = new ObjectEx();
                    objEx.obj = obj;
                    objEx.notdesotryed = notdestoryed;
                    m_LoadedPrefabs.Add(res, objEx);
                }
                else
                {
                    LogUtil.Error("Load asset failed : {0}", res);
                }
            }
            else
            {
                obj = objEx.obj;
            }
            return obj;
        }
        private UnityEngine.Object NewFromUnusedResources(int res)
        {
            UnityEngine.Object obj = null;
            Queue<UnityEngine.Object> queue;
            if (m_UnusedResources.TryGetValue(res, out queue))
            {
                if (queue.Count > 0)
                    obj = queue.Dequeue();
            }
            return obj;
        }
        private void AddToUnusedResources(int res, UnityEngine.Object obj)
        {
            Queue<UnityEngine.Object> queue;
            if (m_UnusedResources.TryGetValue(res, out queue))
            {
                queue.Enqueue(obj);
            }
            else
            {
                queue = new Queue<UnityEngine.Object>();
                queue.Enqueue(obj);
                m_UnusedResources.Add(res, queue);
            }
        }
        private void AddToUsedResources(UnityEngine.Object obj, int resId, float recycleTime)
        {
            int objId = obj.GetInstanceID();
            if (!m_UsedResources.Contains(objId))
            {
                UsedResourceInfo info = ObjectCache.Instance.Get<UsedResourceInfo>();
                info.m_ObjId = objId;
                info.m_Object = obj;
                info.m_ResId = resId;
                info.m_RecycleTime = recycleTime;
                if (m_UseResourcesCount.ContainsKey(obj.name))
                {
                    m_UseResourcesCount[obj.name]++;
                }
                else
                {
                    m_UseResourcesCount.Add(obj.name, 1);
                }
                m_UsedResources.AddLast(objId, info);
            }
        }

        private void CleanUseResource()
        {
            m_UseResourcesCount.Clear();
        }

        private void RemoveFromUsedResources(int objId)
        {
            if (m_UseResourcesCount.Count > 0)
            {
                UsedResourceInfo info = m_UsedResources[objId];
                if (m_UseResourcesCount.ContainsKey(info.m_Object.name))
                {
                    m_UseResourcesCount[info.m_Object.name]--;
                    if (m_UseResourcesCount[info.m_Object.name] <= 0)
                    {
                        m_UseResourcesCount[info.m_Object.name] = 0;
                    }
                }
            }

            m_UsedResources.Remove(objId);
        }

        internal void InitializeObject(UnityEngine.Object obj)
        {
            UnityEngine.GameObject gameObj = obj as UnityEngine.GameObject;
            if (null != gameObj)
            {
                if (!gameObj.activeSelf)
                    SetActiveOptim(gameObj, true);

                UnityEngine.ParticleSystem[] pss = gameObj.GetComponentsInChildren<UnityEngine.ParticleSystem>();
                for (int i = 0; i < pss.Length; i++)
                {
                    UnityEngine.ParticleSystem ps = pss[i];
                    if (null != ps && ps.main.playOnAwake)
                    {
                        ps.Stop();
                        ps.Play();
                    }
                }

                UnityEngine.Rigidbody body = gameObj.GetComponent<UnityEngine.Rigidbody>();
                if (body != null)
                {
                    body.isKinematic = false;
                }
            }
        }
        private void FinalizeObject(UnityEngine.Object obj)
        {
            UnityEngine.GameObject gameObj = obj as UnityEngine.GameObject;
            if (null != gameObj)
            {
                UnityEngine.ParticleSystem[] pss = gameObj.GetComponentsInChildren<UnityEngine.ParticleSystem>();
                for (int i = 0; i < pss.Length; i++)
                {
                    UnityEngine.ParticleSystem ps = pss[i];
                    if (null != ps && ps.main.playOnAwake)
                    {
                        ps.Clear();
                        ps.Stop();
                        ps.time = 0;
                    }
                }
                UnityEngine.Rigidbody body = gameObj.GetComponent<UnityEngine.Rigidbody>();
                if (body != null)
                {
                    body.isKinematic = true;
                }
                if (null != gameObj.transform.parent)
                    gameObj.transform.SetParent(null, false);
                if (gameObj.activeSelf)
                    SetActiveOptim(gameObj, false);
            }
        }

        private class UsedResourceInfo
        {
            internal int m_ObjId;
            internal UnityEngine.Object m_Object;
            internal int m_ResId;
            internal float m_RecycleTime;

            internal void Recycle()
            {
                m_Object = null;
            }
        }
        public class ObjectEx
        {
            public UnityEngine.Object obj;
            public bool notdesotryed = false;
        }


        private static HashSet<string> s_SharedResourcePath = new HashSet<string>() { };

        private Dictionary<string, ObjectEx> m_LoadedPrefabs = new Dictionary<string, ObjectEx>();
        private List<string> m_WaitDeleteLoadedPrefabEntrys = new List<string>();


        private LinkedListDictionary<int, UsedResourceInfo> m_UsedResources = new LinkedListDictionary<int, UsedResourceInfo>();
        private Dictionary<string, int> m_UseResourcesCount = new Dictionary<string, int>();
        private Dictionary<int, Queue<UnityEngine.Object>> m_UnusedResources = new Dictionary<int, Queue<UnityEngine.Object>>();
    }
}
