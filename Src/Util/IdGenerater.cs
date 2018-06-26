using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public sealed class IdGenerator
    {
        public uint NextId()
        {
            return m_NextId++;
        }
        private uint m_NextId = 1;
    }
}
