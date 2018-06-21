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


            s_Logic.OnStart();
            GfxMoudle.Instance.OnStart(s_Logic);
        }

        public static void OnTick()
        {
            s_Logic.OnTick();
            GfxMoudle.Instance.OnTick();
        }
        public static void OnQuit()
        {
        }


        private static LogicMoudle s_Logic = new LogicMoudle();
    }
}
