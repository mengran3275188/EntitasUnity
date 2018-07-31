using System;
using System.Collections.Generic;
using ScriptableData;
using ScriptableSystem;
using UnityClient;
using Entitas.Data;

namespace SceneCommand
{
    internal class ChangeSceneCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            if(Contexts.sharedInstance.gameState.hasNextSceneId)
            {
                int sceneId = Contexts.sharedInstance.gameState.nextSceneId.Value;

                SceneConfig config = SceneConfigProvider.Instance.GetSceneConfig(sceneId);
                if(null != config)
                {
                    Services.Instance.SceneService.LoadSceneAsync(config.Id, config.Name);
                }

            }

            return ExecResult.Finished;
        }
    }
}
