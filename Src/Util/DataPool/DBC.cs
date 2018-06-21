using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Util
{

    /**
     * @brief 数据行
     */
    public class DBC_Row
    {

        /**
         * @brief 数据
         */
        private List<string> m_Data;

        /**
         * @brief 对数据文件的引用
         */
        private DBC m_DBC;

        /**
         * @brief 当前行所处行号
         */
        private int m_RowIndex;

        /**
         * @brief 构造函数
         *
         * @param dbc
         * @param rowIndex
         *
         * @return 
         */
        public DBC_Row(DBC dbc, int rowIndex = -1)
        {
            m_Data = new List<string>();
            m_DBC = dbc;
            m_RowIndex = -1;
        }

        /**
         * @brief 析构函数 
         */
        ~DBC_Row()
        {
            m_Data.Clear();
            m_Data = null;
            m_DBC = null;
        }

        /**
         * @brief 是否包含数据
         */
        public bool HasFields
        {
            get { return (m_Data != null && m_Data.Count > 0); }
        }

        /**
         * @brief 按标题列名字读取数据
         *
         * @param name
         *
         * @return 
         */
        public string SelectFieldByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (m_DBC == null || m_DBC.Header == null
                || m_DBC.Header.Count == 0)
                return null;

            int index = m_DBC.GetHeaderIndexByName(name);
            if (index >= 0 && index < m_Data.Count)
            {
                return m_Data[index];
            }

            return null;
        }

        /**
         * @brief 按列序号读取数据
         *
         * @param index
         *
         * @return 
         */
        public string SelectFieldByIndex(int index)
        {
            if (m_Data == null
                || m_Data.Count == 0)
                return null;

            if (index < 0 || index >= m_Data.Count)
            {
                return "";
            }

            return m_Data[index];
        }

        /**
         * @brief 按标题列名字前缀读取数据
         *
         * @param namePrefix
         *
         * @return 
         */
        public List<string> SelectFieldsByPrefix(string namePrefix)
        {
            List<string> list = new List<string>();

            if (string.IsNullOrEmpty(namePrefix))
                return list;

            if (m_DBC == null || m_DBC.Header == null
                || m_DBC.Header.Count == 0)
                return null;

            foreach (string name in m_DBC.Header)
            {
                if (!name.StartsWith(namePrefix))
                    continue;

                int index = m_DBC.GetHeaderIndexByName(name);
                if (index >= 0 && index < m_Data.Count)
                {
                    list.Add(m_Data[index]);
                }
            }

            return list;
        }

        /**
         * @brief 读取所有数据，并将列标题索引
         *
         * @return 
         */
        public Dictionary<string, string> SelectAllFields()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();

            if (m_DBC == null || m_DBC.Header == null
                || m_DBC.Header.Count == 0)
                return list;

            foreach (string name in m_DBC.Header)
            {
                int index = m_DBC.GetHeaderIndexByName(name);
                if (index >= 0 && index < m_Data.Count)
                {
                    list.Add(name, m_Data[index]);
                }
            }

            return list;
        }

        /**
         * @brief 返回行号
         */
        public int RowIndex
        {
            get { return m_RowIndex; }
        }

        /**
         * @brief 读取数据列表
         */
        public List<string> Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
    }

    /**
     * @brief 数据文件
     */
    public class DBC
    {

        /**
         * @brief 文件头，只用于读取二进制文件
         */
        [StructLayout(LayoutKind.Sequential)]
        struct FILE_HEAD
        {
            int m_Identify;               //标示	0XDDBBCC00
            int m_nFieldsNum;             //列数
            int m_nRecordsNum;          //行数
            int m_nStringBlockSize; //字符串区大小
        };

        /**
         * @brief 标题名列表
         */
        private List<string> m_Header;
        /**
         * @brief 类型列表
         */
        private List<string> m_Types;

        /**
         * @brief 数据池
         */
        private List<DBC_Row> m_DataBuf;

        /**
         * @brief 数据池，用于快速按关键字检索
         */
        private Dictionary<string, DBC_Row> m_HashData;

        /**
         * @brief 行数
         */
        private int m_RowNum;

        /**
         * @brief 列数
         */
        private int m_ColumNum;

        /**
         * @brief 文件名
         */
        private string m_FileName;

        /**
         * @brief 文件大小
         */
        private long m_FileSize;

        /**
         * @brief 构造函数
         *
         * @return 
         */
        public DBC()
        {
            m_Header = new List<string>();
            m_Types = new List<string>();
            m_DataBuf = new List<DBC_Row>();
            m_HashData = new Dictionary<string, DBC_Row>();
            m_FileName = "";
            m_FileSize = 0;
            m_RowNum = 0;
            m_ColumNum = 0;
        }

        /**
         * @brief 析构函数
         */
        ~DBC()
        {
            m_Header = null;
            m_Types = null;
            m_DataBuf.Clear();
            m_HashData.Clear();
            m_FileName = "";
        }

        /**
         * @brief 返回行数
         */
        public int RowNum
        {
            get { return m_RowNum; }
        }

        /**
         * @brief 返回列数
         */
        public int ColumnNum
        {
            get { return m_ColumNum; }
        }

        /**
         * @brief 返回标题名列表
         */
        public List<string> Header
        {
            get { return m_Header; }
        }
        /**
         * @brief 返回类型列表
         */
        public List<string> Types
        {
            get { return m_Types; }
        }

        /**
         * @brief 返回文件名
         */
        public string FileName
        {
            get { return m_FileName; }
        }

        /**
         * @brief 返回标题名，根据列序号
         *
         * @param index
         *
         * @return 
         */
        public string GetHeaderNameByIndex(int index)
        {
            if (m_Header == null || m_Header.Count == 0)
            {
                return "";
            }

            if (index < 0 || index >= m_Header.Count)
            {
                return "";
            }

            return m_Header[index];
        }

        /**
         * @brief 返回标题所在列序号，根据标题名字
         *
         * @param name
         *
         * @return 
         */
        public int GetHeaderIndexByName(string name)
        {
            int ret = -1;
            if (m_Header == null || m_Header.Count == 0)
            {
                return ret;
            }

            if (name == "")
            {
                return ret;
            }

            for (int index = 0; index < m_Header.Count; index++)
            {
                if (name == m_Header[index])
                {
                    ret = index;
                    break;
                }
            }

            return ret;
        }

        /**
         * @brief 返回指定行，根据行号
         *
         * @param index
         *
         * @return 
         */
        public DBC_Row GetRowByIndex(int index)
        {
            if (index < 0 || index >= m_RowNum)
                return null;

            if (m_DataBuf != null && index >= m_DataBuf.Count)
                return null;

            return m_DataBuf[index];
        }

        /**
         * @brief 返回文本，根据行、列号
         *
         * @param rowIndex
         * @param colIndex
         *
         * @return 
         */
        public string GetField(int rowIndex, int colIndex)
        {
            if (rowIndex < 0 || rowIndex >= m_RowNum
                || colIndex < 0 || colIndex >= m_ColumNum)
                return "";

            DBC_Row dbRow = GetRowByIndex(rowIndex);
            if (dbRow != null)
            {
                return dbRow.SelectFieldByIndex(colIndex);
            }

            return "";
        }

        /**
         * @brief 返回文本根据行关键字和列名
         *
         * @param id
         * @param headerName
         *
         * @return 
         */
        public string GetField(string id, string headerName)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(headerName))
                return "";
            DBC_Row row;
            if (m_HashData != null && m_HashData.TryGetValue(id, out row))
            {
                if (row != null)
                {
                    return row.SelectFieldByName(headerName);
                }
            }

            return "";
        }

        /**
         * @brief 载入文件
         *
         * @param path
         *
         * @return 
         */
        public bool Load(string path)
        {
            bool ret = false;

            if (path == "" || !FileReaderProxy.Exists(path))
            {
                return ret;
            }

            Stream ms = null;
            StreamReader sr = null;
            try
            {
                ms = FileReaderProxy.ReadFileAsMemoryStream(path);
                if (ms == null)
                {
                    LogUtil.Warn("DBC, Warning, Load file error or file is empty: {0}", path);
                    return false;
                }

                m_FileName = path;
                ms.Seek(0, SeekOrigin.Begin);
                m_FileSize = ms.Length;
                if (m_FileSize <= 0 || m_FileSize >= int.MaxValue)
                    return ret;

                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                sr = new StreamReader(ms, encoding);

                ret = LoadFromStream(sr);
                ret = true;
            }
            catch (Exception e)
            {
                LogUtil.Error("DBC.Load {0} Exception:{1}\n{2}", path, e.Message, e.StackTrace);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (ms != null)
                {
                    ms.Close();
                }
            }

            return ret;
        }

        //--------------------------------------------------------------
        // 私有方法

        /**
         * @brief 从文件流中读取
         *
         * @param sr
         *
         * @return 
         */
        public bool LoadFromStream(StreamReader sr)
        {
            return LoadFromStream_Text(sr);
        }

        /**
         * @brief 从文本文件流中读取
         *
         * @param sr
         *
         * @return 
         */
        private bool LoadFromStream_Text(StreamReader sr)
        {
            //--------------------------------------------------------------
            //临时变量
            List<string> vRet = null;
            string strLine = "";

            //读第一行,标题行
            strLine = sr.ReadLine();
            //读取失败，即认为读取结束
            if (strLine == null)
                return false;

            vRet = ConvertStringList(strLine, new string[] { "\t" });
            if (vRet == null || vRet.Count == 0)
                return false;
            m_Header = vRet;

            // 读第二行，类型行
            strLine = sr.ReadLine();
            // 读取失败，即认为读取结束
            if (strLine == null)
                return false;

            vRet = ConvertStringList(strLine, new string[] { "\t" });
            if (vRet == null || vRet.Count == 0)
                return false;
            m_Types = vRet;


            //--------------------------------------------------------------
            //初始化
            int nRecordsNum = 0;
            int nFieldsNum = vRet.Count;

            //--------------------------------------------------------------
            //开始读取
            DBC_Row dbcRow = null;
            do
            {
                vRet = null;
                dbcRow = null;

                //读取一行
                strLine = sr.ReadLine();
                //读取失败，即认为读取结束
                if (strLine == null) break;

                //是否是注释行
                if (strLine.StartsWith("#")) continue;

                //分解
                vRet = ConvertStringList(strLine, new string[] { "\t" });

                //列数不对
                if (vRet.Count == 0) continue;
                if (vRet.Count != nFieldsNum)
                {
                    //补上空格
                    if (vRet.Count < nFieldsNum)
                    {
                        int nSubNum = nFieldsNum - vRet.Count;
                        for (int i = 0; i < nSubNum; i++)
                        {
                            vRet.Add("");
                        }
                    }
                }

                //第一列不能为空
                if (string.IsNullOrEmpty(vRet[0])) continue;

                dbcRow = new DBC_Row(this, nRecordsNum);
                dbcRow.Data = vRet;

                m_DataBuf.Add(dbcRow);

                nRecordsNum++;
            } while (true);

            //--------------------------------------------------------
            //生成正式数据库
            m_ColumNum = nFieldsNum;
            m_RowNum = nRecordsNum;

            //--------------------------------------------------------
            //创建索引
            CreateIndex();

            return true;
        }

        /**
            * @brief 将字符串解析为字符串数组
            *
            * @param vec 字符串,类似于"100,200,200"
            *
            * @return 
            */
        private static List<string> ConvertStringList(string vec, string[] split)
        {
            string[] resut = vec.Split(split, StringSplitOptions.None);
            List<string> list = new List<string>();
            foreach (string str in resut)
            {
                list.Add(str);
            }

            return list;
        }

        /**
         * @brief 创建索引，默认第一列作为关键字
         *
         * @return 
         */
        private void CreateIndex()
        {
            foreach (DBC_Row row in m_DataBuf)
            {
                if (row.Data != null && row.Data.Count > 0)
                {
                    string key = row.Data[0];
                    if (!m_HashData.ContainsKey(key))
                    {
                        m_HashData.Add(key, row);
                    }
                    else
                    {
                        string err = string.Format("DBC.CreateIndex FileName:{0} SameKey:{1}", m_FileName, key);
                        System.Diagnostics.Debug.Assert(false, err);
                    }
                }
            }
        }
    }
}

