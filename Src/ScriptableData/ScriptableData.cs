using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScriptableData
{
    /// <summary>
    /// 基于函数样式的脚本化数据解析工具。可以用作DSL元语言。
    /// </summary>
    /// <remarks>
    /// 混淆的当前实现要求脚本里不能出现`字符。另外，测试代码的加解密表的设计要求脚本里不能出现以@开头的标识符。
    /// </remarks>
    public interface ISyntaxComponent
    {
        bool IsValid();
        string GetId();
        int GetIdType();
        int GetLine();
        string ToScriptString();
    }

    public abstract class AbstractSyntaxCompoent : ISyntaxComponent
    {
        public const int ID_TOKEN = 0;
        public const int NUM_TOKEN = 1;
        public const int STRING_TOKEN = 2;
        public const int BOOL_TOKEN = 3;

        public abstract bool IsValid();
        public abstract string GetId();
        public abstract int GetIdType();
        public abstract int GetLine();
        public abstract string ToScriptString();
    }
    /// <summary>
    /// 空语法单件
    /// </summary>
    public class NullSyntax : AbstractSyntaxCompoent
    {
        public override bool IsValid()
        {
            return false;
        }
        public override string GetId()
        {
            return "";
        }
        public override int GetIdType()
        {
            return ID_TOKEN;
        }
        public override int GetLine()
        {
            return -1;
        }
        public override string ToScriptString()
        {
            return ToString();
        }

        public static NullSyntax Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private readonly static NullSyntax s_Instance = new NullSyntax();
    }
    /// <summary>
    /// 用于描述变量、常量与无参命令语句。可能会出现在函数调用参数表与函数语句列表中。
    /// </summary>
    public class ValueData : AbstractSyntaxCompoent
    {
        public override bool IsValid()
        {
            return HaveId();
        }
        public override string GetId()
        {
            return m_Id;
        }
        public override int GetIdType()
        {
            return m_Type;
        }
        public override int GetLine()
        {
            return m_Line;
        }
        public override string ToScriptString()
        {
#if FULL_VERSION
            return Utility.quoteString(m_Id, m_Type);
#else
      return ToString();
#endif
        }

        public bool HaveId()
        {
            return !string.IsNullOrEmpty(m_Id) || m_Type == STRING_TOKEN;
        }
        public void SetId(string id)
        {
            m_Id = id;
        }
        public void SetType(int _type)
        {
            m_Type = _type;
        }
        public void SetLine(int line)
        {
            m_Line = line;
        }
        public bool IsId()
        {
            return ID_TOKEN == m_Type;
        }
        public bool IsNumber()
        {
            return NUM_TOKEN == m_Type;
        }
        public bool IsString()
        {
            return STRING_TOKEN == m_Type;
        }
        public bool IsBoolean()
        {
            return BOOL_TOKEN == m_Type;
        }
        public void Clear()
        {
            m_Type = ID_TOKEN;
            m_Id = "";
            m_Line = -1;
        }
        public void CopyFrom(ValueData other)
        {
            m_Type = other.m_Type;
            m_Id = other.m_Id;
            m_Line = other.m_Line;
        }

        public ValueData()
        { }
        public ValueData(string val)
        {
            if (val == "true" || val == "false")
                m_Type = BOOL_TOKEN;
            else if (Utility.needQuote(val))
                m_Type = STRING_TOKEN;
            else if (val.Length > 0 && (val[0] >= '0' && val[0] <= '9' || val[0] == '.' || val[0] == '-'))
                m_Type = NUM_TOKEN;
            else
                m_Type = ID_TOKEN;

            m_Id = val;
            m_Line = -1;
        }
        public ValueData(string val, int _type)
        {
            m_Type = _type;
            m_Id = val;
            m_Line = -1;
        }

        private int m_Type = ID_TOKEN;
        private string m_Id = "";
        private int m_Line = -1;
    }
    /// <summary>
    /// 函数调用数据，可能出现在函数头、参数表与函数语句列表中。
    /// </summary>
    public class CallData : AbstractSyntaxCompoent
    {
        public enum ParamClassEnum
        {
            PARAM_CLASS_MIN = 0,
            PARAM_CLASS_NOTHING = PARAM_CLASS_MIN,
            PARAM_CLASS_PARENTHESIS,
            PARAM_CLASS_BRACKET,
            PARAM_CLASS_PERIOD,
            PARAM_CLASS_PERIOD_PARENTHESIS,
            PARAM_CLASS_PERIOD_BRACKET,
            PARAM_CLASS_PERIOD_BRACE,
            PARAM_CLASS_OPERATOR,
            PARAM_CLASS_TERNARY_OPERATOR,
            PARAM_CLASS_MAX,
        }
        public override bool IsValid()
        {
            return HaveId() || HaveParam();
        }
        public override string GetId()
        {
            if (null != m_Name)
                return m_Name.GetId();
            else if (null != m_Call)
                return m_Call.GetId();
            else
                return "";
        }
        public override int GetIdType()
        {
            if (null != m_Name)
                return m_Name.GetIdType();
            else if (null != m_Call)
                return m_Call.GetIdType();
            else
                return ID_TOKEN;
        }
        public override int GetLine()
        {
            if (null != m_Name)
                return m_Name.GetLine();
            else if (null != m_Call)
                return m_Call.GetLine();
            else
                return -1;
        }
        public override string ToScriptString()
        {
#if FULL_VERSION
            return Utility.getCallString(this);
#else
      return ToString();
#endif
        }

        public List<ISyntaxComponent> Params
        {
            get
            {
                return m_Params;
            }
            set
            {
                m_Params = value;
                if (value.Count > 0)
                {
                    if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
                    {
                        m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
                    }
                }
            }
        }
        public bool IsHighOrder
        {
            get { return m_IsHighOrder; }
        }
        public ValueData Name
        {
            get { return m_Name; }
            set
            {
                m_Name = value;
                m_Call = null;
                m_IsHighOrder = false;
            }
        }
        public CallData Call
        {
            get { return m_Call; }
            set
            {
                m_Name = null;
                m_Call = value;
                m_IsHighOrder = true;
            }
        }
        public bool HaveId()
        {
            if (null != m_Name)
                return m_Name.HaveId();
            else if (null != m_Call)
                return m_Call.HaveId();
            else
                return false;
        }
        public void SetParamClass(int type)
        {
            if (type >= (int)ParamClassEnum.PARAM_CLASS_MIN && type < (int)ParamClassEnum.PARAM_CLASS_MAX)
            {
                m_ParamClass = type;
            }
        }
        public int GetParamClass()
        {
            return m_ParamClass;
        }
        public bool HaveParam()
        {
            return (int)ParamClassEnum.PARAM_CLASS_NOTHING != m_ParamClass;
        }
        public int GetParamNum()
        {
            return m_Params.Count;
        }
        public void SetParam(int index, ISyntaxComponent data)
        {
            if (index < 0 || index >= m_Params.Count)
                return;
            m_Params[index] = data;
        }
        public ISyntaxComponent GetParam(int index)
        {
            if (index < 0 || index >= (int)m_Params.Count)
                return null;
            return m_Params[index];
        }
        public string GetParamId(int index)
        {
            if (index < 0 || index >= (int)m_Params.Count)
                return "";
            return m_Params[index].GetId();
        }
        public void ClearParams()
        {
            m_Params.Clear();
        }
        public void AddParams(string id)
        {
            m_Params.Add(new ValueData(id));
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void AddParams(string id, int type)
        {
            m_Params.Add(new ValueData(id, type));
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void AddParams(ValueData param)
        {
            m_Params.Add(param);
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void AddParams(CallData param)
        {
            m_Params.Add(param);
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void AddParams(FunctionData param)
        {
            m_Params.Add(param);
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void AddParams(StatementData param)
        {
            m_Params.Add(param);
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void AddParams(ISyntaxComponent param)
        {
            m_Params.Add(param);
            if ((int)ParamClassEnum.PARAM_CLASS_NOTHING == m_ParamClass)
            {
                m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_PARENTHESIS;
            }
        }
        public void Clear()
        {
            m_Name = null;
            m_Call = null;
            m_IsHighOrder = false;
            m_Params = new List<ISyntaxComponent>();
            m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_NOTHING;
        }
        public void CopyFrom(CallData other)
        {
            m_IsHighOrder = other.m_IsHighOrder;
            m_Name = other.m_Name;
            m_Call = other.m_Call;
            m_Params = other.m_Params;
            m_ParamClass = other.m_ParamClass;
        }

        private bool m_IsHighOrder = false;
        private ValueData m_Name = null;
        private CallData m_Call = null;
        private List<ISyntaxComponent> m_Params = new List<ISyntaxComponent>();
        private int m_ParamClass = (int)ParamClassEnum.PARAM_CLASS_NOTHING;
    }
    /// <summary>
    /// 函数数据，由函数调用数据+语句列表构成。
    /// </summary>
    public class FunctionData : AbstractSyntaxCompoent
    {
        public enum ExtentClassEnum
        {
            EXTENT_CLASS_MIN = 0,
            EXTENT_CLASS_NOTHING = EXTENT_CLASS_MIN,
            EXTENT_CLASS_STATEMENT,
            EXTENT_CLASS_EXTERN_SCRIPT,
            EXTENT_CLASS_MAX,
        };
        public override bool IsValid()
        {
            return HaveId() || HaveParam() || HaveStatement() || HaveExternScript();
        }
        public override string GetId()
        {
            if (null != m_Call)
                return m_Call.GetId();
            else
                return "";
        }
        public override int GetIdType()
        {
            if (null != m_Call)
                return m_Call.GetIdType();
            else
                return ID_TOKEN;
        }
        public override int GetLine()
        {
            if (null != m_Call)
                return m_Call.GetLine();
            else
                return -1;
        }
        public override string ToScriptString()
        {
#if FULL_VERSION
            //与write方法不同，这里输出无缩进单行表示
            string line = "";
            if (null != m_Call)
                line = m_Call.ToScriptString();
            StringBuilder stream = new StringBuilder();
            stream.Append(line);
            if (HaveStatement())
            {
                stream.Append("{");
                int ct = GetStatementNum();
                for (int i = 0; i < ct; ++i)
                {
                    ISyntaxComponent data = GetStatement(i);
                    stream.Append(data.ToScriptString());
                    stream.Append(";");
                }
                stream.Append("}");
            }
            else if (HaveExternScript())
            {
                stream.Append("{:");
                stream.Append(GetExternScript());
                stream.Append(":}");
            }
            return stream.ToString();
#else
      return ToString();
#endif
        }

        public CallData Call
        {
            get { return m_Call; }
            set { m_Call = value; }
        }
        public List<ISyntaxComponent> Statements
        {
            get
            {
                return m_Statements;
            }
            set
            {
                m_Statements = value;
                if (value.Count > 0)
                {
                    if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
                    {
                        m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
                    }
                }
            }
        }
        public void SetExtentClass(int type)
        {
            if (type >= (int)ExtentClassEnum.EXTENT_CLASS_MIN && type < (int)ExtentClassEnum.EXTENT_CLASS_MAX)
            {
                m_ExtentClass = type;
            }
        }
        public int GetExtentClass()
        {
            return m_ExtentClass;
        }
        public bool HaveId()
        {
            if (null != m_Call)
                return m_Call.HaveId();
            else
                return false;
        }
        public bool HaveParam()
        {
            if (null != m_Call)
                return m_Call.HaveParam();
            else
                return false;
        }
        public bool HaveStatement()
        {
            return (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT == m_ExtentClass;
        }
        public bool HaveExternScript()
        {
            return (int)ExtentClassEnum.EXTENT_CLASS_EXTERN_SCRIPT == m_ExtentClass;
        }
        public void SetExternScript(string scp)
        {
            m_ExternScript = scp;
        }
        public string GetExternScript()
        {
            return m_ExternScript;
        }
        public int GetStatementNum()
        {
            return m_Statements.Count;
        }
        public void SetStatement(int index, ISyntaxComponent data)
        {
            if (index < 0 || index >= m_Statements.Count)
                return;
            m_Statements[index] = data;
        }
        public ISyntaxComponent GetStatement(int index)
        {
            if (index < 0 || index >= m_Statements.Count)
                return NullSyntax.Instance;
            return m_Statements[index];
        }
        public string GetStatementId(int index)
        {
            if (index < 0 || index >= m_Statements.Count)
                return "";
            return m_Statements[index].GetId();
        }
        public void ClearStatements()
        {
            m_Statements.Clear();
        }
        public void AddStatement(string id)
        {
            m_Statements.Add(new ValueData(id));
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void AddStatement(string id, int type)
        {
            m_Statements.Add(new ValueData(id, type));
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void AddStatement(ValueData statement)
        {
            m_Statements.Add(statement);
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void AddStatement(CallData statement)
        {
            m_Statements.Add(statement);
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void AddStatement(FunctionData statement)
        {
            m_Statements.Add(statement);
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void AddStatement(StatementData statement)
        {
            m_Statements.Add(statement);
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void AddStatement(ISyntaxComponent statement)
        {
            m_Statements.Add(statement);
            if ((int)ExtentClassEnum.EXTENT_CLASS_STATEMENT != m_ExtentClass)
            {
                m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_STATEMENT;
            }
        }
        public void Clear()
        {
            m_Call = null;
            m_Statements = new List<ISyntaxComponent>();
            m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_NOTHING;
            m_ExternScript = null;
        }
        public void CopyFrom(FunctionData other)
        {
            m_Call = other.m_Call;
            m_Statements = other.m_Statements;
            m_ExtentClass = other.m_ExtentClass;
            m_ExternScript = other.m_ExternScript;
        }

        private CallData m_Call = null;
        private List<ISyntaxComponent> m_Statements = new List<ISyntaxComponent>();
        private int m_ExtentClass = (int)ExtentClassEnum.EXTENT_CLASS_NOTHING;
        private string m_ExternScript = null;

        public static FunctionData NullFunctionData
        {
            get
            {
                return s_NullFunctionData;
            }
        }
        private readonly static FunctionData s_NullFunctionData = new FunctionData();
    }
    /// <summary>
    /// 语句数据，由多个函数数据连接而成。
    /// </summary>
    public class StatementData : AbstractSyntaxCompoent
    {
        public override bool IsValid()
        {
            bool ret = true;
            if (m_Functions.Count <= 0)
            {
                ret = false;
            }
            else
            {
                for (int i = 0; i < m_Functions.Count; ++i)
                {
                    ret = ret && m_Functions[i].IsValid();
                }
            }
            return ret;
        }
        public override string GetId()
        {
            if (m_Functions.Count <= 0)
                return "";
            else
                return m_Functions[0].GetId();
        }
        public override int GetIdType()
        {
            if (m_Functions.Count <= 0)
                return ID_TOKEN;
            else
                return m_Functions[0].GetIdType();
        }
        public override int GetLine()
        {
            if (m_Functions.Count <= 0)
                return -1;
            else
                return m_Functions[0].GetLine();
        }
        public override string ToScriptString()
        {
#if FULL_VERSION
            //与write方法不同，这里输出无缩进单行表示
            FunctionData tempData = First;
            CallData callData = null;
            callData = tempData.Call;
            if (null != callData && callData.GetParamClass() == (int)CallData.ParamClassEnum.PARAM_CLASS_TERNARY_OPERATOR)
            {
                if (callData.HaveId() && callData.HaveParam() && tempData.HaveStatement())
                {
                    string line = string.Format("{0} {1} {2}", callData.GetParam(0).ToScriptString(), callData.GetId(), tempData.GetStatement(0).ToScriptString());
                    if (Functions.Count == 2)
                    {
                        FunctionData funcData = Functions[1];
                        if (funcData.HaveId() && funcData.HaveStatement())
                            line = string.Format("{0} {1} {2}", line, funcData.GetId(), funcData.GetStatement(0).ToScriptString());
                    }
                    return line;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                StringBuilder stream = new StringBuilder();
                int ct = Functions.Count;
                for (int i = 0; i < ct; ++i)
                {
                    FunctionData funcData = Functions[i];
                    stream.Append(funcData.ToScriptString());
                }
                return stream.ToString();
            }
#else
      return ToString();
#endif
        }

        public List<FunctionData> Functions
        {
            get { return m_Functions; }
        }
        public FunctionData First
        {
            get
            {
                if (m_Functions.Count > 0)
                    return m_Functions[0];
                else
                    return FunctionData.NullFunctionData;
            }
        }
        public FunctionData Second
        {
            get
            {
                if (m_Functions.Count > 1)
                    return m_Functions[1];
                else
                    return FunctionData.NullFunctionData;
            }
        }
        public FunctionData Last
        {
            get
            {
                if (m_Functions.Count > 0)
                    return m_Functions[m_Functions.Count - 1];
                else
                    return FunctionData.NullFunctionData;
            }
        }
        public void Clear()
        {
            m_Functions = new List<FunctionData>();
        }
        public void CopyFrom(StatementData other)
        {
            m_Functions = other.m_Functions;
        }

        private List<FunctionData> m_Functions = new List<FunctionData>();

        public static StatementData NullStatementData
        {
            get
            {
                return s_NullStatementData;
            }
        }
        private readonly static StatementData s_NullStatementData = new StatementData();
    }

    public class ScriptableDataInfo : StatementData
    {
        public override string ToScriptString()
        {
            StringBuilder stream = new StringBuilder();
            Utility.writeStatementData(stream, this, 0);
            //stream.Append(base.ToScriptString());
            return stream.ToString();
        }

        public void SetLoaded(bool loaded)
        {
            mLoaded = loaded;
        }
        public bool IsLoaded()
        {
            return mLoaded;
        }
        public void SetResourceName(string name)
        {
            mResourceName = name;
        }
        public string GetResourceName()
        {
            return mResourceName;
        }
        public new void Clear()
        {
            base.Clear();
            mLoaded = false;
            mResourceName = "";
        }
        public void CopyFrom(ScriptableDataInfo other)
        {
            base.CopyFrom((StatementData)other);
            mLoaded = other.mLoaded;
            mResourceName = other.mResourceName;
        }

        private bool mLoaded = false;
        private string mResourceName = "";
    };
    public class ScriptableDataFile
    {
        public List<ScriptableDataInfo> ScriptableDatas
        {
            get { return mScriptableDatas; }
        }
        public void AddScriptableData(ScriptableDataInfo data)
        {
            mScriptableDatas.Add(data);
        }

        public bool Load(string file)
        {
#if FULL_VERSION
            string content = File.ReadAllText(file);
            //DashFire.LogSystem.Debug("ScriptableDataFile.Load {0}:\n{1}", file, content);
            return LoadFromString(content, file);
#else
      return false;
#endif
        }
        public bool LoadFromString(string content, string resourceName)
        {
#if FULL_VERSION
            Parser.DslToken tokens = new Parser.DslToken(content);
            Parser.DslError error = new Parser.DslError(tokens);
            Parser.RuntimeAction action = new Parser.RuntimeAction(mScriptableDatas);
            action.onGetLastToken = () => { return tokens.getLastToken(); };
            action.onGetLastLineNumber = () => { return tokens.getLastLineNumber(); };
            Parser.DslParser.parse(action, tokens, error, 0);
            if (error.HasError)
            {
                for (int i = 0; i < mScriptableDatas.Count; i++)
                {
                    mScriptableDatas[i].Clear();
                }
                /*
                      foreach(ScriptableDataInfo data in mScriptableDatas) {
                          data.Clear();
                      }*/
            }
            else
            {
                for (int i = 0; i < mScriptableDatas.Count; i++)
                {
                    mScriptableDatas[i].SetResourceName(resourceName);
                }
                /*
                foreach (ScriptableDataInfo data in mScriptableDatas) {
                          data.SetResourceName(resourceName);
                      }*/
            }
            return !error.HasError;
#else
      return false;
#endif
        }
        public void Save(string file)
        {
#if FULL_VERSION
            using (StreamWriter sw = new StreamWriter(file))
            {
                if (null != sw)
                {
                    for (int i = 0; i < mScriptableDatas.Count; i++)
                    {
                        sw.Write(mScriptableDatas[i].ToScriptString());
                        sw.Write("\r\n");
                    }
                    /*
                    foreach (ScriptableDataInfo data in mScriptableDatas) {
                      sw.Write(data.ToScriptString());
                      sw.Write("\r\n");
                    }*/
                    sw.Close();
                }
            }
#endif
        }

        public string GenerateObfuscatedCode(string content, Dictionary<string, string> encodeTable)
        {
#if FULL_VERSION
            Parser.DslToken tokens = new Parser.DslToken(content);
            Parser.DslError error = new Parser.DslError(tokens);
            Parser.ObfuscationAction action = new Parser.ObfuscationAction();
            action.onGetLastToken = () => { return tokens.getLastToken(); };
            action.onEncodeString = s => Encode(s, encodeTable);
            Parser.DslParser.parse(action, tokens, error, 0);
            if (error.HasError)
            {
                return "";
            }
            else
            {
                return action.getObfuscatedCode();
            }
#else
      return "";
#endif
        }
        public void LoadObfuscatedFile(string file, Dictionary<string, string> decodeTable)
        {
            string code = File.ReadAllText(file);
            LoadObfuscatedCode(code, decodeTable);
        }
        public void LoadObfuscatedCode(string code, Dictionary<string, string> decodeTable)
        {
            Parser.RuntimeAction action = new Parser.RuntimeAction(mScriptableDatas);
            Parser.ObfuscationAction.parse(code, action, s => Decode(s, decodeTable));
        }

        private string Encode(string s0, Dictionary<string, string> encodeTable)
        {
#if FULL_VERSION
            string s;
            if (!encodeTable.TryGetValue(s0, out s))
                s = s0;
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            s = Convert.ToBase64String(bytes);
            int len = s.Length;
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < len; ++i)
            {
                char c = (char)((char)158 - s[i]);
                if (c != '`')
                    str.Append(c);
                else
                    str.Append(s[i]);
            }
            return str.ToString();
#else
      return "";
#endif
        }
        private string Decode(string s, Dictionary<string, string> decodeTable)
        {
            int len = s.Length;
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < len; ++i)
            {
                char c = (char)((char)158 - s[i]);
                if (c != '`')
                    str.Append(c);
                else
                    str.Append(s[i]);
            }
            s = str.ToString();
            byte[] bytes = Convert.FromBase64String(s);
            s = Encoding.UTF8.GetString(bytes);

            if (!decodeTable.TryGetValue(s, out string s0))
                s0 = s;
            return s0;
        }

        private List<ScriptableDataInfo> mScriptableDatas = new List<ScriptableDataInfo>();
    };

    public sealed class Utility
    {
        public static bool needQuote(string str)
        {
            const string escapeChars = " \t\r\n{}()[],;~`!%^&*-+=|:<>?/#\\";
            if (str.Length == 0)
            {
                return true;
            }
            bool haveDot = false;
            bool notNum = false;
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str[i];
                if (escapeChars.IndexOf(c) >= 0)
                {
                    return true;
                }
                if (c == '.')
                {
                    haveDot = true;
                }
                else if (!notNum && !char.IsDigit(c))
                {
                    notNum = true;
                }
                if (haveDot && notNum)
                {
                    return true;
                }
            }
            return false;
        }

        public static string quoteString(string str, int _Type)
        {
            switch (_Type)
            {
                case AbstractSyntaxCompoent.STRING_TOKEN:
                    {
                        return "\"" + str + "\"";
                    }
                case AbstractSyntaxCompoent.NUM_TOKEN:
                case AbstractSyntaxCompoent.BOOL_TOKEN:
                    return str;
            }
            if (needQuote(str))
            {
                return "\"" + str + "\"";
            }
            return str;
        }

        public static string unquoteString(string str)
        {
            int len = str.Length;
            if (len > 2 && (str[0] == '\"' && str[len - 1] == '\"' || str[0] == '\'' && str[len - 1] == '\'') && !needQuote(str))
            {
                return str.Substring(1, len - 2);
            }
            return str;
        }

        public static string getCallString(CallData data)
        {
#if FULL_VERSION
            string lineNo = "";//formatString("/* %d */",data.GetLine());
            string line = "";
            if (data.IsHighOrder)
            {
                line = getCallString(data.Call);
            }
            else if (data.HaveId())
            {
                line = quoteString(data.GetId(), data.GetIdType());
            }
            if (data.HaveParam())
            {
                int paramClass = data.GetParamClass();
                if ((int)CallData.ParamClassEnum.PARAM_CLASS_OPERATOR == paramClass)
                {
                    switch (data.GetParamNum())
                    {
                        case 1:
                            return string.Format("{0} {1}", line, data.GetParam(0).ToScriptString());
                        case 2:
                            return string.Format("{0} {1} {2}", data.GetParam(0).ToScriptString(), line, data.GetParam(1).ToScriptString());
                        default:
                            return line;
                    }
                }
                else
                {
                    string lbracket = "";
                    string rbracket = "";
                    switch (paramClass)
                    {
                        case (int)CallData.ParamClassEnum.PARAM_CLASS_PARENTHESIS:
                            lbracket = "(";
                            rbracket = ")";
                            break;
                        case (int)CallData.ParamClassEnum.PARAM_CLASS_BRACKET:
                            lbracket = "[";
                            rbracket = "]";
                            break;
                        case (int)CallData.ParamClassEnum.PARAM_CLASS_PERIOD:
                            lbracket = ".";
                            rbracket = "";
                            break;
                        case (int)CallData.ParamClassEnum.PARAM_CLASS_PERIOD_PARENTHESIS:
                            lbracket = ".(";
                            rbracket = ")";
                            break;
                        case (int)CallData.ParamClassEnum.PARAM_CLASS_PERIOD_BRACKET:
                            lbracket = ".[";
                            rbracket = "]";
                            break;
                        case (int)CallData.ParamClassEnum.PARAM_CLASS_PERIOD_BRACE:
                            lbracket = ".{";
                            rbracket = "}";
                            break;
                    }
                    StringBuilder stream = new StringBuilder();
                    stream.Append(lbracket);
                    int ct = data.GetParamNum();
                    for (int i = 0; i < ct; ++i)
                    {
                        if (i > 0)
                            stream.Append(",");
                        ISyntaxComponent param = data.GetParam(i);
                        if ((int)CallData.ParamClassEnum.PARAM_CLASS_PERIOD == paramClass)
                            stream.Append(unquoteString(param.ToScriptString()));
                        else
                            stream.Append(param.ToScriptString());
                    }
                    stream.Append(rbracket);
                    return string.Format("{0}{1}{2}", lineNo, line, stream.ToString());
                }
            }
            else
            {
                return string.Format("{0}{1}", lineNo, line);
            }
#else
      return "";
#endif
        }

        public static void writeContent(StringBuilder stream, string line, int indent)
        {
#if FULL_VERSION
            for (int i = 0; i < indent; ++i)
            {
                stream.Append('\t');
            }
            stream.Append(line);
#endif
        }

        public static void writeLine(StringBuilder stream, string line, int indent)
        {
#if FULL_VERSION
            writeContent(stream, line, indent);
            stream.Append("\r\n");
#endif
        }

        public static void writeSyntaxComponent(StringBuilder stream, ISyntaxComponent data, int indent, bool isLastOfStatement)
        {
#if FULL_VERSION
            ValueData val = data as ValueData;
            if (null != val)
            {
                if (isLastOfStatement)
                    writeLine(stream, val.ToScriptString() + ";", indent);
                else
                    writeLine(stream, val.ToScriptString(), indent);
            }
            else
            {
                CallData call = data as CallData;
                if (null != call)
                {
                    if (isLastOfStatement)
                        writeLine(stream, call.ToScriptString() + ";", indent);
                    else
                        writeLine(stream, call.ToScriptString(), indent);
                }
                else
                {
                    FunctionData function = data as FunctionData;
                    if (null != function)
                    {
                        writeFunctionData(stream, function, indent, isLastOfStatement);
                    }
                    else
                    {
                        StatementData statement = data as StatementData;
                        writeStatementData(stream, statement, indent);
                    }
                }
            }
#endif
        }

        public static void writeFunctionData(StringBuilder stream, FunctionData data, int indent, bool isLastOfStatement)
        {
#if FULL_VERSION
            string line = "";
            if (null != data.Call)
                line = data.Call.ToScriptString();
            if (data.HaveStatement())
            {
                if (line.Length > 0)
                {
                    writeLine(stream, line, indent);
                }
                writeLine(stream, "{", indent);
                ++indent;

                int ct = data.GetStatementNum();
                for (int i = 0; i < ct; ++i)
                {
                    ISyntaxComponent tempData = data.GetStatement(i);
                    writeSyntaxComponent(stream, tempData, indent, true);
                }

                --indent;
                if (isLastOfStatement)
                    writeLine(stream, "};", indent);
                else
                    writeLine(stream, "}", indent);
            }
            else if (data.HaveExternScript())
            {
                if (line.Length > 0)
                {
                    writeLine(stream, line, indent);
                }
                writeContent(stream, "{:", indent);
                stream.Append(data.GetExternScript());
                if (isLastOfStatement)
                    stream.AppendLine(":};");
                else
                    stream.AppendLine(":}");
            }
            else
            {
                if (isLastOfStatement)
                    writeLine(stream, line + ";", indent);
                else
                    writeLine(stream, line, indent);
            }
#endif
        }

        public static void writeStatementData(StringBuilder stream, StatementData data, int indent)
        {
#if FULL_VERSION
            FunctionData tempData = data.First;
            CallData callData = tempData.Call;
            if (null != callData && callData.GetParamClass() == (int)CallData.ParamClassEnum.PARAM_CLASS_TERNARY_OPERATOR)
            {
                if (callData.HaveId() && callData.HaveParam() && tempData.HaveStatement())
                {
                    string line = string.Format("{0} {1} {2}", callData.GetParam(0).ToScriptString(), callData.GetId(), tempData.GetStatement(0).ToScriptString());
                    if (data.Functions.Count == 2)
                    {
                        FunctionData funcData = data.Functions[1];
                        if (funcData.HaveId() && funcData.HaveStatement())
                            line = string.Format("{0} {1} {2}", line, funcData.GetId(), funcData.GetStatement(0).ToScriptString());
                    }
                    writeLine(stream, line + ";", indent);
                }
            }
            else
            {
                int ct = data.Functions.Count;
                bool isLastOfStatement = false;
                if (ct == 0)
                    isLastOfStatement = true;
                for (int i = 0; i < ct; ++i)
                {
                    if (i == ct - 1)
                        isLastOfStatement = true;
                    FunctionData func = data.Functions[i];
                    writeFunctionData(stream, func, indent, isLastOfStatement);
                }
            }
#endif
        }
    }
}
