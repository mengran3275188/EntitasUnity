using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    // read input and write to global InputComponent
    public class InputSystem : IInitializeSystem, IExecuteSystem
    {
        internal enum KeyHit
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8,
            Other = 16,
        }
        public InputSystem(Contexts contexts)
        {
        }
        public void Initialize()
        {
            Services.Instance.InputService.ListenKeyPressState(
                Keyboard.Code.W,
                Keyboard.Code.A,
                Keyboard.Code.S,
                Keyboard.Code.D);

            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.H, this.MainPlayerUseSkillH);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.J, this.MainPlayerUseSkillJ);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.K, this.MainPlayerUseSkillK);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.L, this.MainPlayerUseSkillL);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.M, this.ChangeScene);
            Services.Instance.InputService.ListenKeyboardEvent(Keyboard.Code.Space, this.MainPlayerUseSkillSpace);
        }
        public void Execute()
        {
            Services.Instance.InputService.Tick();
            // keyboard
            KeyHit kh = GetKeyboadHit();

            float moveDir = CalcMoveDir(kh);
            bool isMoving = moveDir >= 0;

            if(!isMoving)
                isMoving = GfxSystem.GetJoyStickDir(out moveDir);


            GameContext gameContext = Contexts.sharedInstance.game;

            GameEntity mainPlayer = gameContext.mainPlayerEntity;
            if (null != mainPlayer)
            {
                if (mainPlayer.hasMovement)
                {
                    if(!(SkillSystem.Instance.IsDisableMoveInput(mainPlayer) || BuffSystem.Instance.IsDisableMoveInput(mainPlayer)))
                    {
                        if (mainPlayer.hasAttr)
                        {
                            float moveSpeed = isMoving ? mainPlayer.attr.Value.MoveSpeed : 0;
                            mainPlayer.ReplaceMovement(new Vector3(Mathf.Sin(moveDir), 0, Mathf.Cos(moveDir)) * moveSpeed);
                        }
                    }
                }
                if (isMoving && !Mathf.Approximately(moveDir, mainPlayer.rotation.Value))
                {
                    if(!(SkillSystem.Instance.IsDisableRotationInput(mainPlayer) || BuffSystem.Instance.IsDisableRotationInput(mainPlayer)))
                        mainPlayer.ReplaceRotation(moveDir);
                }
            }
        }

        private void MainPlayerUseSkillH(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                if(null != mainPlayer)
                {
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 1, mainPlayer.position.Value, mainPlayer.rotation.Value);
                }
            }
        }
        private void MainPlayerUseSkillJ(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                if(null != mainPlayer)
                {
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 2, mainPlayer.position.Value, mainPlayer.rotation.Value);
                }
            }
        }
        private void MainPlayerUseSkillK(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                if(null != mainPlayer)
                {
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 3, mainPlayer.position.Value, mainPlayer.rotation.Value);
                }
            }
        }
        private void MainPlayerUseSkillL(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                if(null != mainPlayer)
                {
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 4, mainPlayer.position.Value, mainPlayer.rotation.Value);
                }
            }
        }
        private void MainPlayerUseSkillSpace(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                var mainPlayer = Contexts.sharedInstance.game.mainPlayerEntity;
                if(null != mainPlayer)
                {
                    SkillSystem.Instance.BreakSkill(mainPlayer);
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 5, mainPlayer.position.Value, mainPlayer.rotation.Value);
                }
            }
        }
        private void ChangeScene(int keyCode, int what)
        {
            if((int)Keyboard.Event.Down == what)
            {
                Contexts.sharedInstance.game.isCleanup = true;
                Contexts.sharedInstance.gameState.ReplaceNextSceneId(2);
                Contexts.sharedInstance.gameState.ReplaceTargetSceneId(3);
            }
        }
        private KeyHit GetKeyboadHit()
        {
            KeyHit kh = KeyHit.None;
            if (Services.Instance.InputService.IsKeyPressed(Keyboard.Code.W))
                kh |= KeyHit.Up;
            if (Services.Instance.InputService.IsKeyPressed(Keyboard.Code.A))
                kh |= KeyHit.Left;
            if (Services.Instance.InputService.IsKeyPressed(Keyboard.Code.S))
                kh |= KeyHit.Down;
            if (Services.Instance.InputService.IsKeyPressed(Keyboard.Code.D))
                kh |= KeyHit.Right;

            return kh;
        }
        private float CalcMoveDir(KeyHit kh)
        {
            return s_MoveDirs[(int)kh];
        }


        private static readonly float[] s_MoveDirs = new float[] { -1,  0, (float)Math.PI, -1, 3*(float)Math.PI/2, 7*(float)Math.PI/4, 5*(float)Math.PI/4,
                            3*(float)Math.PI/2, (float)Math.PI/2, (float)Math.PI/4, 3*(float)Math.PI/4, (float)Math.PI/2, -1, 0,   (float)Math.PI, -1 };

        private const float c_2PI = (float)Math.PI * 2;
    }
}
