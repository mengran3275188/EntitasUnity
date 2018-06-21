using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.Data;
using Util;

namespace UnityClient
{
    public class GameStartSystem : IInitializeSystem
    {
        readonly GameContext game;
        public GameStartSystem(Contexts contexts)
        {
            game = contexts.game;
        }

        public void Initialize()
        {
            uint resId = IdGenerater.Instance.NextId();
            GfxSystem.Instantiate(resId, "");
            GameEntity entity = game.CreateEntity();
            entity.isMainPlayer = true;
            entity.AddMovement(MoveState.Idle, 0, 0);
            entity.AddResource(resId);
            Vector3 pos = SpatialSystem.Instance.GetNearestWalkablePos(new Vector3(0, 0, 0));
            entity.AddPosition(pos.x, pos.y, pos.z);
            entity.AddRotation(RotateState.UserRotate, 0);
            entity.AddSkill(null);
            LoadConfig();
        }

        private void LoadConfig()
        {
            PlayerConfigProvider.Instance.LoadForClient();
            LogUtil.Debug(PlayerConfigProvider.Instance.GetPlayerConfigCount().ToString());
        }
    }
}
