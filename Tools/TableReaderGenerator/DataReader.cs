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
	public sealed partial class NpcConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 24)]
		private struct NpcConfigRecord
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
			NpcConfigRecord record = GetRecord(table,index);
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
			NpcConfigRecord record = new NpcConfigRecord();
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

		private unsafe NpcConfigRecord GetRecord(BinaryTable table, int index)
		{
			NpcConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(NpcConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(NpcConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(NpcConfigRecord)];
			fixed (byte* p = bytes) {
				NpcConfigRecord* temp = (NpcConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class NpcConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_NpcConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_NpcConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_NpcConfigMgr.CollectDataFromBinary(file);
			} else {
				m_NpcConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_NpcConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_NpcConfigMgr.Clear();
		}

		public DataDictionaryMgr2<NpcConfig> NpcConfigMgr
		{
			get { return m_NpcConfigMgr; }
		}

		public int GetNpcConfigCount()
		{
			return m_NpcConfigMgr.GetDataCount();
		}

		public NpcConfig GetNpcConfig(int id)
		{
			return m_NpcConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<NpcConfig> m_NpcConfigMgr = new DataDictionaryMgr2<NpcConfig>();

		public static NpcConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static NpcConfigProvider s_Instance = new NpcConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class PlayerConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 16)]
		private struct PlayerConfigRecord
		{
			internal int Id;
			internal int Model;
			internal int ActionId;
			internal int ActionPrefix;
		}

		public int Id;
		public string Model;
		public int ActionId;
		public string ActionPrefix;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Model = DBCUtil.ExtractString(node, "Model", "");
			ActionId = DBCUtil.ExtractNumeric<int>(node, "ActionId", 0);
			ActionPrefix = DBCUtil.ExtractString(node, "ActionPrefix", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			PlayerConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Model = DBCUtil.ExtractString(table, record.Model, "");
			ActionId = DBCUtil.ExtractInt(table, record.ActionId, 0);
			ActionPrefix = DBCUtil.ExtractString(table, record.ActionPrefix, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			PlayerConfigRecord record = new PlayerConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Model = DBCUtil.SetValue(table, Model, "");
			record.ActionId = DBCUtil.SetValue(table, ActionId, 0);
			record.ActionPrefix = DBCUtil.SetValue(table, ActionPrefix, "");
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe PlayerConfigRecord GetRecord(BinaryTable table, int index)
		{
			PlayerConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(PlayerConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(PlayerConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(PlayerConfigRecord)];
			fixed (byte* p = bytes) {
				PlayerConfigRecord* temp = (PlayerConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class PlayerConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_PlayerConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_PlayerConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_PlayerConfigMgr.CollectDataFromBinary(file);
			} else {
				m_PlayerConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_PlayerConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_PlayerConfigMgr.Clear();
		}

		public DataDictionaryMgr2<PlayerConfig> PlayerConfigMgr
		{
			get { return m_PlayerConfigMgr; }
		}

		public int GetPlayerConfigCount()
		{
			return m_PlayerConfigMgr.GetDataCount();
		}

		public PlayerConfig GetPlayerConfig(int id)
		{
			return m_PlayerConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<PlayerConfig> m_PlayerConfigMgr = new DataDictionaryMgr2<PlayerConfig>();

		public static PlayerConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static PlayerConfigProvider s_Instance = new PlayerConfigProvider();
	}
}
