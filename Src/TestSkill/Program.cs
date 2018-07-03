using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityClient;
using System.IO;
using Entitas;
using Util;

namespace TestSkill
{
    class Program
    {
        static void Main(string[] args)
        {
            LogUtil.OnOutput = (Log_Type type, string msg) =>
            {
                Console.WriteLine(msg);
            };


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

            var contexts = Contexts.sharedInstance;

            var systems = new Systems();

            systems.Add(new TimeSystem(contexts));
            systems.Add(SkillSystem.Instance);

            systems.Initialize();

            var player = contexts.game.CreateEntity();

            //player.AddSkill(null);

            //SkillSystem.Instance.StartSkill(player, player, 1, player.position.Value, player.);


            while(true)
            {
                systems.Execute();
            }

        }
    }
}
