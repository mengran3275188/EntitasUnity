using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util;

namespace Skill
{
  public sealed class SkillConfigManager : Singleton<SkillConfigManager>
  {
    public void LoadSkillIfNotExist(int skillId, int sceneId, params string[] files)
    {
      if (!ExistSkill(skillId, sceneId)) {
        for (int i = 0; i < files.Length; i++ )
        {
          LoadSkill(files[i], sceneId);
        }
        /*
        foreach (string file in files) {
          LoadStory(file, sceneId);
        }*/
      }
    }
    public bool ExistSkill(int skillId, int sceneId)
    {
      int id = GenId(skillId, sceneId);
      return null != GetSkillInstanceResource(id);
    }
    public void LoadSkill(string file, int sceneId)
    {
      if (!string.IsNullOrEmpty(file)) {
        ScriptableData.ScriptableDataFile dataFile = new ScriptableData.ScriptableDataFile();
        if (dataFile.Load(file)) {
          Load(dataFile, sceneId);
        }
//#if DEBUG
//#else
//        dataFile.LoadObfuscatedFile(file, GlobalVariables.Instance.DecodeTable);
//        Load(dataFile, sceneId);
//#endif
      }
    }
    public void LoadSkillText(string text, int sceneId)
    {
      ScriptableData.ScriptableDataFile dataFile = new ScriptableData.ScriptableDataFile();
      if(dataFile.LoadFromString(text,"story")) {
        Load(dataFile, sceneId);
      }
//#if DEBUG
//#else
//      dataFile.LoadObfuscatedCode(text, GlobalVariables.Instance.DecodeTable);
//      Load(dataFile, sceneId);
//#endif
    }
    public SkillInstance NewSkillInstance(int storyId, int sceneId)
    {
      SkillInstance instance = null;
      int id = GenId(storyId, sceneId);
      SkillInstance temp = GetSkillInstanceResource(id);
      if (null != temp) {
        instance = temp.Clone();
      }
      return instance;
    }
    public void Clear()
    {
      lock (m_Lock) {
        m_SkillInstances.Clear();
      }
    }

    private void Load(ScriptableData.ScriptableDataFile dataFile, int sceneId)
    {
      lock (m_Lock) {
        for (int i = 0; i < dataFile.ScriptableDatas.Count; i++)
        {
          if (dataFile.ScriptableDatas[i].GetId() == "skill" || dataFile.ScriptableDatas[i].GetId() == "script")
          {
            ScriptableData.FunctionData funcData = dataFile.ScriptableDatas[i].First;
            if (null != funcData)
            {
              ScriptableData.CallData callData = funcData.Call;
              if (null != callData && callData.HaveParam())
              {
                int storyId = int.Parse(callData.GetParamId(0));
                int id = GenId(storyId, sceneId);
                if (!m_SkillInstances.ContainsKey(id))
                {
                  SkillInstance instance = new SkillInstance();
                  instance.Init(dataFile.ScriptableDatas[i]);
                  m_SkillInstances.Add(id, instance);

                  LogUtil.Debug("ParseStory {0}", id);
                }
                else
                {
                  //repeated story config.
                }
              }
            }
          }
        }
        /*
        foreach (ScriptableData.ScriptableDataInfo info in dataFile.ScriptableDatas) {
          if (info.GetId() == "story" || info.GetId() == "script") {
            ScriptableData.FunctionData funcData = info.First;
            if (null != funcData) {
              ScriptableData.CallData callData = funcData.Call;
              if (null != callData && callData.HaveParam()) {
                int storyId = int.Parse(callData.GetParamId(0));
                int id = GenId(storyId, sceneId);
                if (!m_SkillInstances.ContainsKey(id)) {
                  SkillInstance instance = new SkillInstance();
                  instance.Init(info);
                  m_SkillInstances.Add(id, instance);

                  LogSystem.Debug("ParseStory {0}", id);
                } else {
                  //repeated story config.
                }
              }
            }
          }
        }*/
      }
    }
    private SkillInstance GetSkillInstanceResource(int id)
    {
      SkillInstance instance = null;
      lock (m_Lock) {
        m_SkillInstances.TryGetValue(id, out instance);
      }
      return instance;
    }

    private object m_Lock = new object();
    private Dictionary<int, SkillInstance> m_SkillInstances = new Dictionary<int, SkillInstance>();
    private static int GenId(int storyId, int sceneId)
    {
      return sceneId * 100 + storyId;
    }
  }
}
