using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityClient
{
    public class ChunkFixedUpdateSystems : Feature
    {
        public ChunkFixedUpdateSystems(Contexts contexts, Services services)
        {
            Add(new ChunkSystem(contexts));
        }
    }
}
