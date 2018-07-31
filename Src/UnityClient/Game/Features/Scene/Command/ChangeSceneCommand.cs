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
            if(Contexts.sharedInstance.gameState.hasTargetSceneId)
            {
                Contexts.sharedInstance.game.isCleanup = true;

                int sceneId = Contexts.sharedInstance.gameState.targetSceneId.Value;
                Contexts.sharedInstance.gameState.ReplaceNextSceneId(sceneId);
            }
            return ExecResult.Finished;
        }
    }
}
