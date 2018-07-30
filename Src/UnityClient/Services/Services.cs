using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace UnityClient
{
    public class Services : Singleton<Services>
    {
        public SceneService SceneService = new SceneService(Contexts.sharedInstance);
        public UnityChunkService ChunkService = new UnityChunkService(Contexts.sharedInstance);
        public UnityViewService ViewService = new UnityViewService(Contexts.sharedInstance);
        public CreateCharacterService CreateCharacterService = new CreateCharacterService(Contexts.sharedInstance);
        public UnityCameraService CameraService = new UnityCameraService(Contexts.sharedInstance);
        public UnityHudServices HudService = new UnityHudServices(Contexts.sharedInstance);
    }
}
