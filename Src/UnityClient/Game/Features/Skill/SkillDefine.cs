using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityClient
{
    internal enum SkillResourceEnum
    {
        Resource = 0,
        Collider = 1,
    }
    internal enum StateBuff_State
    {
        Default = 0,
        Skill = 1,
    }
    internal class StateBuff
    {
        public StateBuff_State m_State;
        public List<int> m_Buffs = new List<int>();
    }
}
