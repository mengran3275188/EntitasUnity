using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public interface IView
    {
        void Init(Entity entity);
        void PlayAnimation(string animName);
        void CrossFadeAnimation(string animName, float fadeLength = 0.3f);
        void PlayAnimation(string animName, float speed, float weight, int layer, int wrapMode, int blendMode, int playMode, long crossFadeTime);
        void SetAnimationSpeed(string animName, float speed);
        void SetVisible(bool visible);
    }
}
