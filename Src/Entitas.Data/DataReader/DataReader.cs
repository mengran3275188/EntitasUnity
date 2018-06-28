//----------------------------------------------------------------------------
//！！！不要手动修改此文件，此文件由TableReaderGenerator按table.dsl生成！！！
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Util;

namespace Entitas.Data
{
	public sealed partial class ActionConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 16)]
		private struct ActionConfigRecord
		{
			internal int ModelId;
			internal int Description;
			internal int Stand;
			internal int Run;
		}

		public int ModelId;
		public string Description;
		public string Stand;
		public string Run;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			ModelId = DBCUtil.ExtractNumeric<int>(node, "ModelId", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Stand = DBCUtil.ExtractString(node, "Stand", "");
			Run = DBCUtil.ExtractString(node, "Run", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			ActionConfigRecord record = GetRecord(table,index);
			ModelId = DBCUtil.ExtractInt(table, record.ModelId, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Stand = DBCUtil.ExtractString(table, record.Stand, "");
			Run = DBCUtil.ExtractString(table, record.Run, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			ActionConfigRecord record = new ActionConfigRecord();
			record.ModelId = DBCUtil.SetValue(table, ModelId, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Stand = DBCUtil.SetValue(table, Stand, "");
			record.Run = DBCUtil.SetValue(table, Run, "");
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return ModelId;
		}

		private unsafe ActionConfigRecord GetRecord(BinaryTable table, int index)
		{
			ActionConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(ActionConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(ActionConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(ActionConfigRecord)];
			fixed (byte* p = bytes) {
				ActionConfigRecord* temp = (ActionConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class ActionConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_ActionConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_ActionConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_ActionConfigMgr.CollectDataFromBinary(file);
			} else {
				m_ActionConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_ActionConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_ActionConfigMgr.Clear();
		}

		public DataDictionaryMgr2<ActionConfig> ActionConfigMgr
		{
			get { return m_ActionConfigMgr; }
		}

		public int GetActionConfigCount()
		{
			return m_ActionConfigMgr.GetDataCount();
		}

		public ActionConfig GetActionConfig(int id)
		{
			return m_ActionConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<ActionConfig> m_ActionConfigMgr = new DataDictionaryMgr2<ActionConfig>();

		public static ActionConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static ActionConfigProvider s_Instance = new ActionConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class CharacterConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 24)]
		private struct CharacterConfigRecord
		{
			internal int Id;
			internal int Description;
			internal int Model;
			internal float Scale;
			internal int ActionId;
			internal int ActionPrefix;
		}

		public int Id;
		public string Description;
		public string Model;
		public float Scale;
		public int ActionId;
		public string ActionPrefix;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Model = DBCUtil.ExtractString(node, "Model", "");
			Scale = DBCUtil.ExtractNumeric<float>(node, "Scale", 0);
			ActionId = DBCUtil.ExtractNumeric<int>(node, "ActionId", 0);
			ActionPrefix = DBCUtil.ExtractString(node, "ActionPrefix", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			CharacterConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Model = DBCUtil.ExtractString(table, record.Model, "");
			Scale = DBCUtil.ExtractFloat(table, record.Scale, 0);
			ActionId = DBCUtil.ExtractInt(table, record.ActionId, 0);
			ActionPrefix = DBCUtil.ExtractString(table, record.ActionPrefix, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			CharacterConfigRecord record = new CharacterConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Model = DBCUtil.SetValue(table, Model, "");
			record.Scale = DBCUtil.SetValue(table, Scale, 0);
			record.ActionId = DBCUtil.SetValue(table, ActionId, 0);
			record.ActionPrefix = DBCUtil.SetValue(table, ActionPrefix, "");
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe CharacterConfigRecord GetRecord(BinaryTable table, int index)
		{
			CharacterConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(CharacterConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(CharacterConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(CharacterConfigRecord)];
			fixed (byte* p = bytes) {
				CharacterConfigRecord* temp = (CharacterConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class CharacterConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_CharacterConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_CharacterConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_CharacterConfigMgr.CollectDataFromBinary(file);
			} else {
				m_CharacterConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_CharacterConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_CharacterConfigMgr.Clear();
		}

		public DataDictionaryMgr2<CharacterConfig> CharacterConfigMgr
		{
			get { return m_CharacterConfigMgr; }
		}

		public int GetCharacterConfigCount()
		{
			return m_CharacterConfigMgr.GetDataCount();
		}

		public CharacterConfig GetCharacterConfig(int id)
		{
			return m_CharacterConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<CharacterConfig> m_CharacterConfigMgr = new DataDictionaryMgr2<CharacterConfig>();

		public static CharacterConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static CharacterConfigProvider s_Instance = new CharacterConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class SceneConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 16)]
		private struct SceneConfigRecord
		{
			internal int Id;
			internal int Description;
			internal int Script;
			internal int Navmesh;
		}

		public int Id;
		public string Description;
		public string Script;
		public string Navmesh;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Script = DBCUtil.ExtractString(node, "Script", "");
			Navmesh = DBCUtil.ExtractString(node, "Navmesh", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			SceneConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Script = DBCUtil.ExtractString(table, record.Script, "");
			Navmesh = DBCUtil.ExtractString(table, record.Navmesh, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			SceneConfigRecord record = new SceneConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Script = DBCUtil.SetValue(table, Script, "");
			record.Navmesh = DBCUtil.SetValue(table, Navmesh, "");
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe SceneConfigRecord GetRecord(BinaryTable table, int index)
		{
			SceneConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(SceneConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(SceneConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(SceneConfigRecord)];
			fixed (byte* p = bytes) {
				SceneConfigRecord* temp = (SceneConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class SceneConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_SceneConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_SceneConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_SceneConfigMgr.CollectDataFromBinary(file);
			} else {
				m_SceneConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_SceneConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_SceneConfigMgr.Clear();
		}

		public DataDictionaryMgr2<SceneConfig> SceneConfigMgr
		{
			get { return m_SceneConfigMgr; }
		}

		public int GetSceneConfigCount()
		{
			return m_SceneConfigMgr.GetDataCount();
		}

		public SceneConfig GetSceneConfig(int id)
		{
			return m_SceneConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<SceneConfig> m_SceneConfigMgr = new DataDictionaryMgr2<SceneConfig>();

		public static SceneConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static SceneConfigProvider s_Instance = new SceneConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class SkillConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 12)]
		private struct SkillConfigRecord
		{
			internal int Id;
			internal int Description;
			internal int Script;
		}

		public int Id;
		public string Description;
		public string Script;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Script = DBCUtil.ExtractString(node, "Script", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			SkillConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Script = DBCUtil.ExtractString(table, record.Script, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			SkillConfigRecord record = new SkillConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Script = DBCUtil.SetValue(table, Script, "");
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe SkillConfigRecord GetRecord(BinaryTable table, int index)
		{
			SkillConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(SkillConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(SkillConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(SkillConfigRecord)];
			fixed (byte* p = bytes) {
				SkillConfigRecord* temp = (SkillConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class SkillConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_SkillConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_SkillConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_SkillConfigMgr.CollectDataFromBinary(file);
			} else {
				m_SkillConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_SkillConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_SkillConfigMgr.Clear();
		}

		public DataDictionaryMgr2<SkillConfig> SkillConfigMgr
		{
			get { return m_SkillConfigMgr; }
		}

		public int GetSkillConfigCount()
		{
			return m_SkillConfigMgr.GetDataCount();
		}

		public SkillConfig GetSkillConfig(int id)
		{
			return m_SkillConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<SkillConfig> m_SkillConfigMgr = new DataDictionaryMgr2<SkillConfig>();

		public static SkillConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static SkillConfigProvider s_Instance = new SkillConfigProvider();
	}
}
