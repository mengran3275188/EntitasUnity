using Entitas.CodeGeneration.Attributes;
using Entitas.Data;

namespace Entitas.Component
{
    public sealed class SkillComponent : IComponent
    {
        public SkillInstanceInfo Instance;
    }
    public sealed class ImpactComponent : IComponent
    {
        public ImpactInstanceInfo Instance;
    }
}
