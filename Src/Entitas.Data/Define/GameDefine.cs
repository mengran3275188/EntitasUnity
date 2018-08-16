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
        Dead = 4,
        Born = 5,
    }

    public enum CampId
    {
        Red = 0,
        Blue = 1,
    }
    public enum SkillBreakType
    {
        Move = 1,
    }
    public enum Operate_Type
    {
        // 设置绝对值，直接设置当前值
        OT_Absolute,

        // 设置相对值，即在当前基础上的增加值，可以为负数
        OT_Relative,

        // 设置相对当前值的百分比
        OT_PercentCurrent,

        // 设置相对最大值百分比，[!!!此类操作必须要求改值存在最大值，比如HP，ENERGY, MOVESPEED, SHOOTSPEED]
        OT_PercentMax,

        // 设置位
        OT_AddBit,

        // 取消位
        OT_RemoveBit,
    }
}
