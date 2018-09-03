using System;
using System.Collections.Generic;
using Util;

namespace ScriptableData
{
    public sealed class ConfigManager : Singleton<ConfigManager>
    {
        public void LoadIfNotExist(int id, int type, params string[] files)
        {
            if (!Exist(id, type))
            {
                for (int i = 0; i < files.Length; i++)
                {
                    Load(files[i], type);
                }
                /*
                foreach (string file in files) {
                  LoadStory(file, sceneId);
                }*/
            }
        }
        public bool Exist(int id, int type)
        {
            int finalId = GenId(id, type);
            return null != GetInstanceResource(finalId);
        }
        public void Load(string file, int type)
        {
            if (!string.IsNullOrEmpty(file))
            {
                ScriptableData.ScriptableDataFile dataFile = new ScriptableData.ScriptableDataFile();
                if (dataFile.Load(file))
                {
                    Load(dataFile, type);
                }
                //#if DEBUG
                //#else
                //        dataFile.LoadObfuscatedFile(file, GlobalVariables.Instance.DecodeTable);
                //        Load(dataFile, sceneId);
                //#endif
            }
        }
        public void LoadText(string text, int sceneId)
        {
            ScriptableData.ScriptableDataFile dataFile = new ScriptableData.ScriptableDataFile();
            if (dataFile.LoadFromString(text, "story"))
            {
                Load(dataFile, sceneId);
            }
            //#if DEBUG
            //#else
            //      dataFile.LoadObfuscatedCode(text, GlobalVariables.Instance.DecodeTable);
            //      Load(dataFile, sceneId);
            //#endif
        }
        public IInstance NewInstance(int id, int type)
        {
            IInstance instance = null;
            int finalId = GenId(id, type);
            IInstance temp = GetInstanceResource(finalId);
            if (null != temp)
            {
                instance = temp.Clone();
            }
            return instance;
        }
        public void Clear()
        {
            lock (m_Lock)
            {
                m_Instances.Clear();
            }
        }
        public void AddInstanceResource(int id, IInstance instance)
        {
            lock(m_Lock)
            {
                if(!m_Instances.ContainsKey(id))
                {
                    m_Instances.Add(id, instance);
                }
            }
        }

        private void Load(ScriptableData.ScriptableDataFile dataFile, int type)
        {
            lock (m_Lock)
            {
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
                                int id = GenId(storyId, type);
                                if (!m_Instances.ContainsKey(id))
                                {
                                    Instance instance = new Instance();
                                    instance.Init(dataFile.ScriptableDatas[i]);
                                    m_Instances.Add(id, instance);

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
            }
        }
        private IInstance GetInstanceResource(int id)
        {
            IInstance instance = null;
            lock (m_Lock)
            {
                m_Instances.TryGetValue(id, out instance);
            }
            return instance;
        }

        private object m_Lock = new object();
        private Dictionary<int, IInstance> m_Instances = new Dictionary<int, IInstance>();
        private static int GenId(int id, int type)
        {
            return id * 100 + type;
        }
    }
}
