using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityDelegate
{
    public static class ResourceSystem
    {
        public static void PreloadResource(string res, int count)
        {
            ResourceManager.Instance.PreloadResource(res, count);
        }
        public static void PreloadResource(UnityEngine.Object prefab, int count)
        {
            ResourceManager.Instance.PreloadResource(prefab, count);
        }
        public static void PreloadSharedResource(string res)
        {
            ResourceManager.Instance.PreloadSharedResource(res);
        }
        public static UnityEngine.Object NewObject(string res)
        {
            return ResourceManager.Instance.NewObject(res);
        }
        public static UnityEngine.Object NewObject(string res, float timeToRecycle)
        {
            return ResourceManager.Instance.NewObject(res, timeToRecycle);
        }
        public static UnityEngine.Object NewObject(UnityEngine.Object prefab)
        {
            return ResourceManager.Instance.NewObject(prefab);
        }
        public static UnityEngine.Object NewObject(UnityEngine.Object prefab, float timeToRecycle)
        {
            return ResourceManager.Instance.NewObject(prefab, timeToRecycle);
        }

        public static void InitializeObject(UnityEngine.Object obj)
        {
            ResourceManager.Instance.InitializeObject(obj);
        }
        public static bool RecycleObject(UnityEngine.Object obj)
        {
            return ResourceManager.Instance.RecycleObject(obj);
        }
        public static UnityEngine.Object GetObject(int instanceId)
        {
            return ResourceManager.Instance.GetObject(instanceId);
        }
        public static UnityEngine.Object GetSharedResource(string res)
        {
            return ResourceManager.Instance.GetSharedResource(res);
        }

        public static void Cleanup()
        {
            ResourceManager.Instance.CleanupResourcePool();
        }
        public static UnityEngine.AnimationState PrepareAnimation(UnityEngine.GameObject obj, string animName)
        {
            UnityEngine.AnimationState state = null;
            UnityEngine.Animation animation = obj.GetComponent<UnityEngine.Animation>();
            if (null != animation && !string.IsNullOrEmpty(animName))
            {
                state = animation[animName];
                if (null == state)
                {
                    UnityEngine.AnimationClip clip = GetSharedResource("Animations/" + animName) as UnityEngine.AnimationClip;
                    if (null != clip)
                    {
                        animation.AddClip(clip, animName);
                        state = animation[animName];
                    }
                }
            }
            return state;
        }
    }
}
