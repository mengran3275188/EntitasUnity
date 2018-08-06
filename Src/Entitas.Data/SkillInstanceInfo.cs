using System.Collections.Generic;
using ScriptableSystem;

namespace Entitas.Data
{
    public class SkillInstanceInfo
    {
        public List<BreakSection> BreakSections;
        public int SkillId;
        public Instance SkillInstance;
        public bool IsUsed;
    }

    public class BuffInstanceInfo
    {
        public int BuffId;
        public Instance BuffInstance;
        public bool IsUsed;
    }
    public class SkillInfo
    {
        public int SkillId;
    }
    public class BreakSection
    {
        public int BreakType;
        public long StartTime;
        public long EndTime;
    }
}
