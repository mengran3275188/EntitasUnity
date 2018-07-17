using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using UnityDelegate;
using Util;

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
            GfxSystem.ListenKeyPressState(
                Keyboard.Code.W,
                Keyboard.Code.A,
                Keyboard.Code.S,
                Keyboard.Code.D);

            GfxSystem.ListenKeyboardEvent(Keyboard.Code.H, this.MainPlayerUseSkillH);
            GfxSystem.ListenKeyboardEvent(Keyboard.Code.J, this.MainPlayerUseSkillJ);
            GfxSystem.ListenKeyboardEvent(Keyboard.Code.K, this.MainPlayerUseSkillK);
            GfxSystem.ListenKeyboardEvent(Keyboard.Code.L, this.MainPlayerUseSkillL);
            GfxSystem.ListenKeyboardEvent(Keyboard.Code.Space, this.MainPlayerUseSkillSpace);
        }
        public void Execute()
        {
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
                if(mainPlayer.hasMovement)
                {
                    if(!mainPlayer.isDisableMoveControl)
                    {
                        if(mainPlayer.hasAttr)
                        {
                            float moveSpeed = isMoving ? mainPlayer.attr.Value.MoveSpeed : 0;
                            mainPlayer.ReplaceMovement(new Vector3(Mathf.Sin(moveDir), 0, Mathf.Cos(moveDir)) * moveSpeed);
                        }
                    }
                }
                if(isMoving && !Util.Mathf.Approximately(moveDir, mainPlayer.rotation.Value))
                {
                    if(!mainPlayer.isDisableRotationControl)
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
        private KeyHit GetKeyboadHit()
        {
            KeyHit kh = KeyHit.None;
            if (GfxSystem.IsKeyPressed(Keyboard.Code.W))
                kh |= KeyHit.Up;
            if (GfxSystem.IsKeyPressed(Keyboard.Code.A))
                kh |= KeyHit.Left;
            if (GfxSystem.IsKeyPressed(Keyboard.Code.S))
                kh |= KeyHit.Down;
            if (GfxSystem.IsKeyPressed(Keyboard.Code.D))
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
