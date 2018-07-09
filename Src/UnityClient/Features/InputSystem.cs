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
            m_MainPlayers = contexts.game.GetGroup(GameMatcher.MainPlayer);
        }
        public void Initialize()
        {
            GfxSystem.ListenKeyPressState(
                Keyboard.Code.W,
                Keyboard.Code.A,
                Keyboard.Code.S,
                Keyboard.Code.D);
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
                    mainPlayer.ReplaceMovement(isMoving ? MoveState.UserMoving : MoveState.Idle, new Vector3(Mathf.Sin(moveDir), 0, Mathf.Cos(moveDir)) * 10);
                }
                if(isMoving && mainPlayer.hasRotation && mainPlayer.rotation.State != RotateState.SkillRotate && mainPlayer.rotation.State != RotateState.ImpactRotate && !Util.Mathf.Approximately(moveDir, mainPlayer.rotation.RotateDir))
                {
                    mainPlayer.ReplaceRotation(RotateState.UserRotate, moveDir);
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
        private readonly IGroup<GameEntity> m_MainPlayers;

        private const float c_2PI = (float)Math.PI * 2;
    }
}
