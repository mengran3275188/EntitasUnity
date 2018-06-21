using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Skill
{
    public class SkillInstanceInfo
    {
        public int m_SkillId;
        public SkillInstance m_SkillInstance;
        public bool m_IsUsed;
    }
    public class ClientSkillSystem : Singleton<ClientSkillSystem>
    {
        public int ActiveStoryCount
        {
            get
            {
                return m_SkillLogicInfos.Count;
            }
        }
        internal Dictionary<string, object> GlobalVariables
        {
            get { return m_GlobalVariables; }
        }
        internal void Init()
        {
        }
        internal void Reset()
        {
            m_GlobalVariables.Clear();
            int count = m_SkillLogicInfos.Count;
            for(int index = count -1; index >= 0; --index)
            {
                SkillInstanceInfo info = m_SkillLogicInfos[index];
                if(null != info)
                {
                    RecycleSkillInstance(info);
                    m_SkillLogicInfos.RemoveAt(index);
                }
            }
            m_SkillLogicInfos.Clear();
        }

        internal void PreloadStoryInstance(int skillId)
        {
            SkillInstanceInfo info = NewSkillInstance(skillId);
            if(null != info)
            {
                RecycleSkillInstance(info);
            }
        }
        internal void ClearSkillInstancePool()
        {
            m_SkillInstancePool.Clear();
        }
        public void StartSkill(int skillId)
        {
            SkillInstanceInfo instance = NewSkillInstance(skillId);
            if(null != instance)
            {
                m_SkillLogicInfos.Add(instance);
                instance.m_SkillInstance.Context = null;
                instance.m_SkillInstance.GlobalVariables = m_GlobalVariables;
                instance.m_SkillInstance.Start();

                LogUtil.Info("StartSkill {0}.", skillId);
            }
        }
        internal void StopSkill(int skillId)
        {
            int count = m_SkillLogicInfos.Count;
            for(int index = count - 1; index >= 0; --index)
            {
                SkillInstanceInfo info = m_SkillLogicInfos[index];
                if(info.m_SkillId == skillId)
                {
                    RecycleSkillInstance(info);
                    m_SkillLogicInfos.RemoveAt(index);
                }
            }
        }
        public void Tick()
        {
            long time = TimeUtility.Instance.GetLocalMilliseconds();
            int ct = m_SkillLogicInfos.Count;
            for(int ix = ct - 1; ix >= 0; --ix)
            {
                SkillInstanceInfo info = m_SkillLogicInfos[ix];
                info.m_SkillInstance.Tick(time);
                if(info.m_SkillInstance.IsTerminated)
                {
                    RecycleSkillInstance(info);
                    m_SkillLogicInfos.RemoveAt(ix);
                }
            }
        }
        internal void SendMessage(string msgId, params object[] args)
        {
            int ct = m_SkillLogicInfos.Count;
            for(int ix = ct - 1; ix >= 0; --ix)
            {
                SkillInstanceInfo info = m_SkillLogicInfos[ix];
                info.m_SkillInstance.SendMessage(msgId, args);
            }
        }
        private SkillInstanceInfo NewSkillInstance(int skillId)
        {
            SkillInstanceInfo instanceInfo = GetUnusedSkillInstanceInfoFromPool(skillId);
            if(null == instanceInfo)
            {
                //do load
                SkillConfigManager.Instance.LoadSkillIfNotExist(skillId, 0, "test.mr");
                SkillInstance instance = SkillConfigManager.Instance.NewSkillInstance(skillId, 0);
                if(null == instance)
                {
                    LogUtil.Error("Can't load skill config, skill:{0}!", skillId);
                }
                SkillInstanceInfo res = new SkillInstanceInfo();
                res.m_SkillId = skillId;
                res.m_SkillInstance = instance;
                res.m_IsUsed = true;

                AddStoryInstanceInfoToPool(skillId, res);
                return res;
            }
            else
            {
                instanceInfo.m_IsUsed = true;
                return instanceInfo;
            }
        }
        private void RecycleSkillInstance(SkillInstanceInfo info)
        {
            info.m_SkillInstance.Reset();
            info.m_IsUsed = false;
        }
        private void AddStoryInstanceInfoToPool(int skillId, SkillInstanceInfo info)
        {
            List<SkillInstanceInfo> infos;
            if(m_SkillInstancePool.TryGetValue(skillId, out infos))
            {
                infos.Add(info);
            }
            else
            {
                infos = new List<SkillInstanceInfo>();
                infos.Add(info);
                m_SkillInstancePool.Add(skillId, infos);
            }
        }
        private SkillInstanceInfo GetUnusedSkillInstanceInfoFromPool(int skillId)
        {
            SkillInstanceInfo info = null;
            List<SkillInstanceInfo> infos;
            if(m_SkillInstancePool.TryGetValue(skillId, out infos))
            {
                foreach(var skillInfo in infos)
                {
                    if (!skillInfo.m_IsUsed)
                    {
                        info = skillInfo;
                        break;
                    }
                }
            }
            return info;
        }
        private Dictionary<string, object> m_GlobalVariables = new Dictionary<string, object>();
        private List<SkillInstanceInfo> m_SkillLogicInfos = new List<SkillInstanceInfo>();
        private Dictionary<int, List<SkillInstanceInfo>> m_SkillInstancePool = new Dictionary<int, List<SkillInstanceInfo>>();
    }
}
