using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entitas.Data
{
    public enum MoveState
    {
        Idle,
        UserMoving,
        SkillMoving,
        ImpactMoving,
    }
    public enum RotateState
    { 
        UserRotate,
        SkillRotate,
        ImpactRotate,
    }

    public enum AnimationType
    {
        None = -1,
        Idle = 1,
        Run = 2,
        Hurt = 3,
    }

    public enum CampId
    {
        Red = 0,
        Blue = 1,
    }
}
