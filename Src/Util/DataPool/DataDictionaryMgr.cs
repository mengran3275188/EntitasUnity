using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public class DataDictionaryMgr2<TData> where TData : IData2, new()
    {
        //---------------------------------------------------------
        // 属性
        //---------------------------------------------------------
        Dictionary<int, object> m_DataContainer;

        public DataDictionaryMgr2()
        {
            m_DataContainer = new Dictionary<int, object>();
        }

        public bool CollectDataFromDBC(string file)
        {
            long t1 = TimeUtility.Instance.GetElapsedTimeUs();
            bool result = true;
            DBC document = new DBC();
            document.Load(HomePath.Instance.GetAbsolutePath(file));
            long t2 = TimeUtility.Instance.GetElapsedTimeUs();

            long t3 = TimeUtility.Instance.GetElapsedTimeUs();
            for (int index = 0; index < document.RowNum; index++)
            {
                try
                {
                    DBC_Row node = document.GetRowByIndex(index);
                    if (node != null)
                    {
                        TData data = new TData();
                        bool ret = data.CollectDataFromDBC(node);
                        if (ret && !m_DataContainer.ContainsKey(data.GetId()))
                        {
                            m_DataContainer.Add(data.GetId(), data);
                        }
                        else
                        {
                            string info = string.Format("DataTempalteMgr.CollectDataFromDBC collectData Row:{0} failed!", index);
                            LogUtil.Error(info);
                            LogUtil.CallStack();
                            result = false;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogUtil.Error("CollectData failed. file:{0} rowIndex:{1}\nException:{2}\n{3}", file, index, ex.Message, ex.StackTrace);
                }
            }
            long t4 = TimeUtility.Instance.GetElapsedTimeUs();
            //LogUtil.Info("text load {0} parse {1}, file {2}", t2 - t1, t4 - t3, file);
            return result;
        }
        public bool CollectDataFromBinary(string file)
        {
            long t1 = TimeUtility.Instance.GetElapsedTimeUs();
            bool result = true;
            BinaryTable table = new BinaryTable();
            table.Load(HomePath.Instance.GetAbsolutePath(file));
            long t2 = TimeUtility.Instance.GetElapsedTimeUs();

            long t3 = TimeUtility.Instance.GetElapsedTimeUs();
            for (int index = 0; index < table.Records.Count; ++index)
            {
                try
                {
                    TData data = new TData();
                    bool ret = data.CollectDataFromBinary(table, index);
                    if (ret && !m_DataContainer.ContainsKey(data.GetId()))
                    {
                        m_DataContainer.Add(data.GetId(), data);
                    }
                    else
                    {
                        string info = string.Format("DataTempalteMgr.CollectDataFromBinary collectData Row:{0} failed!", index);
                        LogUtil.Error(info);
                        LogUtil.CallStack();
                        result = false;
                    }
                }
                catch (System.Exception ex)
                {
                    LogUtil.Error("CollectData failed. file:{0} rowIndex:{1}\nException:{2}\n{3}", file, index, ex.Message, ex.StackTrace);
                }
            }
            long t4 = TimeUtility.Instance.GetElapsedTimeUs();
            LogUtil.Info("binary load {0} parse {1}, file {2}", t2 - t1, t4 - t3, file);
            return result;
        }
        public void SaveToBinary(string file)
        {
#if DEBUG
      BinaryTable table = new BinaryTable();
      foreach (KeyValuePair<int, object> pair in m_DataContainer) {
        TData data = (TData)pair.Value;
        data.AddToBinary(table);
      }
      table.Save(file);
#endif
        }
        public bool AddData(TData data)
        {
            try
            {
                if (!m_DataContainer.ContainsKey(data.GetId()))
                {
                    m_DataContainer.Add(data.GetId(), data);
                }
                else
                {
                    string info = string.Format("DataTempalteMgr.AddData failed!");
                    LogUtil.Error(info);
                    LogUtil.CallStack();
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                LogUtil.Error("AddData failed.\nException:{0}\n{1}", ex.Message, ex.StackTrace);
                return false;
            }
            return true;
        }

        public TData GetDataById(int id)
        {
            if (m_DataContainer.TryGetValue(id, out object ret))
                return (TData)ret;
            else
                return default(TData);
        }

        public int GetDataCount()
        {
            return m_DataContainer.Count;
        }

        public Dictionary<int, object> GetData()
        {
            return m_DataContainer;
        }

        public void Clear()
        {
            m_DataContainer.Clear();
        }
    }
}
