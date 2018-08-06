using System.Collections.Generic;

namespace Entitas.Data
{
    public sealed class SkillConfigComponent : IComponent
    {
        public List<SkillInfo> Skills;
    }
    public sealed class SkillComponent : IComponent
    {
        public SkillInstanceInfo Instance;
        public StartSkillParam StartParam;
    }
    public sealed class BuffComponent : IComponent
    {
        public Dictionary<int, List<BuffInstanceInfo>> InstanceInfos;
        public List<StartBuffParam> StartParams;
    }
}
