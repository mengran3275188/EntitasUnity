using System.Collections.Generic;

namespace Entitas.Data
{
    public sealed class SkillComponent : IComponent
    {
        public SkillInstanceInfo Instance;
    }
    public sealed class BuffComponent : IComponent
    {
        public List<BuffInstanceInfo> InstanceInfos;
    }
}
