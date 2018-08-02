using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace UnityClient
{
    public abstract class Service
    {
        protected readonly Contexts _contexts;
        public Service(Contexts contexts)
        {
            _contexts = contexts;
        }
    }
    public class Services : Singleton<Services>
    {
        public UnityChunkService ChunkService = new UnityChunkService(Contexts.sharedInstance);
        public UnityViewService ViewService = new UnityViewService(Contexts.sharedInstance);
        public CreateCharacterService CreateCharacterService = new CreateCharacterService(Contexts.sharedInstance);
        public UnityCameraService CameraService = new UnityCameraService(Contexts.sharedInstance);
        public UnityHudServices HudService = new UnityHudServices(Contexts.sharedInstance);
        public UnitySceneService SceneService = new UnitySceneService(Contexts.sharedInstance);
        public UnityUIServices UIService = new UnityUIServices(Contexts.sharedInstance);
        public UnityInputServices InputService = new UnityInputServices(Contexts.sharedInstance);
        public UnityPhysicsService PhysicsService = new UnityPhysicsService(Contexts.sharedInstance);
    }
}
