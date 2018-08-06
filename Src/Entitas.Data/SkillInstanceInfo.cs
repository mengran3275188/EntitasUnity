using System.Collections.Generic;
using ScriptableSystem;

namespace Entitas.Data
{
    public class SkillInstanceInfo
    {
        public int m_SkillId;
        public Instance m_SkillInstance;
        public bool m_IsUsed;
    }

    public class BuffInstanceInfo
    {
        public int m_BuffId;
        public Instance m_BuffInstance;
        public bool m_IsUsed;
    }
    public class SkillInfo
    {
        public int SkillId;
    }
}
