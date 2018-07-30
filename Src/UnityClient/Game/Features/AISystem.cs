using System;
using System.Collections.Generic;
using Entitas;
using Util;

namespace UnityClient
{
    public class AISystem : IInitializeSystem, IExecuteSystem
    {
        public AISystem(Contexts contexts)
        {
            m_Context = contexts.game;
        }
        public void Initialize()
        {
            InitBehavic();

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
