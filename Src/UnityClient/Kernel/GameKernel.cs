using System;
using System.Collections.Generic;
using UnityDelegate;
using Util;
using System.IO;

namespace UnityClient.Kernel
{
    public sealed class GameKernel
    {
        public static void OnStart(string dataPath)
        {
            GfxMoudle.Instance.RegisteLog();

            HomePath.Instance.SetHomePath(dataPath);

            FileReaderProxy.RegisterReadFileHandler((string filePath) =>
            {
                try
                {
                    return File.ReadAllBytes(filePath);
                }
                catch (Exception e)
                {
                    LogUtil.Error("Exception:{0}\n{1}", e.Message, e.StackTrace);
                    return null;
                }
            });

            LoadConfig();

            GfxMoudle.Instance.OnStart(s_Logic);

            s_Logic.OnStart();
        }

        public static void OnTick()
        {
            s_Logic.OnTick();
            GfxMoudle.Instance.OnTick();
        }
        public static void OnQuit()
        {
            GfxMoudle.Instance.OnQuit();

            s_Logic.OnQuit();
        }
        private static void LoadConfig()
        {
            Entitas.Data.CameraConfigProvider.Instance.LoadForClient();
            Entitas.Data.ActionConfigProvider.Instance.LoadForClient();
            Entitas.Data.SceneConfigProvider.Instance.LoadForClient();
            Entitas.Data.CharacterConfigProvider.Instance.LoadForClient();
            Entitas.Data.SkillConfigProvider.Instance.LoadForClient();
            Entitas.Data.AttributeConfigProvider.Instance.LoadForClient();
            Entitas.Data.BuffConfigProvider.Instance.LoadForClient();
            Entitas.Data.BlocksProvider.Instance.LoadForClient();
        }


        private static LogicMoudle s_Logic = new LogicMoudle();
    }
}
