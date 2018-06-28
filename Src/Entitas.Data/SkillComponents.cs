using System.Collections.Generic;
using Entitas.Data;

namespace Entitas.Component
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
