using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;

namespace Entitas.Data
{
    public class RigidObject : RigidBody
    {
        public uint EntityId
        {
            get { return m_EntityId; }
        }
        public RigidObject(uint id, Shape shape, Material material) : base(shape, material, true)
        {
            m_EntityId = id;
        }
        private uint m_EntityId;
    }
}
