using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class GameUpdateSystems : Feature
    {
        public GameUpdateSystems(Contexts contexts, Services services)
        {
            Add(IdSystem.Instance);
            Add(SceneSystem.Instance);
            Add(new ChunkSystem(contexts));
            Add(new AttrSystem(contexts));
            Add(new HpSystem(contexts));
            Add(new DeadSystem(contexts));
            Add(new BornSystem(contexts));
            Add(new AISystem(contexts));

            Add(new CleanupEntitySystem(contexts));
            Add(new DestoryEntitySystem(contexts));
            Add(new CleanResourceSystem(contexts));
        }
    }
    public class GameFixedUpdateSystems : Feature
    {
        public GameFixedUpdateSystems(Contexts contexts, Services services)
        {
            Add(SkillSystem.Instance);
            Add(BuffSystem.Instance);

            Add(new CameraSystem(contexts));
            Add(new MovementSystem(contexts));
            Add(new AnimationSystem(contexts));
            Add(new HudSystem(contexts));
        }
    }
}
