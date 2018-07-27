using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Data
{
    public interface IView
    {
        void PlayAnimation(string animName);
        void CrossFadeAnimation(string animName, float fadeLength = 0.3f);
    }
}
