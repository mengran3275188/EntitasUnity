using System;
using System.Collections.Generic;
using Entitas;
using Util;

namespace UnityClient
{
    public class AISystem : IInitializeSystem, IExecuteSystem, ITearDownSystem
    {
        public AISystem(Contexts contexts)
        {
            m_Context = contexts.game;
        }
        public void Initialize()
        {
            InitBehavic();

            m_Context.GetGroup(GameMatcher.AI).OnEntityAdded += AISystem_OnEntityAdded;
            m_Context.GetGroup(GameMatcher.AI).OnEntityRemoved += AISystem_OnEntityRemoved;
        }
        public void Execute()
        {
            var entities = m_Context.GetGroup(GameMatcher.AI).GetEntities();
            foreach(GameEntity entity in entities)
            {
                if(!entity.hasDead && !entity.hasBorn)
                    entity.aI.Agent.btexec();
            }
        }
        public void TearDown()
        {
            m_Context.GetGroup(GameMatcher.AI).OnEntityAdded -= AISystem_OnEntityAdded;
            m_Context.GetGroup(GameMatcher.AI).OnEntityRemoved -= AISystem_OnEntityRemoved;
        }
        private void AISystem_OnEntityAdded(Entitas.IGroup<GameEntity> group, GameEntity entity, int index, Entitas.IComponent component)
        {
        }
        private void AISystem_OnEntityRemoved(IGroup<GameEntity> group, GameEntity entity, int index, IComponent component)
        {
        }

        #region behaviac
        private bool InitBehavic()
        {
            LogUtil.Info("InitBehavic");

            behaviac.Workspace.Instance.FilePath = HomePath.Instance.GetAbsolutePath("Tables/Behaviac/exported/");
            behaviac.Workspace.Instance.FileFormat = behaviac.Workspace.EFileFormat.EFF_xml;

            return true;
        }

        #endregion

        private readonly GameContext m_Context;
    }
}
