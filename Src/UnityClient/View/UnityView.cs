using System;
using System.Collections.Generic;
using Util;
using UnityEngine;
using Entitas.Data;

namespace UnityClient
{
    [RequireComponent(typeof(UnityRigid), typeof(Rigidbody))]
    public class UnityView : MonoBehaviour, IView
    {
        public void PlayAnimation(string animName)
        {
            Animation animation = GetComponent<Animation>();
            if(null != animation)
            {
                if(!animation.Play(animName))
                {
                    LogUtil.Error("PlayAnimation failed. Target {0} animName {1}.", this.name, animName);
                }
            }
        }
        public void CrossFadeAnimation(string animName, float fadeLength = 0.3f)
        {
            Animation animation = GetComponent<Animation>();
            if(null != animation)
            {
                animation.CrossFade(animName, 0.3f);
            }
        }
    }
}
