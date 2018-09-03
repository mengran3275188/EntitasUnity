using System;
using System.Collections.Generic;
using Util;

namespace ScriptableData.SkillScripts
{
    public class SkillScriptsManager : Singleton<SkillScriptsManager>
    {
        public bool LoadIfNotExits(int id, int type)
        {
            int finalId = GenId(id, type);
            ISkillScript script;
            if(m_Scripts.TryGetValue(finalId, out script))
            {
                ConfigManager.Instance.AddInstanceResource(finalId, script.GetSkillInstance());
                return true;
            }
            return false;
        }
        public void RegisterScript(int id, int type, ISkillScript script)
        {
            m_Scripts[GenId(id, type)] = script;
        }
        private static int GenId(int id, int type)
        {
            return id * 100 + type;
        }

        private Dictionary<int, ISkillScript> m_Scripts = new Dictionary<int, ISkillScript>();
    }
}
