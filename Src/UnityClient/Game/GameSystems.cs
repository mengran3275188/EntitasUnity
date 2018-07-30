using System;
using System.Collections.Generic;
using Entitas;

namespace UnityClient
{
    public class GameLogicSystems : Feature
    {
        public GameLogicSystems(Contexts contexts, Services services)
        {
            Add(new TimeSystem(contexts));
            Add(new InputSystem(contexts));
            Add(IdSystem.Instance);
            Add(SceneSystem.Instance);
            Add(new ChunkSystem(contexts));
            Add(new AttrSystem(contexts));
            Add(new HpSystem(contexts));
            Add(new DeadSystem(contexts));
            Add(new BornSystem(contexts));
            Add(new AISystem(contexts));
            Add(SkillSystem.Instance);
            Add(BuffSystem.Instance);

            Add(new DestoryEntitySystem(contexts));
        }
    }
    public class GameViewSystems : Feature
    {
        public GameViewSystems(Contexts contexts, Services services)
        {
            Add(new CameraSystem(contexts));
            Add(new MovementSystem(contexts));
            Add(new AnimationSystem(contexts));
            Add(new HudSystem(contexts));
        }
    }
}
