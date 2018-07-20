using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityDelegate;

namespace UnityClient
{
    internal sealed class GfxSystemImpl
    {
        internal static GfxMoudle Gfx
        {
            get { return s_Gfx; }
        }
        private readonly static GfxMoudle s_Gfx =  GfxMoudle.Instance;
    }
}
