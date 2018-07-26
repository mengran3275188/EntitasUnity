using System;
using Entitas;
using Entitas.Data;
using ScriptableData;
using ScriptableSystem;
using UnityClient;
using Util;

namespace SceneCommand
{
    internal class CreateCharacterCommand : AbstractCommand
    {
        protected override void Load(CallData callData)
        {
            if(callData.GetParamNum() >= 3)
            {
                m_CharacterId = int.Parse(callData.GetParamId(0));

                m_LocalPosition = ScriptableDataUtility.CalcVector3(callData.GetParam(1) as ScriptableData.CallData);
                m_LocalRotation = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
            }
        }
        protected override void Load(FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null == callData)
                return;
            Load(callData);
            for(int i = 0; i < funcData.Statements.Count; ++i)
            {
                ScriptableData.CallData stCall = funcData.Statements[i] as ScriptableData.CallData;
                if (null == stCall)
                    continue;
                if(stCall.GetId() == "mainplayer")
                {
                    m_MainPlayer = true;
                }
                if(stCall.GetId() == "skill")
                {
                    if(stCall.GetParamNum() >= 1)
                    {
                        m_SkillId = int.Parse(stCall.GetParamId(0));
                    }
                }
                if(stCall.GetId() == "ai")
                {
                    if(stCall.GetParamNum() >= 1)
                    {
                        m_AIScript = stCall.GetParamId(0);
                    }
                }
                if(stCall.GetId() == "attr")
                {
                    if(stCall.GetParamNum() >= 1)
                    {
                        m_AttrId = int.Parse(stCall.GetParamId(0));
                    }
                }
                if(stCall.GetId() == "physics")
                {
                    if(stCall.GetParamNum() == 3)
                    {
                        m_PhysicsType = PhysicsType.Box;
                        m_PhysicsIsTrigger = bool.Parse(stCall.GetParamId(0));
                        m_PhysicsOffset = ScriptableDataUtility.CalcVector3(stCall.GetParam(1) as CallData);
                        m_PhysicsSize = ScriptableDataUtility.CalcVector3(stCall.GetParam(2) as CallData);

                    }else if(stCall.GetParamNum() == 4)
                    {
                        m_PhysicsType = PhysicsType.Capsule;
                        m_PhysicsIsTrigger = bool.Parse(stCall.GetParamId(0));
                        m_PhysicsOffset = ScriptableDataUtility.CalcVector3(stCall.GetParam(1) as CallData);

                        m_PhysicsLength = float.Parse(stCall.GetParamId(2));
                        m_PhysicsRadius = float.Parse(stCall.GetParamId(3));
                        m_PhysicsLength -= m_PhysicsRadius * 2;

                        if (m_PhysicsLength < 0)
                            m_PhysicsLength = 0;
                    }
                }
            }
        }
        protected override ExecResult ExecCommand(Instance instance, long delta)
        {
            var gameContext = Contexts.sharedInstance.game;

            var target = instance.Target as GameEntity;
            if (null == target)
                return ExecResult.Finished;

            CharacterConfig config = CharacterConfigProvider.Instance.GetCharacterConfig(m_CharacterId);
            if(null != config)
            {
                uint entityId = IdSystem.Instance.GenId(IdEnum.Entity);
                GameEntity entity = gameContext.CreateEntity();
                entity.AddId(entityId);

                entity.isMainPlayer = m_MainPlayer;

                // res
                uint resId = IdSystem.Instance.GenId(IdEnum.Resource);
                GfxSystem.Instantiate(resId, config.Model);
                entity.AddResource(resId);

                // animation
                entity.AddAnimation(config.ActionId, config.ActionPrefix);

                // movement
                Quaternion quaternion = Quaternion.CreateFromYawPitchRoll(target.rotation.Value, 0, 0);
                entity.AddMovement(Vector3.zero);
                entity.AddPosition(target.position.Value + quaternion * m_LocalPosition);
                entity.AddRotation(m_LocalRotation.y + target.rotation.Value);

                // AI
                if(!string.IsNullOrEmpty(m_AIScript))
                {
                    var agent = new CharacterAgent();
                    agent.Init(entityId);
                    bool ret = agent.btload(m_AIScript);
                    agent.btsetcurrent(m_AIScript);
                    entity.AddAI(agent);
                }
                // skill
                entity.AddSkill(null, null);
                entity.AddBuff(new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<BuffInstanceInfo>>(), new System.Collections.Generic.List<StartBuffParam>());

                // 考虑采用skillinputcomponent类似的形式替换这种直接释放技能的形式。减少依赖。
                if(m_SkillId > 0)
                {
                    SkillSystem.Instance.StartSkill(target, entity, m_SkillId, target.position.Value, target.rotation.Value);
                }

                if(m_PhysicsType == PhysicsType.Box)
                {
                    Jitter.Collision.Shapes.BoxShape shape = new Jitter.Collision.Shapes.BoxShape(m_PhysicsSize);

                    Jitter.Dynamics.Material physicsMaterial = new Jitter.Dynamics.Material();
                    physicsMaterial.KineticFriction = 0;
                    physicsMaterial.StaticFriction = 0;

                    RigidObject rigid = new RigidObject(entity.id.value, shape, physicsMaterial, false);
                    rigid.Mass = 1000;
                    rigid.IsTrigger = m_PhysicsIsTrigger;
                    rigid.Position = entity.position.Value + m_PhysicsOffset;

                    entity.AddPhysics(rigid, m_PhysicsOffset);
                }else if(m_PhysicsType == PhysicsType.Capsule)
                {
                    Jitter.Collision.Shapes.CapsuleShape shape = new Jitter.Collision.Shapes.CapsuleShape(m_PhysicsLength, m_PhysicsRadius);

                    Jitter.Dynamics.Material physicsMaterial = new Jitter.Dynamics.Material();
                    physicsMaterial.KineticFriction = 0;
                    physicsMaterial.StaticFriction = 0;

                    RigidObject rigid = new RigidObject(entity.id.value, shape, physicsMaterial, true);
                    rigid.IsTrigger = m_PhysicsIsTrigger;
                    rigid.Position = entity.position.Value + m_PhysicsOffset;

                    entity.AddPhysics(rigid, m_PhysicsOffset);
                }

                // camp id
                int campId = entity.isMainPlayer ? (int)CampId.Red : (int)CampId.Blue;
                if (target.hasCamp)
                    campId = target.camp.Value;
                entity.AddCamp(campId);

                // attribute
                if(m_AttrId > 0)
                {
                    GfxSystem.CreateHudHead(entity.resource.Value);

                    AttributeConfig attrConfig = AttributeConfigProvider.Instance.GetAttributeConfig(m_AttrId);
                    if(null != attrConfig)
                    {
                        AttributeData attrData = new AttributeData();
                        attrData.SetAbsoluteByConfig(attrConfig);
                        entity.AddAttr(m_AttrId, attrData);
                        entity.ReplaceHp(attrData.HpMax);
                    }
                }

                if(!entity.isMainPlayer)
                    entity.AddBorn(Contexts.sharedInstance.game.timeInfo.Time);
            }
            else
            {
                LogUtil.Error("CreateCharacterCommand.ExecCommand : character config {0} not found!", m_CharacterId);
            }

            return ExecResult.Finished;
        }
        private int m_CharacterId;
        private bool m_MainPlayer = false;
        private Vector3 m_LocalPosition;
        private Vector3 m_LocalRotation;
        private int m_SkillId = 0;
        private string m_AIScript = string.Empty;
        private int m_AttrId = 0;

        private Vector3 m_PhysicsOffset = Vector3.zero;
        private Vector3 m_PhysicsSize = Vector3.one;
        private float m_PhysicsLength = 0;
        private float m_PhysicsRadius = 0;
        private PhysicsType m_PhysicsType = PhysicsType.Invalid;
        private bool m_PhysicsIsTrigger = false;

        private enum PhysicsType
        {
            Invalid,
            Box,
            Capsule,
        }
    }
}
