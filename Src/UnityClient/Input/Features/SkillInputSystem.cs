using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Data;

namespace UnityClient
{
    public class SkillInputSystem : IInitializeSystem
    {
        public SkillInputSystem(Contexts contexts)
        {
        }
        public void Initialize()
        {
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.H, this.OnKeyboardEvent);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.J, this.OnKeyboardEvent);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.K, this.OnKeyboardEvent);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.L, this.OnKeyboardEvent);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.Space, this.OnKeyboardEvent);
        }
        private void OnKeyboardEvent(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                var keybings = Contexts.sharedInstance.input.skillKeyBinding.Value;
                int skillId = 0;
                if(keybings.TryGetValue((Keyboard.Code)keyCode, out skillId))
                {
                    var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                    if(null != mainPlayer)
                    {
                        SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, skillId, mainPlayer.position.Value, mainPlayer.rotation.Value);
                    }
                }
            }
        }
    }
}
