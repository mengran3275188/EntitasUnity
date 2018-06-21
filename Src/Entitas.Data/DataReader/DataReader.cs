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
	public sealed partial class PlayerConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 8)]
		private struct PlayerConfigRecord
		{
			internal int Id;
			internal int Model;
		}

		public int Id;
		public string Model;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Model = DBCUtil.ExtractString(node, "Model", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			PlayerConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Model = DBCUtil.ExtractString(table, record.Model, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			PlayerConfigRecord record = new PlayerConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Model = DBCUtil.SetValue(table, Model, "");
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
