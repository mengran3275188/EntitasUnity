using System;
using ScriptableData;

namespace SceneCommand
{
    internal class ChangeSceneCommand : AbstractCommand
    {
        protected override ExecResult ExecCommand(IInstance instance, long delta)
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
