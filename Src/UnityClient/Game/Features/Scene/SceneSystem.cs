using System;
using System.Collections.Generic;
using Util;
using Entitas;
using Entitas.Data;
using ScriptableSystem;
using UnityEngine;

namespace UnityClient
{
    public class SceneSystem : Singleton<SceneSystem>, IInitializeSystem, IExecuteSystem
    {
        public void Initialize()
        {
            CommandManager.Instance.RegisterCommandFactory("createcharacter", new CommandFactoryHelper<SceneCommand.CreateCharacterCommand>());
            CommandManager.Instance.RegisterCommandFactory("loadui", new CommandFactoryHelper<SceneCommand.LoadUICommand>());
            CommandManager.Instance.RegisterCommandFactory("changescene", new CommandFactoryHelper<SceneCommand.ChangeSceneCommand>());

        }
        public void Load(int id)
        {

            var config = SceneConfigProvider.Instance.GetSceneConfig(id);
            if(null != config)
            {
                Contexts.sharedInstance.game.ReplaceScene(null, null);
                //SpatialSystem.Instance.Load(config.Navmesh);
                var sceneInstance = NewSceneInstance(id, config.Script);
                // scriptsystem 归一化
                // script system 依赖一个拥有transform 信息的entity.
                Contexts.sharedInstance.game.ReplaceScene(config, sceneInstance);
                Contexts.sharedInstance.game.sceneEntity.ReplacePosition(Vector3.zero);
                Contexts.sharedInstance.game.sceneEntity.ReplaceRotation(0);

                uint entityId = IdSystem.Instance.GenId(IdEnum.Entity);
                Contexts.sharedInstance.game.sceneEntity.ReplaceId(entityId);
                Services.Instance.SceneService.InitChunks();

                sceneInstance.m_SceneInstance.Start();
            }
            else
            {
                LogUtil.Error("SceneSystem.Load : scene config {0} not found ！", id);
            }
        }
        public void Execute()
        {
            var context = Contexts.sharedInstance.game;
            long time = (long)(Contexts.sharedInstance.input.time.Value * 1000);

            if(context.hasScene && null != context.scene.ScriptInstance)
            {
                context.scene.ScriptInstance.m_SceneInstance.Tick(time);
            }
        }
        public void SendMessage(string messageId)
        {
            Contexts.sharedInstance.game.scene.ScriptInstance.m_SceneInstance.SendMessage(messageId);
        }

        private SceneInstanceInfo NewSceneInstance(int sceneId, string scriptPath)
        {
            SceneInstanceInfo instanceInfo = GetUnusedSceneInstanceInfoFromPool(sceneId);
            if(null == instanceInfo)
            {
                ConfigManager.Instance.LoadIfNotExist(sceneId, 1, HomePath.Instance.GetAbsolutePath(scriptPath));
                Instance instance = ConfigManager.Instance.NewInstance(sceneId, 1);
                if(null == instance)
                {
                    LogUtil.Error("SceneSystem::NewSceneInstance scene script {0} not found!", sceneId);
                }
                instance.SenderId = 0;
                instance.Target = Contexts.sharedInstance.game.sceneEntity;

                SceneInstanceInfo sii = new SceneInstanceInfo();
                sii.m_SceneId = sceneId;
                sii.m_SceneInstance = instance;
                sii.m_IsUsed = true;

                AddStoryInstanceInfoToPool(sceneId, sii);
                return sii;
            }
            else
            {
                instanceInfo.m_IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleSceneInstance(SceneInstanceInfo info)
        {
            info.m_SceneInstance.Reset();
            info.m_IsUsed = false;
        }
        private void AddStoryInstanceInfoToPool(int sceneId, SceneInstanceInfo info)
        {
            List<SceneInstanceInfo> infos;
            if(m_SceneInstancePool.TryGetValue(sceneId, out infos))
            {
                infos.Add(info);
            }
            else
            {
                infos = new List<SceneInstanceInfo>();
                infos.Add(info);
                m_SceneInstancePool.Add(sceneId, infos);
            }
        }
        private SceneInstanceInfo GetUnusedSceneInstanceInfoFromPool(int sceneId)
        {
            SceneInstanceInfo info = null;
            List<SceneInstanceInfo> infos;
            if(m_SceneInstancePool.TryGetValue(sceneId, out infos))
            {
                foreach(var sceneInfo in infos)
                {
                    if(!sceneInfo.m_IsUsed)
                    {
                        info = sceneInfo;
                        break;
                    }
                }
            }
            return info;
        }

        private readonly List<SceneInstanceInfo> m_SceneLogicInfos = new List<SceneInstanceInfo>();
        private Dictionary<int, List<SceneInstanceInfo>> m_SceneInstancePool = new Dictionary<int, List<SceneInstanceInfo>>();
    }
}
