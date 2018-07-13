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

            GfxSystem.ListenKeyboardEvent(Keyboard.Code.J, this.MainPlayerUseSkillJ);
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
            gameContext.ReplaceInput(isMoving, moveDir);


            GameEntity mainPlayer = gameContext.mainPlayerEntity;
            if (null != mainPlayer)
            {
                if(mainPlayer.hasMovement && mainPlayer.movement.State != MoveState.SkillMoving && mainPlayer.movement.State != MoveState.ImpactMoving)
                {
                    if(mainPlayer.hasAttr)
                    {
                        float moveSpeed = mainPlayer.attr.Value.MoveSpeed;
                        mainPlayer.ReplaceMovement(isMoving ? MoveState.UserMoving : MoveState.Idle, new Vector3(Mathf.Sin(moveDir), 0, Mathf.Cos(moveDir)) * moveSpeed);
                    }
                }
                if(isMoving && mainPlayer.hasRotation && mainPlayer.rotation.State != RotateState.SkillRotate && mainPlayer.rotation.State != RotateState.ImpactRotate && !Util.Mathf.Approximately(moveDir, mainPlayer.rotation.RotateDir))
                {
                    mainPlayer.ReplaceRotation(RotateState.UserRotate, moveDir);
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
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 1, mainPlayer.position.Value, mainPlayer.rotation.RotateDir);
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
                    SkillSystem.Instance.StartSkill(mainPlayer, mainPlayer, 2, mainPlayer.position.Value, mainPlayer.rotation.RotateDir);
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
