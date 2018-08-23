using System;
using System.Collections.Generic;
using Util;
using UnityEngine;
using Entitas.Data;
using Entitas;

namespace UnityClient
{
    [RequireComponent(typeof(UnityRigid), typeof(Rigidbody))]
    public class UnityView : MonoBehaviour, IView
    {
        public Entity Entity
        {
            get { return m_Entity; }
        }
        public void Init(Entity entity)
        {
            m_Entity = entity;
        }
        public void PlayAnimation(string animName)
        {
            Animation animation = GetComponent<Animation>();
            if (null != animation)
            {
                if (!animation.Play(animName))
                {
                    LogUtil.Error("PlayAnimation failed. Target {0} animName {1}.", this.name, animName);
                }
            }
        }
        public void CrossFadeAnimation(string animName, float fadeLength = 0.3f)
        {
            Animation animation = GetComponent<Animation>();
            if (null != animation)
            {
                animation.CrossFade(animName, 0.3f);
            }
        }
        public void PlayAnimation(string animName, float speed, float weight, int layer, int wrapMode, int blendMode, int playMode, long crossFadeTime)
        {
            Animation animation = GetComponent<Animation>();
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
        public void SetAnimationSpeed(string animName, float speed)
        {
            Animation animation = GetComponent<Animation>();
            if (null != animation)
            {
                AnimationState state = animation[animName];
                if (null != state)
                {
                    state.speed = speed;
                }
            }
        }
        public void SetVisible(bool visible)
        {
            // 这里只隐藏skinedmeshrenderder, 看后续能否资源规范化
            SkinnedMeshRenderer[] smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(var smr in smrs)
            {
                smr.enabled = visible;
            }
        }

        private Entity m_Entity;
    }
}
