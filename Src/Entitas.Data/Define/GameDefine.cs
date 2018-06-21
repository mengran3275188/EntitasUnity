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
    }
    public enum RotateState
    { 
        UserRotate,
        SkillRotate,
    }
}
