using System.Collections.Generic;
using ScriptableData;

namespace Entitas.Data
{
    public class SkillInstanceInfo
    {
        public List<BreakSection> BreakSections;
        public int SkillId;
        public int Category;
        public IInstance SkillInstance;
        public bool IsUsed;
    }

    public class BuffInstanceInfo
    {
        public int BuffId;
        public IInstance BuffInstance;
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
