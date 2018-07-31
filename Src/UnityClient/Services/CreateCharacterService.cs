using System;
using System.Collections.Generic;
using Entitas.Data;
using UnityEngine;

namespace UnityClient
{
    public class CreateCharacterService : Service
    {
        public CreateCharacterService(Contexts contexts) : base(contexts)
        {
        }
        public void CreateNpc(uint id, int characterId, int campId, Vector3 position, float rotation)
        {
            var e = CreateCharacter(id, characterId, campId, position, rotation);
            if (null != e)
                e.isNpc = true;
        }
        public void CreatePlayer(uint id, int characterId, int campId, Vector3 position, float rotation)
        {
            var e = CreateCharacter(id, characterId, campId, position, rotation);
            if (null != e)
                e.isMainPlayer = true;
        }
        private GameEntity CreateCharacter(uint id, int characterId, int campId, Vector3 position, float rotation)
        {
            CharacterConfig config = CharacterConfigProvider.Instance.GetCharacterConfig(characterId);
            if (null != config)
            {
                var e = _contexts.game.CreateEntity();
                e.AddId(id);

                // res
                uint resId = IdSystem.Instance.GenId(IdEnum.Resource);
                Services.Instance.ViewService.LoadAsset(e, resId, config.Model);
                e.AddResource(resId);

                // animation
                e.AddAnimation(config.ActionId, config.ActionPrefix);

                // movement
                e.AddMovement(Vector3.zero);
                Quaternion quaternion = Quaternion.Euler(0, rotation, 0);
                e.physics.Rigid.Position = position;
                e.AddPosition(position);
                e.AddRotation(rotation);

                // AI
                if (!string.IsNullOrEmpty(config.AIScript))
                {
                    var agent = new CharacterAgent();
                    agent.Init(id);
                    bool ret = agent.btload(config.AIScript);
                    agent.btsetcurrent(config.AIScript);
                    e.AddAI(agent);
                }

                // skill
                e.AddSkill(null, null);
                e.AddBuff(new Dictionary<int, List<BuffInstanceInfo>>(), new List<StartBuffParam>());

                // camp
                e.AddCamp(campId);

                AttributeConfig attrConfig = AttributeConfigProvider.Instance.GetAttributeConfig(config.AttrId);
                if (null != attrConfig)
                {
                    AttributeData attrData = new AttributeData();
                    attrData.SetAbsoluteByConfig(attrConfig);
                    e.AddAttr(config.AttrId, attrData);
                    e.ReplaceHp(attrData.HpMax);

                    Services.Instance.HudService.AddHudHead(e);
                }

                e.AddBorn(Contexts.sharedInstance.gameState.timeInfo.Time);

                return e;
            }
            return null;
        }
    }
}
