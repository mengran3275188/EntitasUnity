using System;
using System.Collections.Generic;
using Util;
using Entitas;
using Entitas.Data;
using ScriptableSystem;

namespace UnityClient
{
    public class SceneSystem : Singleton<SceneSystem>, IInitializeSystem, IExecuteSystem
    {
        public void Initialize()
        {
            CommandManager.Instance.RegisterCommandFactory("createcharacter", new CommandFactoryHelper<SceneCommand.CreateCharacterCommand>());

            // scriptsystem 归一化
            // script system 依赖一个拥有transform 信息的entity.
            Contexts.sharedInstance.game.SetScene(null, null);
            Contexts.sharedInstance.game.sceneEntity.AddPosition(Vector3.zero);
            Contexts.sharedInstance.game.sceneEntity.AddRotation(RotateState.UserRotate, 0);
        }
        public void Load(int id)
        {
            var config = SceneConfigProvider.Instance.GetSceneConfig(id);
            if(null != config)
            {
                SpatialSystem.Instance.Init(config.Navmesh);
                var sceneInstance = NewSceneInstance(id, config.Script);
                sceneInstance.m_SceneInstance.Start();
                Contexts.sharedInstance.game.ReplaceScene(config, sceneInstance);

            }
            else
            {
                LogUtil.Error("SceneSystem.Load : scene config {0} not found ！", id);
            }
        }
        public void Execute()
        {
            var context = Contexts.sharedInstance.game;
            long time = (long)(context.timeInfo.Time * 1000);

            var scene = context.scene;
            if(null != scene && null != scene.ScriptInstance)
            {
                scene.ScriptInstance.m_SceneInstance.Tick(time);
            }
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
                instance.Sender = Contexts.sharedInstance.game.sceneEntity;
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

        private List<SceneInstanceInfo> m_SceneLogicInfos = new List<SceneInstanceInfo>();
        private Dictionary<int, List<SceneInstanceInfo>> m_SceneInstancePool = new Dictionary<int, List<SceneInstanceInfo>>();
    }
}
