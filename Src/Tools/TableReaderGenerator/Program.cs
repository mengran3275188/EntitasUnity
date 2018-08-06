using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ScriptableData;
using Util;

namespace TableReaderGenerator
{
    internal enum TableFileTypeEnum
    {
        CLIENT = 0,
        PUBLIC,
        SERVER,
        MULTIFILE,
    }
    internal class TypeDef
    {
        internal string m_TypeName;
        internal string m_TypeCode;
        internal string m_TextCode;
        internal string m_BinaryCode;
        internal string m_CloneCode;
        internal string m_RecordCode = "DBCUtil.SetValue";

        internal TypeDef() { }
        internal TypeDef(string typeName, string typeCode, string textCode, string binaryCode, string cloneCode)
        {
            m_TypeName = typeName;
            m_TypeCode = typeCode;
            m_TextCode = textCode;
            m_BinaryCode = binaryCode;
            m_CloneCode = cloneCode;
        }
        internal TypeDef(string typeName, string typeCode, string textCode, string binaryCode, string cloneCode, string recordcode)
        {
            m_TypeName = typeName;
            m_TypeCode = typeCode;
            m_TextCode = textCode;
            m_BinaryCode = binaryCode;
            m_CloneCode = cloneCode;
            m_RecordCode = recordcode;
        }
    }
    internal class FieldDef
    {
        internal string m_MemberName;
        internal string m_FieldName;
        internal string m_Type;
        internal string m_Default;
        internal bool m_IsString;
        internal string m_Access;
        internal bool m_IsCustom;
        internal string m_TextCode;
        internal string m_BinaryCode;
    }
    internal class TableDef
    {
        internal string m_TableName;
        internal string m_Type;
        internal string m_RecordCode;
        internal string m_ProviderCode;
        internal List<FieldDef> m_Fields = new List<FieldDef>();
        internal TableFileTypeEnum m_FileType;
        internal string m_RecordName;
        internal string m_ProviderName;
        internal bool m_HookBeforeCollectData;
        internal bool m_HookAfterCollectData;
        internal bool m_HookBeforeLoad;
        internal bool m_HookAfterLoad;
        internal bool m_OverrideGetId;
        internal bool m_RecordGenClone;
        internal string m_RecordModifier = "";
        internal string m_ProviderModifier = "";
        internal string m_CsFileName = "DataReader";

        internal int GetLayoutFieldCount()
        {
            int ct = 0;
            for (int i = 0; i < m_Fields.Count; ++i)
            {
                if (!m_Fields[i].m_IsCustom)
                {
                    ++ct;
                }
            }
            return ct;
        }
    }
    internal class TableDslParser
    {
        internal bool Init(string dslFile, string basePath)
        {
            try
            {
                m_BasePath = basePath;
                ScriptableDataFile file = new ScriptableDataFile();
                if (!file.Load(dslFile))
                {
                    return false;
                }
                bool haveError = false;
                foreach (ScriptableDataInfo info in file.ScriptableDatas)
                {
                    if (info.GetId() == "tool")
                    {
                        FunctionData funcData = info.First;
                        if (null != funcData && funcData.HaveExternScript())
                        {
                            string toolFile;
                            CallData callData = funcData.Call;
                            if (null != callData && callData.GetParamNum() == 1)
                            {
                                toolFile = callData.GetParamId(0);
                            }
                            else
                            {
                                toolFile = m_DefToolCsFileName;
                            }
                            List<string> codes;
                            if (!m_ToolCodes.TryGetValue(toolFile, out codes))
                            {
                                codes = new List<string>();
                                m_ToolCodes.Add(toolFile, codes);
                            }
                            codes.Add(funcData.GetExternScript());
                        }
                    }
                    else if (info.GetId() == "global")
                    {
                        FunctionData funcData = info.First;
                        if (null != funcData && funcData.HaveExternScript())
                        {
                            string globalFile;
                            CallData callData = funcData.Call;
                            if (null != callData && callData.GetParamNum() == 1)
                            {
                                globalFile = callData.GetParamId(0);
                            }
                            else
                            {
                                globalFile = m_DefGlobalCsFileName;
                            }
                            List<string> codes;
                            if (!m_GlobalCodes.TryGetValue(globalFile, out codes))
                            {
                                codes = new List<string>();
                                m_GlobalCodes.Add(globalFile, codes);
                            }
                            codes.Add(funcData.GetExternScript());
                        }
                    }
                    else if (info.GetId() == "typedef")
                    {
                        FunctionData funcData = info.First;
                        if (null != funcData)
                        {
                            CallData callData = funcData.Call;
                            if (null != callData && callData.GetParamNum() == 1)
                            {
                                string typeName = callData.GetParamId(0);

                                TypeDef typeDef = new TypeDef();
                                typeDef.m_TypeName = typeName;
                                if (m_Types.ContainsKey(typeName))
                                {
                                    m_Types[typeName] = typeDef;
                                }
                                else
                                {
                                    m_Types.Add(typeName, typeDef);
                                }

                                foreach (ISyntaxComponent comp in funcData.Statements)
                                {
                                    FunctionData item = comp as FunctionData;
                                    if (null != item)
                                    {
                                        if (item.HaveExternScript())
                                        {
                                            if (item.GetId() == "type")
                                            {
                                                typeDef.m_TypeCode = item.GetExternScript();
                                            }
                                            else if (item.GetId() == "text")
                                            {
                                                typeDef.m_TextCode = item.GetExternScript();
                                            }
                                            else if (item.GetId() == "binary")
                                            {
                                                typeDef.m_BinaryCode = item.GetExternScript();
                                            }
                                            else if (item.GetId() == "record")
                                            {
                                                typeDef.m_RecordCode = item.GetExternScript();
                                            }
                                        }
                                        else
                                        {
                                            LogUtil.Error("typedef {0} must contains code ! line {1}", info.ToScriptString(), info.GetLine());
                                            haveError = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                LogUtil.Error("typedef {0} must have 1 params ! line {1}", info.ToScriptString(), info.GetLine());
                                haveError = true;
                            }
                        }
                    }
                    else if (info.GetId() == "tabledef")
                    {
                        FunctionData funcData = info.First;
                        if (null != funcData)
                        {
                            CallData callData = funcData.Call;
                            if (null != callData && callData.GetParamNum() == 3)
                            {
                                TableFileTypeEnum fileType = TableFileTypeEnum.PUBLIC;
                                string tableName, tableType, recordName, providerName;
                                tableName = callData.GetParamId(0);
                                tableType = callData.GetParamId(1);
                                recordName = tableName;
                                providerName = tableName + "Provider";

                                string fileTypeStr = callData.GetParamId(2);
                                if (0 == fileTypeStr.CompareTo("client"))
                                {
                                    fileType = TableFileTypeEnum.CLIENT;
                                }
                                else if (0 == fileTypeStr.CompareTo("server"))
                                {
                                    fileType = TableFileTypeEnum.SERVER;
                                }
                                else if (0 == fileTypeStr.CompareTo("multifile"))
                                {
                                    fileType = TableFileTypeEnum.MULTIFILE;
                                }
                                else
                                {
                                    fileType = TableFileTypeEnum.PUBLIC;
                                }

                                TableDef tableDef = new TableDef();
                                tableDef.m_TableName = tableName;
                                tableDef.m_Type = tableType;
                                tableDef.m_FileType = fileType;
                                tableDef.m_RecordName = recordName;
                                tableDef.m_ProviderName = providerName;
                                if (m_Tables.ContainsKey(tableName))
                                {
                                    m_Tables[tableName] = tableDef;
                                }
                                else
                                {
                                    m_Tables.Add(tableName, tableDef);
                                }

                                foreach (ISyntaxComponent comp in funcData.Statements)
                                {
                                    CallData field = comp as CallData;
                                    if (null != field)
                                    {
                                        if (field.GetId() == "recordname")
                                        {
                                            tableDef.m_RecordName = field.GetParamId(0);
                                            tableDef.m_ProviderName = tableDef.m_RecordName + "Provider";
                                        }
                                        else if (field.GetId() == "providername")
                                        {
                                            tableDef.m_ProviderName = field.GetParamId(0);
                                        }
                                        else if (field.GetId() == "filename")
                                        {
                                            tableDef.m_CsFileName = field.GetParamId(0);
                                        }
                                        else if (field.GetId() == "recordhook")
                                        {
                                            tableDef.m_HookBeforeCollectData = field.GetParamId(0) == "BeforeCollectData";
                                            tableDef.m_HookAfterCollectData = field.GetParamId(0) == "AfterCollectData";
                                        }
                                        else if (field.GetId() == "recordgenclone")
                                        {
                                            tableDef.m_RecordGenClone = field.GetParamId(0) == "Clone";
                                        }
                                        else if (field.GetId() == "providerhook")
                                        {
                                            tableDef.m_HookBeforeLoad = field.GetParamId(0) == "BeforeLoad";
                                            tableDef.m_HookAfterLoad = field.GetParamId(0) == "AfterLoad";
                                        }
                                        else if (field.GetId() == "override")
                                        {
                                            tableDef.m_OverrideGetId = field.GetParamId(0) == "GetId";
                                        }
                                        else if (field.GetId() == "recordmodifier")
                                        {
                                            tableDef.m_RecordModifier = field.GetParamId(0);
                                            if (tableDef.m_RecordModifier.Length > 0 && tableDef.m_RecordModifier[0] != ' ')
                                            {
                                                tableDef.m_RecordModifier = " " + tableDef.m_RecordModifier;
                                            }
                                        }
                                        else if (field.GetId() == "providermodifier")
                                        {
                                            tableDef.m_ProviderModifier = field.GetParamId(0);
                                            if (tableDef.m_ProviderModifier.Length > 0 && tableDef.m_ProviderModifier[0] != ' ')
                                            {
                                                tableDef.m_ProviderModifier = " " + tableDef.m_ProviderModifier;
                                            }
                                        }
                                        else if (field.GetId() == "fielddef" && field.GetParamNum() >= 3)
                                        {
                                            FieldDef fieldDef = new FieldDef();
                                            fieldDef.m_IsCustom = false;
                                            fieldDef.m_MemberName = field.GetParamId(0);
                                            fieldDef.m_FieldName = field.GetParamId(1);
                                            fieldDef.m_Type = field.GetParamId(2);
                                            if (field.GetParamNum() >= 4)
                                            {
                                                ISyntaxComponent param = field.GetParam(3);
                                                if (null != param)
                                                {
                                                    if (0 == fieldDef.m_Type.CompareTo("string"))
                                                    {
                                                        fieldDef.m_Default = param.GetId();
                                                        fieldDef.m_IsString = true;
                                                    }
                                                    else
                                                    {
                                                        fieldDef.m_Default = param.GetId();
                                                        fieldDef.m_IsString = false;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (0 == fieldDef.m_Type.CompareTo("string"))
                                                {
                                                    fieldDef.m_Default = "";
                                                    fieldDef.m_IsString = true;
                                                }
                                                else if (fieldDef.m_Type.Contains("_list") || fieldDef.m_Type.Contains("_array"))
                                                {
                                                    fieldDef.m_Default = "null";
                                                    fieldDef.m_IsString = false;
                                                }
                                                else if (0 == fieldDef.m_Type.CompareTo("bool"))
                                                {
                                                    fieldDef.m_Default = "false";
                                                    fieldDef.m_IsString = false;
                                                }
                                                else
                                                {
                                                    fieldDef.m_Default = "0";
                                                    fieldDef.m_IsString = false;
                                                }
                                            }
                                            if (field.GetParamNum() >= 5)
                                            {
                                                fieldDef.m_Access = field.GetParamId(4);
                                            }
                                            else
                                            {
                                                fieldDef.m_Access = "public";
                                            }
                                            tableDef.m_Fields.Add(fieldDef);
                                        }
                                        else
                                        {
                                            LogUtil.Error("field {0} must have name (member and field) and type ! line {1}", comp.ToScriptString(), comp.GetLine());
                                            haveError = true;
                                        }
                                    }
                                    else
                                    {
                                        FunctionData customCode = comp as FunctionData;
                                        if (null != customCode)
                                        {
                                            if (customCode.GetId() == "fielddef")
                                            {
                                                CallData customField = customCode.Call;
                                                if (null != customField)
                                                {
                                                    FieldDef fieldDef = new FieldDef();
                                                    fieldDef.m_IsCustom = true;
                                                    fieldDef.m_MemberName = customField.GetParamId(0);
                                                    fieldDef.m_Type = customField.GetParamId(1);
                                                    tableDef.m_Fields.Add(fieldDef);

                                                    foreach (ISyntaxComponent comp2 in customCode.Statements)
                                                    {
                                                        FunctionData item = comp2 as FunctionData;
                                                        if (null != item)
                                                        {
                                                            if (item.HaveExternScript())
                                                            {
                                                                if (item.GetId() == "text")
                                                                {
                                                                    fieldDef.m_TextCode = item.GetExternScript();
                                                                }
                                                                else if (item.GetId() == "binary")
                                                                {
                                                                    fieldDef.m_BinaryCode = item.GetExternScript();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                LogUtil.Error("custom fielddef {0} must contains code ! line {1}", customCode.ToScriptString(), customCode.GetLine());
                                                                haveError = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (customCode.GetId() == "providercode")
                                            {
                                                if (customCode.HaveExternScript())
                                                {
                                                    if (string.IsNullOrEmpty(tableDef.m_RecordCode))
                                                    {
                                                        tableDef.m_ProviderCode = customCode.GetExternScript();
                                                    }
                                                    else
                                                    {
                                                        LogUtil.Error("one provider class can have only one providercoe, providercode {0} ! line {1}", comp.ToScriptString(), comp.GetLine());
                                                        haveError = true;
                                                    }
                                                }
                                                else
                                                {
                                                    LogUtil.Error("providercode {0} must contains code ! line {1}", comp.ToScriptString(), comp.GetLine());
                                                    haveError = true;
                                                }
                                            }
                                            else if (customCode.GetId() == "recordcode")
                                            {
                                                if (customCode.HaveExternScript())
                                                {
                                                    if (string.IsNullOrEmpty(tableDef.m_RecordCode))
                                                    {
                                                        tableDef.m_RecordCode = customCode.GetExternScript();
                                                    }
                                                    else
                                                    {
                                                        LogUtil.Error("one record class can have only one recordcoe, recordcode {0} ! line {1}", comp.ToScriptString(), comp.GetLine());
                                                        haveError = true;
                                                    }
                                                }
                                                else
                                                {
                                                    LogUtil.Error("recordcode {0} must contains code ! line {1}", comp.ToScriptString(), comp.GetLine());
                                                    haveError = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                LogUtil.Error("tabledef {0} must have 3 params ! line {1}", info.ToScriptString(), info.GetLine());
                                haveError = true;
                            }
                        }
                    }
                    else
                    {
                        LogUtil.Error("Unknown part {0}, line {1}", info.GetId(), info.GetLine());
                        haveError = true;
                    }
                }
                m_Types.Add("string", new TypeDef("string", "string", "DBCUtil.ExtractString", "DBCUtil.ExtractString", "DBCUtil.CloneValue"));
                m_Types.Add("bool", new TypeDef("bool", "bool", "DBCUtil.ExtractBool", "DBCUtil.ExtractBool", "DBCUtil.CloneValue"));
                m_Types.Add("int", new TypeDef("int", "int", "DBCUtil.ExtractNumeric<int>", "DBCUtil.ExtractInt", "DBCUtil.CloneValue"));
                m_Types.Add("float", new TypeDef("float", "float", "DBCUtil.ExtractNumeric<float>", "DBCUtil.ExtractFloat", "DBCUtil.CloneValue"));
                m_Types.Add("long", new TypeDef("long", "long", "DBCUtil.ExtractNumeric<long>", "DBCUtil.ExtractLong", "DBCUtil.CloneValue"));
                m_Types.Add("string_list", new TypeDef("string_list", "List<string>", "DBCUtil.ExtractStringList", "DBCUtil.ExtractStringList", "DBCUtil.CloneList"));
                m_Types.Add("string_array", new TypeDef("string_array", "string[]", "DBCUtil.ExtractStringArray", "DBCUtil.ExtractStringArray", "DBCUtil.CloneArray"));
                m_Types.Add("int_list", new TypeDef("int_list", "List<int>", "DBCUtil.ExtractNumericList<int>", "DBCUtil.ExtractIntList", "DBCUtil.CloneList"));
                m_Types.Add("int_array", new TypeDef("int_array", "int[]", "DBCUtil.ExtractNumericArray<int>", "DBCUtil.ExtractIntArray", "DBCUtil.CloneArray"));
                m_Types.Add("float_list", new TypeDef("float_list", "List<float>", "DBCUtil.ExtractNumericList<float>", "DBCUtil.ExtractFloatList", "DBCUtil.CloneList"));
                m_Types.Add("float_array", new TypeDef("float_array", "float[]", "DBCUtil.ExtractNumericArray<float>", "DBCUtil.ExtractFloatArray", "DBCUtil.CloneArray"));
                return !haveError;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
        internal void GenAllReaders(Dictionary<string, string> tableFiles)
        {
            System.Diagnostics.Process p = System.Diagnostics.Process.Start("cmd", "/c del /f/q *.cs");
            p.WaitForExit();

            // gen path define
            bool firstFile = true;
            foreach (var tablePair in m_Tables)
            {
                TableDef tableDef = tablePair.Value;
                tableDef.m_HookAfterCollectData = false;
                tableDef.m_HookBeforeCollectData = false;
                tableDef.m_HookAfterLoad = false;
                tableDef.m_HookBeforeLoad = false;
                tableDef.m_RecordGenClone = false;
            }
            string file = "FilePathDefine" + ".cs";
            try
            {
                using (StreamWriter sw = new StreamWriter(file, true))
                {
                    sw.WriteLine("//----------------------------------------------------------------------------");
                    sw.WriteLine("//！！！不要手动修改此文件，此文件由TableReaderGenerator按table.dsl生成！！！");
                    sw.WriteLine("//----------------------------------------------------------------------------");
                    sw.WriteLine("using System;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.Runtime.InteropServices;");
                    sw.WriteLine("using System.IO;");
                    sw.WriteLine("using System.Text;");
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            if (firstFile)
            {
                firstFile = false;
                using (StreamWriter sw = new StreamWriter(file, true))
                {
                    sw.WriteLine();
                    sw.WriteLine("namespace Entitas.Data");
                    sw.WriteLine("{");
                    sw.WriteLine("\tinternal class FilePathDefine_Client");
                    sw.WriteLine("\t{");
                    foreach (var pair in tableFiles)
                    {
                        string filename = pair.Key;
                        string filepath = pair.Value;

                        sw.WriteLine("\t\tpublic const string C_{0} = \"{1}\";", filename, HomePath.RelativePath(HomePath.Instance.GetAbsolutePath(m_BasePath), filepath));
                    }
                    sw.WriteLine("\t}");
                    sw.WriteLine("\tinternal class FilePathDefine_Server");
                    sw.WriteLine("\t{");
                    foreach (var pair in tableFiles)
                    {
                        string filename = pair.Key;
                        string filepath = pair.Value;

                        sw.WriteLine("\t\tpublic const string C_{0} = \"{1}\";", filename, HomePath.RelativePath(HomePath.Instance.GetAbsolutePath(m_BasePath), filepath));
                    }
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    sw.Close();
                }
            }

            HashSet<string> tableHeaders = new HashSet<string>();
            foreach (var tablePair in m_Tables)
            {
                string table = tablePair.Key;
                TableDef tableDef = tablePair.Value;
                file = tableDef.m_CsFileName + ".cs";
                if (tableHeaders.Add(file))
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(file, true))
                        {
                            sw.WriteLine("//----------------------------------------------------------------------------");
                            sw.WriteLine("//！！！不要手动修改此文件，此文件由TableReaderGenerator按table.dsl生成！！！");
                            sw.WriteLine("//----------------------------------------------------------------------------");
                            sw.WriteLine("using System;");
                            sw.WriteLine("using System.Collections.Generic;");
                            sw.WriteLine("using System.Runtime.InteropServices;");
                            sw.WriteLine("using System.IO;");
                            sw.WriteLine("using System.Text;");
                            sw.WriteLine("using Util;");
                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                if (table == "AttributeConfig")
                {
                    tableDef.m_HookAfterCollectData = true;
                    GenAttributeData(table, "AttributeConfigProvider.cs", "AttributeEnum.cs", "AttributeData.cs");
                }
                GenReader(table, file, true);
            }
        }
        internal void GenReader(string table, string file, bool append)
        {
            TableDef tableDef;
            if (m_Tables.TryGetValue(table, out tableDef))
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(file, append))
                    {
                        string firstMemberName = "";
                        sw.WriteLine();
                        sw.WriteLine("namespace Entitas.Data");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic sealed{0} class {1} : IData2", tableDef.m_RecordModifier, tableDef.m_RecordName);
                        sw.WriteLine("\t{");
                        sw.WriteLine("\t\t[StructLayout(LayoutKind.Auto, Pack = 1, Size = {0})]", tableDef.GetLayoutFieldCount() * sizeof(int));
                        sw.WriteLine("\t\tprivate struct {0}Record", tableDef.m_RecordName);
                        sw.WriteLine("\t\t{");
                        foreach (FieldDef fieldDef in tableDef.m_Fields)
                        {
                            TypeDef typeDef;
                            if (!fieldDef.m_IsCustom)
                            {
                                if (m_Types.TryGetValue(fieldDef.m_Type, out typeDef))
                                {
                                    sw.WriteLine("\t\t\tinternal {0} {1};", GetRecordType(typeDef.m_TypeCode), fieldDef.m_MemberName);
                                    if (string.IsNullOrEmpty(firstMemberName))
                                    {
                                        firstMemberName = fieldDef.m_MemberName;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Can't find type {0}'s definition ! table {1}", fieldDef.m_Type, table);
                                }
                            }
                        }
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        foreach (FieldDef fieldDef in tableDef.m_Fields)
                        {
                            TypeDef typeDef;
                            if (fieldDef.m_IsCustom)
                            {
                                if (!string.IsNullOrEmpty(fieldDef.m_MemberName))
                                {
                                    sw.WriteLine("\t\tpublic {0} {1};", fieldDef.m_Type, fieldDef.m_MemberName);
                                }
                            }
                            else if (m_Types.TryGetValue(fieldDef.m_Type, out typeDef))
                            {
                                sw.WriteLine("\t\t{0} {1} {2};", fieldDef.m_Access, typeDef.m_TypeCode, fieldDef.m_MemberName);
                                if (string.IsNullOrEmpty(firstMemberName))
                                {
                                    firstMemberName = fieldDef.m_MemberName;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Can't find type {0}'s definition ! table {1}", fieldDef.m_Type, table);
                            }
                        }
                        sw.WriteLine();
                        sw.WriteLine("\t\t{0}", "public bool CollectDataFromDBC(DBC_Row node)");
                        sw.WriteLine("\t\t{");
                        if (tableDef.m_HookBeforeCollectData)
                        {
                            sw.WriteLine("\t\t\tBeforeCollectData();");
                        }
                        foreach (FieldDef fieldDef in tableDef.m_Fields)
                        {
                            TypeDef typeDef;
                            if (fieldDef.m_IsCustom)
                            {
                                sw.WriteLine("\t{0}", fieldDef.m_TextCode);
                            }
                            else if (m_Types.TryGetValue(fieldDef.m_Type, out typeDef))
                            {
                                if (fieldDef.m_IsString)
                                    sw.WriteLine("\t\t\t{0} = {1}(node, \"{2}\", \"{3}\");", fieldDef.m_MemberName, typeDef.m_TextCode, fieldDef.m_FieldName, fieldDef.m_Default);
                                else
                                    sw.WriteLine("\t\t\t{0} = {1}(node, \"{2}\", {3});", fieldDef.m_MemberName, typeDef.m_TextCode, fieldDef.m_FieldName, fieldDef.m_Default);
                            }
                            else
                            {
                                Console.WriteLine("Can't find type {0}'s definition ! table {1}", fieldDef.m_Type, table);
                            }
                        };
                        if (tableDef.m_HookAfterCollectData)
                        {
                            sw.WriteLine("\t\t\tAfterCollectData();");
                        }
                        sw.WriteLine("\t\t\treturn true;");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\t{0}", "public bool CollectDataFromBinary(BinaryTable table, int index)");
                        sw.WriteLine("\t\t{");
                        if (tableDef.m_HookBeforeCollectData)
                        {
                            sw.WriteLine("\t\t\tBeforeCollectData();");
                        }
                        sw.WriteLine("\t\t\t{0}Record record = GetRecord(table,index);", tableDef.m_RecordName);
                        foreach (FieldDef fieldDef in tableDef.m_Fields)
                        {
                            TypeDef typeDef;
                            if (fieldDef.m_IsCustom)
                            {
                                sw.WriteLine("\t{0}", fieldDef.m_BinaryCode);
                            }
                            else if (m_Types.TryGetValue(fieldDef.m_Type, out typeDef))
                            {
                                if (fieldDef.m_IsString)
                                    sw.WriteLine("\t\t\t{0} = {1}(table, record.{2}, \"{3}\");", fieldDef.m_MemberName, typeDef.m_BinaryCode, fieldDef.m_MemberName, fieldDef.m_Default);
                                else
                                    sw.WriteLine("\t\t\t{0} = {1}(table, record.{2}, {3});", fieldDef.m_MemberName, typeDef.m_BinaryCode, fieldDef.m_MemberName, fieldDef.m_Default);
                            }
                            else
                            {
                                Console.WriteLine("Can't find type {0}'s definition ! table {1}", fieldDef.m_Type, table);
                            }
                        }
                        if (tableDef.m_HookAfterCollectData)
                        {
                            sw.WriteLine("\t\t\tAfterCollectData();");
                        }
                        sw.WriteLine("\t\t\treturn true;");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic void AddToBinary(BinaryTable table)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\t{0}Record record = new {1}Record();", tableDef.m_RecordName, tableDef.m_RecordName);
                        foreach (FieldDef fieldDef in tableDef.m_Fields)
                        {
                            TypeDef typeDef;
                            if (!fieldDef.m_IsCustom)
                            {
                                if (m_Types.TryGetValue(fieldDef.m_Type, out typeDef))
                                {
                                    if (fieldDef.m_IsString)
                                        sw.WriteLine("\t\t\trecord.{0} = {1}(table, {2}, \"{3}\");", fieldDef.m_MemberName, typeDef.m_RecordCode, fieldDef.m_MemberName, fieldDef.m_Default);
                                    else
                                        sw.WriteLine("\t\t\trecord.{0} = {1}(table, {2}, {3});", fieldDef.m_MemberName, typeDef.m_RecordCode, fieldDef.m_MemberName, fieldDef.m_Default);
                                }
                                else
                                {
                                    Console.WriteLine("Can't find type {0}'s definition ! table {1}", fieldDef.m_Type, table);
                                }
                            }
                        }
                        sw.WriteLine("\t\t\tbyte[] bytes = GetRecordBytes(record);");
                        sw.WriteLine("\t\t\ttable.Records.Add(bytes);");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        if (tableDef.m_RecordGenClone)
                        {
                            sw.WriteLine("\t\tpublic {0} Clone()", tableDef.m_RecordName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\t{0} clone = new {1}();", tableDef.m_RecordName, tableDef.m_RecordName);
                            foreach (FieldDef fieldDef in tableDef.m_Fields)
                            {
                                TypeDef typeDef;
                                if (fieldDef.m_IsCustom)
                                {
                                    sw.WriteLine("\t\t\tclone.{0} = this.Clone();", fieldDef.m_MemberName);
                                }
                                else if (m_Types.TryGetValue(fieldDef.m_Type, out typeDef))
                                {
                                    sw.WriteLine("\t\t\tclone.{0} = {1}(this.{2});", fieldDef.m_MemberName, typeDef.m_CloneCode, fieldDef.m_MemberName);
                                }
                                else
                                {
                                    Console.WriteLine("Can't find type {0}'s definition ! table {1}", fieldDef.m_Type, table);
                                }
                            }
                            sw.WriteLine("\t\t\treturn clone;");
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                        }
                        if (!tableDef.m_OverrideGetId)
                        {
                            sw.WriteLine("\t\t{0}", "public int GetId()");
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\treturn {0};", firstMemberName);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                        }
                        sw.WriteLine("\t\tprivate unsafe {0}Record GetRecord(BinaryTable table, int index)", tableDef.m_RecordName);
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\t{0}Record record;", tableDef.m_RecordName);
                        sw.WriteLine("\t\t\tbyte[] bytes = table.Records[index];");
                        sw.WriteLine("\t\t\tfixed (byte* p = bytes) {");
                        sw.WriteLine("\t\t\t\trecord = *({0}Record*)p;", tableDef.m_RecordName);
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\treturn record;");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("\t\tprivate static unsafe byte[] GetRecordBytes({0}Record record)", tableDef.m_RecordName);
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tbyte[] bytes = new byte[sizeof({0}Record)];", tableDef.m_RecordName);
                        sw.WriteLine("\t\t\tfixed (byte* p = bytes) {");
                        sw.WriteLine("\t\t\t\t{0}Record* temp = ({1}Record*)p;", tableDef.m_RecordName, tableDef.m_RecordName);
                        sw.WriteLine("\t\t\t\t*temp = record;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\treturn bytes;");
                        sw.WriteLine("\t\t}");
                        if (!string.IsNullOrEmpty(tableDef.m_RecordCode))
                        {
                            sw.WriteLine("\t{0}", tableDef.m_RecordCode);
                        }
                        sw.WriteLine("\t}");
                        sw.WriteLine();
                        sw.WriteLine("\tpublic sealed{0} class {1}", tableDef.m_ProviderModifier, tableDef.m_ProviderName);
                        sw.WriteLine("\t{");
                        if (!string.IsNullOrEmpty(tableDef.m_ProviderCode))
                        {
                            sw.WriteLine("\t{0}", tableDef.m_ProviderCode);
                        }
                        if (tableDef.m_FileType == TableFileTypeEnum.CLIENT || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\t\tpublic void LoadForClient()");
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\tLoad(FilePathDefine_Client.C_{0});", table);
                            sw.WriteLine("\t\t}");
                        }
                        if (tableDef.m_FileType == TableFileTypeEnum.SERVER || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\t\tpublic void LoadForServer()");
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\tLoad(FilePathDefine_Server.C_{0});", table);
                            sw.WriteLine("\t\t}");
                        }
                        sw.WriteLine("\t\tpublic void Load(string file)");
                        sw.WriteLine("\t\t{");
                        if (tableDef.m_HookBeforeLoad)
                        {
                            sw.WriteLine("\t\t\tBeforeLoad();");
                        }
                        sw.WriteLine("\t\t\tif (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {");
                        sw.WriteLine("\t\t\t\tm_{0}Mgr.CollectDataFromBinary(file);", tableDef.m_RecordName);
                        sw.WriteLine("\t\t\t} else {");
                        sw.WriteLine("\t\t\t\tm_{0}Mgr.CollectDataFromDBC(file);", tableDef.m_RecordName);
                        sw.WriteLine("\t\t\t}");
                        if (tableDef.m_HookAfterLoad)
                        {
                            sw.WriteLine("\t\t\tAfterLoad();");
                        }
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("\t\tpublic void Save(string file)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t#if DEBUG");
                        sw.WriteLine("\t\t\tm_{0}Mgr.SaveToBinary(file);", tableDef.m_RecordName);
                        sw.WriteLine("\t\t#endif");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("\t\tpublic void Clear()");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tm_{0}Mgr.Clear();", tableDef.m_RecordName);
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        if (0 == tableDef.m_Type.CompareTo("dictionary"))
                        {
                            sw.WriteLine("\t\tpublic DataDictionaryMgr2<{0}> {1}Mgr", tableDef.m_RecordName, tableDef.m_RecordName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\tget {{ return m_{0}Mgr; }}", tableDef.m_RecordName);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                            sw.WriteLine("\t\tpublic int Get{0}Count()", tableDef.m_RecordName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\treturn m_{0}Mgr.GetDataCount();", tableDef.m_RecordName);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                            sw.WriteLine("\t\tpublic {0} Get{1}(int id)", tableDef.m_RecordName, tableDef.m_RecordName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\treturn m_{0}Mgr.GetDataById(id);", tableDef.m_RecordName);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                            sw.WriteLine("\t\tprivate DataDictionaryMgr2<{0}> m_{1}Mgr = new DataDictionaryMgr2<{2}>();", tableDef.m_RecordName, tableDef.m_RecordName, tableDef.m_RecordName);
                        }
                        else
                        {
                            sw.WriteLine("\t\tpublic DataListMgr2<{0}> {1}Mgr", tableDef.m_RecordName, tableDef.m_RecordName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\tget {{ return m_{0}Mgr; }}", tableDef.m_RecordName);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                            sw.WriteLine("\t\tpublic int Get{0}Count()", tableDef.m_RecordName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\treturn m_{0}Mgr.GetDataCount();", tableDef.m_RecordName);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine();
                            sw.WriteLine("\t\tprivate DataListMgr2<{0}> m_{1}Mgr = new DataListMgr2<{2}>();", tableDef.m_RecordName, tableDef.m_RecordName, tableDef.m_RecordName);
                        }
                        sw.WriteLine();
                        if (tableDef.m_FileType != TableFileTypeEnum.MULTIFILE)
                        {
                            sw.WriteLine("\t\tpublic static {0} Instance", tableDef.m_ProviderName);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\tget { return s_Instance; }");
                            sw.WriteLine("\t\t}");
                            sw.WriteLine("\t\tprivate static {0} s_Instance = new {1}();", tableDef.m_ProviderName, tableDef.m_ProviderName);
                        }
                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("Can't find table {0}'s definition !", table);
            }
        }
        internal void GenConverter(string file)
        {
            File.Delete(file);
            try
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine("//----------------------------------------------------------------------------");
                    sw.WriteLine("//！！！不要手动修改此文件，此文件由TableReaderGenerator按table.dsl生成！！！");
                    sw.WriteLine("//----------------------------------------------------------------------------");
                    sw.WriteLine("using System;");
                    sw.WriteLine("using System.Collections.Generic;");
                    sw.WriteLine("using System.IO;");
                    sw.WriteLine("using System.Text;");
                    sw.WriteLine("using DashFire;");
                    sw.WriteLine();

                    sw.WriteLine("public static class DataConverter");
                    sw.WriteLine("{");

                    // RegisterTableHandler()
                    sw.WriteLine(@"
  private delegate void TableHandler(string dir, HashSet<string> binFiles);
  private static Dictionary<string, TableHandler> s_ClientTableHandlers = new Dictionary<string, TableHandler>();
  private static Dictionary<string, TableHandler> s_ServerTableHandlers = new Dictionary<string, TableHandler>();
  private static void RegisterTableHandler(string filePath, TableHandler handler, bool isServer)
  {
    Dictionary<string, TableHandler> handlers = s_ClientTableHandlers;
    if (isServer) {
      handlers = s_ServerTableHandlers;
    }
    if (handlers.ContainsKey(filePath)) {
      handlers[filePath] = handler;
    } else {
      handlers.Add(filePath, handler);
    }
  }
          ");

                    // RegisterAll()
                    sw.WriteLine("\tpublic static void RegisterAll()");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\ts_ClientTableHandlers.Clear();");
                    sw.WriteLine("\t\ts_ServerTableHandlers.Clear();");
                    sw.WriteLine("\t\t// Client");
                    foreach (var pair in m_Tables)
                    {
                        string table = pair.Key;
                        TableDef tableDef = pair.Value;
                        if (tableDef.m_FileType == TableFileTypeEnum.CLIENT || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\t\tRegisterTableHandler(DashFire.FilePathDefine_Client.C_{0}, ConvertClient_{0}, false);", table);
                        }
                    }
                    sw.WriteLine("\t\t// Server");
                    foreach (var pair in m_Tables)
                    {
                        string table = pair.Key;
                        TableDef tableDef = pair.Value;
                        if (tableDef.m_FileType == TableFileTypeEnum.SERVER || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\t\tRegisterTableHandler(DashFire.FilePathDefine_Server.C_{0}, ConvertServer_{0}, true);", table);
                        }
                    }
                    sw.WriteLine("\t}");

                    // ConverClient_*()
                    foreach (var pair in m_Tables)
                    {
                        string table = pair.Key;
                        TableDef tableDef = pair.Value;
                        if (tableDef.m_FileType == TableFileTypeEnum.CLIENT || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\tprivate static void ConvertClient_{0}(string dir, HashSet<string> binFiles)", table);
                            sw.WriteLine("\t{");
                            sw.WriteLine("\t\tstring path = Path.Combine(dir, DashFire.FilePathDefine_Client.C_{0});", table);
                            sw.WriteLine("\t\tbinFiles.Add(Path.GetFileName(path));");
                            sw.WriteLine("\t\t{0}.Instance.Load(path);", tableDef.m_ProviderName);
                            sw.WriteLine("\t\t{0}.Instance.Save(path);", tableDef.m_ProviderName);
                            sw.WriteLine("\t}");
                        }
                    }

                    // ConvertClient()
                    sw.WriteLine("\tpublic static void ConvertClient(string dir, HashSet<string> binFiles)");
                    sw.WriteLine("\t{");
                    foreach (var pair in m_Tables)
                    {
                        string table = pair.Key;
                        TableDef tableDef = pair.Value;
                        if (tableDef.m_FileType == TableFileTypeEnum.CLIENT || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\t\tConvertClient_{0}(dir, binFiles);", table);
                        }
                    }
                    sw.WriteLine("\t}");

                    // ConvertServer_*()
                    foreach (var pair in m_Tables)
                    {
                        string table = pair.Key;
                        TableDef tableDef = pair.Value;
                        if (tableDef.m_FileType == TableFileTypeEnum.SERVER || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\tprivate static void ConvertServer_{0}(string dir, HashSet<string> binFiles)", table);
                            sw.WriteLine("\t{");
                            sw.WriteLine("\t\tstring path = Path.Combine(dir, DashFire.FilePathDefine_Server.C_{0});", table);
                            sw.WriteLine("\t\tbinFiles.Add(Path.GetFileName(path));");
                            sw.WriteLine("\t\t{0}.Instance.Load(path);", tableDef.m_ProviderName);
                            sw.WriteLine("\t\t{0}.Instance.Save(path);", tableDef.m_ProviderName);
                            sw.WriteLine("\t}");
                        }
                    }

                    // ConvertServer()
                    sw.WriteLine("\tpublic static void ConvertServer(string dir, HashSet<string> binFiles)");
                    sw.WriteLine("\t{");
                    foreach (var pair in m_Tables)
                    {
                        string table = pair.Key;
                        TableDef tableDef = pair.Value;
                        if (tableDef.m_FileType == TableFileTypeEnum.SERVER || tableDef.m_FileType == TableFileTypeEnum.PUBLIC)
                        {
                            sw.WriteLine("\t\tConvertServer_{0}(dir, binFiles);", table);
                        }
                    }
                    sw.WriteLine("\t}");
                    // ConvertClientFile()/ConvertServerFile()
                    sw.WriteLine(@"
  public static void ConvertClientFile(string dir, HashSet<string> binFiles, string destFilePath)
  {
    if (s_ClientTableHandlers.ContainsKey(destFilePath)) {
      s_ClientTableHandlers[destFilePath](dir, binFiles);
    }
  }
  public static void ConvertServerFile(string dir, HashSet<string> binFiles, string destFilePath)
  {
    if (s_ServerTableHandlers.ContainsKey(destFilePath)) {
      s_ServerTableHandlers[destFilePath](dir, binFiles);
    }
  }
          ");
                    sw.WriteLine("}");

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal void GenAttributeData(string table, string providerFile, string enumFile, string dataFile)
        {
            string attributeEnumName = "AttrbuteEnum";
            List<string> ignoreFileds = new List<string> { "Id", "Describe" };
            TableDef tableDef;
            if (m_Tables.TryGetValue(table, out tableDef))
            {
                try
                {
                    using(StreamWriter sw = new StreamWriter(providerFile, false))
                    {
                        sw.WriteLine("//----------------------------------------------------------------------------");
                        sw.WriteLine("//！！！不要手动修改此文件，此文件由LogicDataGenerator按AttributeConfig.txt生成！！！");
                        sw.WriteLine("//----------------------------------------------------------------------------");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("using Util;");
                        sw.WriteLine("");
                        sw.WriteLine("namespace Entitas.Data");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic sealed partial class AttributeConfig : IData2");
                        sw.WriteLine("\t{");
                        sw.WriteLine("\tprivate enum ValueType : int");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tAbsoluteValue = 0,");
                        sw.WriteLine("\t\t\tPercentValue,");
                        sw.WriteLine("\t\t\tLevelRateValue,");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("\t\tpublic const int c_MaxAbsoluteValue = 1000000000;");
                        sw.WriteLine("\t\tpublic const int c_MaxPercentValue = 2000000000;");
                        sw.WriteLine("\t\tpublic const float c_Rate = 100.0f;");
                        sw.WriteLine("");
                        foreach (var member in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(member.m_FieldName))
                                continue;
                            string name = member.m_FieldName;

                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\tprivate float m_Add{0} = 0;", name);
                            sw.WriteLine("\t\tprivate int m_{0}Type = 0;", name);
                        }
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprivate void AfterCollectData()");
                        sw.WriteLine("\t\t{");
                        foreach (var member in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(member.m_FieldName))
                                continue;
                            string name = member.m_FieldName;

                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\t\tm_Add{0} = CalcRealValue(Add{0}, out m_{0}Type);", name);
                        }
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        foreach (var member in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(member.m_FieldName))
                                continue;
                            string name = member.m_FieldName;

                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\tpublic float Get{0}(float refVal, int refLevel)", name);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\treturn CalcAddedAttrValue(refVal, refLevel, m_Add{0}, m_{0}Type);", name);
                            sw.WriteLine("\t\t}");
                        }
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprivate float CalcRealValue(int tableValue, out int type)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tfloat retVal = 0;");
                        sw.WriteLine("\t\t\tint val = tableValue;");
                        sw.WriteLine("\t\t\tbool isNegative = false;");
                        sw.WriteLine("\t\t\tif(tableValue < 0){;");
                        sw.WriteLine("\t\t\t\tisNegative =true;");
                        sw.WriteLine("\t\t\t\tval = -val;");
                        sw.WriteLine("\t\t\t};");
                        sw.WriteLine("\t\t\tif(val < c_MaxAbsoluteValue) {");
                        sw.WriteLine("\t\t\t\tretVal = val / c_Rate;");
                        sw.WriteLine("\t\t\t\ttype = (int)ValueType.AbsoluteValue;");
                        sw.WriteLine("\t\t\t}else if(val < c_MaxPercentValue) {");
                        sw.WriteLine("\t\t\t\tretVal = (val - c_MaxAbsoluteValue) / c_Rate / 100;");
                        sw.WriteLine("\t\t\t\ttype = (int)ValueType.PercentValue;");
                        sw.WriteLine("\t\t\t}else{");
                        sw.WriteLine("\t\t\t\tretVal = (val - c_MaxPercentValue) / c_Rate / 100;");
                        sw.WriteLine("\t\t\t\ttype = (int)ValueType.LevelRateValue;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\tif(isNegative)");
                        sw.WriteLine("\t\t\t\tretVal = -retVal;");
                        sw.WriteLine("\t\t\treturn retVal;");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("");

                        sw.WriteLine("\t\tprivate float CalcAddedAttrValue(float refVal, int refLevel, float addVal, int type)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tfloat retVal = 0;");
                        sw.WriteLine("\t\t\tswitch(type){");
                        sw.WriteLine("\t\t\t\tcase (int)ValueType.AbsoluteValue:");
                        sw.WriteLine("\t\t\t\t\tretVal = addVal;");
                        sw.WriteLine("\t\t\t\t\tbreak;");
                        sw.WriteLine("\t\t\t\tcase (int)ValueType.PercentValue:");
                        sw.WriteLine("\t\t\t\t\tretVal = refVal * addVal;");
                        sw.WriteLine("\t\t\t\t\tbreak;");
                        sw.WriteLine("\t\t\t\tcase (int)ValueType.LevelRateValue:");
                        sw.WriteLine("\t\t\t\t\tretVal = refLevel * addVal;");
                        sw.WriteLine("\t\t\t\t\tbreak;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\treturn retVal;");
                        sw.WriteLine("\t\t}");


                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        sw.WriteLine("");
                        sw.Close();
                    }
                    using (StreamWriter sw = new StreamWriter(enumFile, false))
                    {
                        sw.WriteLine("//----------------------------------------------------------------------------");
                        sw.WriteLine("//！！！不要手动修改此文件，此文件由LogicDataGenerator按AttributeConfig.txt生成！！！");
                        sw.WriteLine("//----------------------------------------------------------------------------");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("");
                        sw.WriteLine("namespace Entitas.Data");
                        sw.WriteLine("{");

                        sw.WriteLine("\tpublic enum {0}", attributeEnumName);
                        sw.WriteLine("\t{");
                        sw.WriteLine("\t\tActual_Invalid = -1,");
                        foreach (var member in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(member.m_FieldName))
                                continue;
                            string name = member.m_FieldName;

                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }

                            sw.WriteLine("\t\tActual_{0},", name);
                        }

                        sw.WriteLine("\t\tActual_Max");
                        sw.WriteLine("\t}");

                        sw.WriteLine("}");
                        sw.WriteLine("");
                        sw.Close();

                    }

                    using (StreamWriter sw = new StreamWriter(dataFile, true))
                    {
                        sw.WriteLine("//----------------------------------------------------------------------------");
                        sw.WriteLine("//！！！不要手动修改此文件，此文件由LogicDataGenerator按AttributeConfig.txt生成！！！");
                        sw.WriteLine("//----------------------------------------------------------------------------");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections.Generic;");

                        sw.WriteLine();
                        sw.WriteLine("namespace Entitas.Data");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic sealed class AttributeData");
                        sw.WriteLine("\t{");

                        sw.WriteLine("\t\t//----------------------------------------------------------------------------");
                        sw.WriteLine("\t\t//基础属性以及读写接口");
                        sw.WriteLine("\t\t//----------------------------------------------------------------------------");

                        foreach (var memberDef in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(memberDef.m_FieldName))
                                continue;

                            string name = memberDef.m_FieldName;
                            string type = memberDef.m_Type;

                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\tpublic void Set{0}(Operate_Type opType, float tVal)", name);
                            sw.WriteLine("\t\t{");
                            sw.WriteLine("\t\t\tm_{0} = UpdateAttr(m_{0}, m_{0}, opType, tVal);", name);
                            sw.WriteLine("\t\t}");
                            sw.WriteLine("\t\tpublic float {0}", name);
                            sw.WriteLine("\t\t{");
                            sw.Write("\t\t\tget { return m_");
                            sw.Write("{0}", name);
                            sw.WriteLine(" / s_Key; }");
                            sw.WriteLine("\t\t}");
                            sw.WriteLine("\t\tprivate float m_{0};", name);
                            sw.WriteLine();
                        }

                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\t//指定属性枚举获取属性值，全部转化为float类型,返回值需要自行转换类型");
                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\tpublic float GetAttributeByType({0} attrType)", attributeEnumName);
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tfloat attValue = 0;");
                        sw.WriteLine("\t\t\tswitch (attrType) {");
                        foreach (var memberDef in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(memberDef.m_FieldName))
                                continue;

                            string name = memberDef.m_MemberName;
                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\t\t\tcase {0}.Actual_{1}:", attributeEnumName, name);
                            sw.WriteLine("\t\t\t\t\tattValue = {0};", name);
                            sw.WriteLine("\t\t\t\t\tbreak;");
                        }
                        sw.WriteLine("\t\t\t\tdefault:");
                        sw.WriteLine("\t\t\t\t\tattValue = -1;");
                        sw.WriteLine("\t\t\t\t\tbreak;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\treturn attValue;");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\t//指定属性枚举值设置属性值,属性值参数为float类型，内部根据属性类型自行转换");
                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\tpublic void SetAttributeByType({0} attrType, Operate_Type opType, float tVal)", attributeEnumName);
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tswitch (attrType) {");
                        foreach (var memberDef in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(memberDef.m_FieldName))
                                continue;

                            string name = memberDef.m_MemberName;
                            string type = memberDef.m_Type;
                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\t\t\tcase {0}.Actual_{1}:", attributeEnumName, name);
                            sw.WriteLine("\t\t\t\t\tSet{0}(opType, ({1})tVal);", name, type);
                            sw.WriteLine("\t\t\t\t\tbreak;");
                        }
                        sw.WriteLine("\t\t\tdefault:");
                        sw.WriteLine("\t\t\t\tbreak;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t}"); sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\t//属性修改接口");
                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\tpublic static float UpdateAttr(float val, float maxVal, Operate_Type opType, float tVal)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tfloat ret = val;");
                        sw.WriteLine("\t\t\tif (opType == Operate_Type.OT_PercentMax) {");
                        sw.WriteLine("\t\t\t\tfloat t = maxVal * (tVal / 100.0f);");
                        sw.WriteLine("\t\t\t\tret = t;");
                        sw.WriteLine("\t\t\t} else {");
                        sw.WriteLine("\t\t\t\tret = UpdateAttr(val, opType, tVal);");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\treturn ret;");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine();
                        sw.WriteLine("\t\tpublic static float UpdateAttr(float val, Operate_Type opType, float tVal)");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tfloat ret = val;");
                        sw.WriteLine("\t\t\tif (opType == Operate_Type.OT_Absolute) {");
                        sw.WriteLine("\t\t\t\tret = tVal * s_Key;");
                        sw.WriteLine("\t\t\t} else if (opType == Operate_Type.OT_Relative) {");
                        sw.WriteLine("\t\t\t\tfloat t = (ret + tVal * s_Key);");
                        sw.WriteLine("\t\t\t\tif (t < 0) {");
                        sw.WriteLine("\t\t\t\t\tt = 0;");
                        sw.WriteLine("\t\t\t\t}");
                        sw.WriteLine("\t\t\t\tret = t;");
                        sw.WriteLine("\t\t\t} else if (opType == Operate_Type.OT_PercentCurrent) {");
                        sw.WriteLine("\t\t\t\tfloat t = (ret * (tVal / 100.0f));");
                        sw.WriteLine("\t\t\t\tret = t;");
                        sw.WriteLine("\t\t\t}");
                        sw.WriteLine("\t\t\treturn ret;");
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\t// 属性初始化接口");
                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\tpublic void SetAbsoluteByConfig(AttributeConfig attr)");
                        sw.WriteLine("\t\t{");
                        foreach (var memberDef in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(memberDef.m_FieldName))
                                continue;

                            string name = memberDef.m_MemberName;
                            string type = memberDef.m_Type;
                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\t\tfloat a{0} = attr.Get{0}(0, 0);", name);
                        }
                        foreach (var memberDef in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(memberDef.m_FieldName))
                                continue;

                            string name = memberDef.m_MemberName;
                            string type = memberDef.m_Type;
                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\t\tSet{0}(Operate_Type.OT_Absolute, a{0});", name);
                        }
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t\tpublic void SetRelativeByConfig(AttributeConfig attr)");
                        sw.WriteLine("\t\t{");
                        foreach (var memberDef in tableDef.m_Fields)
                        {
                            if (ignoreFileds.Contains(memberDef.m_FieldName))
                                continue;

                            string name = memberDef.m_MemberName;
                            string type = memberDef.m_Type;
                            if(name.StartsWith("Add"))
                            {
                                name = name.Substring("Add".Length);
                            }
                            sw.WriteLine("\t\t\tSet{0}(Operate_Type.OT_Relative, attr.Get{0}({0}, 0));", name);
                        }
                        sw.WriteLine("\t\t}");

                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\t//注意：Key的修改应该在所有对象创建前执行，否则属性会乱！！！");
                        sw.WriteLine("\t\t//------------------------------------------------------------------------");
                        sw.WriteLine("\t\tpublic static int Key");
                        sw.WriteLine("\t\t{");
                        sw.WriteLine("\t\t\tget { return s_Key; }");
                        sw.WriteLine("\t\t\tset { s_Key = value; }");
                        sw.WriteLine("\t\t}");
                        sw.WriteLine("\t\tprivate static int s_Key = 1;");
                        sw.WriteLine();
                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            else
            {
                Console.WriteLine("Can't find table {0}'s definition !", table);
            }
        }

        private string GetRecordType(string type)
        {
            if (0 == type.CompareTo("int") ||
              0 == type.CompareTo("float"))
            {
                return type;
            }
            else
            {
                return "int";
            }
        }

        private string m_BasePath = string.Empty;
        private const string m_DefGlobalCsFileName = "Global";
        private const string m_DefToolCsFileName = "Tool";
        private SortedDictionary<string, List<string>> m_GlobalCodes = new SortedDictionary<string, List<string>>();
        private SortedDictionary<string, List<string>> m_ToolCodes = new SortedDictionary<string, List<string>>();
        private SortedDictionary<string, TypeDef> m_Types = new SortedDictionary<string, TypeDef>();
        private SortedDictionary<string, TableDef> m_Tables = new SortedDictionary<string, TableDef>();
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            LogUtil.OnOutput = (Log_Type type, string msg) =>
            {
                Console.WriteLine("{0}", msg);
            };
            FileReaderProxy.RegisterReadFileHandler((string path) =>
            {
                byte[] buffer = null;
                try
                {
                    using(var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                    }
                }
                catch(Exception ex)
                {
                    LogUtil.Error("{0}\n{1}", ex.Message, ex.StackTrace);
                }
                return buffer;
            });
            if (args.Length == 1)
            {
                if (args[0] == "gendsl")
                {
                    LogUtil.Info("GenAllDsl");
                    GenAllDsl();
                    GenDataReader(true);
                }
                else if (args[0] == "gentoolreader")
                {
                    LogUtil.Info("GenToolReader");
                    GenDataReader(true);
                }
            }
            else
            {
                LogUtil.Info("GenDataReader");
                GenAllDsl();
                GenDataReader(false);
            }
        }

        private static void GenDataReader(bool genToolReader)
        {
            TableDslParser parser = new TableDslParser();
            parser.Init("table.dsl", "../../Resource/");
            parser.GenAllReaders(GetAllTableFiles());
            if (genToolReader)
            {
                parser.GenConverter("DataConverter.cs");
            }
        }

        private static Dictionary<string, string> GetAllTableFiles()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            try
            {
                DirectoryInfo folder = new DirectoryInfo(basePath);
                foreach (FileInfo file in folder.GetFiles("*.txt"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.FullName);
                    if (dict.ContainsKey(fileName))
                    {
                        LogUtil.Error("File name duplication, fileName: {0}", fileName);
                    }
                    dict.Add(fileName, file.FullName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return dict;
        }

        private static void GenAllDsl()
        {
            try
            {
                File.Delete("table.dsl");
                DirectoryInfo folder = new DirectoryInfo(basePath);
                foreach (FileInfo file in folder.GetFiles("*.txt"))
                {
                    GenDsl(file.FullName);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private static void GenDsl(string file)
        {
            DBC dbc = new DBC();
            if (dbc.Load(file))
            {
                string dirName = Path.GetDirectoryName(file);
                string fileName = Path.GetFileName(file);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                string fileType;
                if (dirName.IndexOf("Scenes") >= 0 && 0 != fileName.CompareTo("SceneConfig.txt") && 0 != fileName.CompareTo("SceneDropOut.txt") && 0 != fileName.CompareTo("NpcDropOut.txt") && 0 != fileName.CompareTo("ReliveConfig.txt") && 0 != fileName.CompareTo("SceneTransport.txt"))
                    fileType = "multifile";
                else if (dirName.IndexOf("Client") >= 0)
                    fileType = "client";
                else if (dirName.IndexOf("Server") >= 0)
                    fileType = "server";
                else
                    fileType = "public";

                bool noPrefix = false;
                if (noPrefixFiles.Contains(fileName))
                {
                    noPrefix = true;
                }

                using (StreamWriter sw = new StreamWriter("table.dsl", true))
                {
                    sw.WriteLine("tabledef({0}, dictionary, {1})", fileNameWithoutExtension, fileType);
                    sw.WriteLine("{");
                    sw.WriteLine("\trecordmodifier(partial);");
                    sw.WriteLine("\tprovidermodifier(partial);");

                    List<string> fields = dbc.Header;
                    List<string> types = dbc.Types;
                    int ct = fields.Count;
                    if (noPrefix)
                    {
                        for (int ix = 0; ix < ct; ++ix)
                        {
                            if (string.IsNullOrEmpty(fields[ix]))
                                continue;

                            sw.WriteLine("\tfielddef({0}, {1}, {2});", fields[ix], fields[ix], types[ix]);
                        }
                    }
                    else
                    {
                        for (int ix = 0; ix < ct; ++ix)
                        {
                            if (string.IsNullOrEmpty(fields[ix]))
                                continue;

                            sw.WriteLine("\tfielddef({0}, {1}, {2});", fields[ix], fields[ix], types[ix]);
                        }
                    }

                    sw.WriteLine("};");
                    sw.Close();
                }
                LogUtil.Info("gen {0} to dsl.", file);
            }
        }

        private static void ConvertToUtf8(string file, string destfile)
        {
            Encoding ansi = Encoding.GetEncoding(936);
            File.WriteAllText(destfile, File.ReadAllText(file, ansi), Encoding.UTF8);
        }

        private static string GetFieldValue(DBC dbc, int colIx)
        {
            string val = null;
            int ct = dbc.RowNum;
            if (ct > 0)
            {
                for (int rowIx = 0; rowIx < ct; ++rowIx)
                {
                    DBC_Row row = dbc.GetRowByIndex(rowIx);
                    if (null != row)
                    {
                        val = row.SelectFieldByIndex(colIx);
                        if (null != val)
                        {
                            val = val.Trim();
                            break;
                        }
                    }
                }
            }
            return val;
        }

        private static readonly string basePath = "../../Resource/Tables/";

        private const string filters = "*.txt,*.dsl";
        private const string txtfilters = "*.txt";
        private const string dslfilters = "*.dsl";

        private static readonly HashSet<string> excludeFiles = new HashSet<string>(
          new string[] { "list.txt", "listall.txt", "SensitiveDictionary.txt", "ServerConfig.txt", "GameConfig.txt" });

        private static readonly HashSet<string> noPrefixFiles = new HashSet<string>(
          new string[] { "PromotionShopConfig.txt", "PromotionShopItemConfig.txt", "PartnerLevelUpConfig.txt", "PartnerStageUpConfig.txt", "PartnerConfig.txt", "GrowthFundConfig.txt", "SkillData.txt"
        , "AIActionConfig.txt", "AIConfig.txt", "AISkillComboList.txt", "ArenaBaseConfig.txt", "MatchRuleConfig.txt", "ArenaPrizeConfig.txt", "ArenaBuyFightCountConfig.txt", "BuyEliteConfig.txt"
        , "BuyPartnerCombatConfig.txt", "CameraConfig.txt", "CorpsConfig.txt", "FightingScoreRankConfig.txt", "HitSoundConfig.txt", "EffectData.txt", "ImpactData.txt", "LoginLotteryConfig.txt"
        , "LoginLotteryItemConfig.txt", "LootBaseConfig.txt", "LootDomainConfig.txt", "LotteryActivityTimeConfig.txt", "LotteryPoolConfig.txt", "LotteryRuleConfig.txt", "MissionConfig.txt", "TaskConfig.txt"
        , "MonthCardConfig.txt", "AppendAttributeConfig.txt", "CorpsCharpterConfig.txt", "TitleContentConfig.txt", "BuyFightCountConfig.txt", "QueryCostConfig.txt", "ItemDrop.txt", "SystemGuideConfig.txt"
        , "SoundConfig.txt", "SensitiveWord.txt", "AwardItemConfig.txt", "PaymentInfo.txt", "AFKRewardConfig.txt", "MpvpBaseConfig.txt", "MpvpPrizeConfig.txt", "SceneFsparaConfig.txt"
          });
    }
}

