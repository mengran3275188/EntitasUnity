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
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 24)]
		private struct ActionConfigRecord
		{
			internal int ModelId;
			internal int Description;
			internal int Stand;
			internal int Run;
			internal int Dead;
			internal int Born;
		}

		public int ModelId;
		public string Description;
		public string Stand;
		public string Run;
		public string Dead;
		public string Born;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			ModelId = DBCUtil.ExtractNumeric<int>(node, "ModelId", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Stand = DBCUtil.ExtractString(node, "Stand", "");
			Run = DBCUtil.ExtractString(node, "Run", "");
			Dead = DBCUtil.ExtractString(node, "Dead", "");
			Born = DBCUtil.ExtractString(node, "Born", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			ActionConfigRecord record = GetRecord(table,index);
			ModelId = DBCUtil.ExtractInt(table, record.ModelId, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Stand = DBCUtil.ExtractString(table, record.Stand, "");
			Run = DBCUtil.ExtractString(table, record.Run, "");
			Dead = DBCUtil.ExtractString(table, record.Dead, "");
			Born = DBCUtil.ExtractString(table, record.Born, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			ActionConfigRecord record = new ActionConfigRecord();
			record.ModelId = DBCUtil.SetValue(table, ModelId, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Stand = DBCUtil.SetValue(table, Stand, "");
			record.Run = DBCUtil.SetValue(table, Run, "");
			record.Dead = DBCUtil.SetValue(table, Dead, "");
			record.Born = DBCUtil.SetValue(table, Born, "");
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
	public sealed partial class AttributeConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 112)]
		private struct AttributeConfigRecord
		{
			internal int Id;
			internal int Describe;
			internal int AddEnergyRecover;
			internal int AddEnergyMax;
			internal int AddMaxAd;
			internal int AddMinAd;
			internal int AddCritical;
			internal int AddCriticalFactor;
			internal int AddMetalFactor;
			internal int AddWoodFactor;
			internal int AddWaterFactor;
			internal int AddFireFactor;
			internal int AddEarthFactor;
			internal int AddFullDamageFactor;
			internal int AddHpMax;
			internal int AddArmor;
			internal int AddMiss;
			internal int AddMetalResist;
			internal int AddWoodResist;
			internal int AddWaterResist;
			internal int AddFireResist;
			internal int AddEarthResist;
			internal int AddAccuracyRecover;
			internal int AddDamageDerate;
			internal int AddMoveSpeed;
			internal int AddFullElementResist;
			internal int AddFullElementFactor;
			internal int AddHpFactor;
		}

		public int Id;
		public string Describe;
		public int AddEnergyRecover;
		public int AddEnergyMax;
		public int AddMaxAd;
		public int AddMinAd;
		public int AddCritical;
		public int AddCriticalFactor;
		public int AddMetalFactor;
		public int AddWoodFactor;
		public int AddWaterFactor;
		public int AddFireFactor;
		public int AddEarthFactor;
		public int AddFullDamageFactor;
		public int AddHpMax;
		public int AddArmor;
		public int AddMiss;
		public int AddMetalResist;
		public int AddWoodResist;
		public int AddWaterResist;
		public int AddFireResist;
		public int AddEarthResist;
		public int AddAccuracyRecover;
		public int AddDamageDerate;
		public int AddMoveSpeed;
		public int AddFullElementResist;
		public int AddFullElementFactor;
		public int AddHpFactor;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Describe = DBCUtil.ExtractString(node, "Describe", "");
			AddEnergyRecover = DBCUtil.ExtractNumeric<int>(node, "AddEnergyRecover", 0);
			AddEnergyMax = DBCUtil.ExtractNumeric<int>(node, "AddEnergyMax", 0);
			AddMaxAd = DBCUtil.ExtractNumeric<int>(node, "AddMaxAd", 0);
			AddMinAd = DBCUtil.ExtractNumeric<int>(node, "AddMinAd", 0);
			AddCritical = DBCUtil.ExtractNumeric<int>(node, "AddCritical", 0);
			AddCriticalFactor = DBCUtil.ExtractNumeric<int>(node, "AddCriticalFactor", 0);
			AddMetalFactor = DBCUtil.ExtractNumeric<int>(node, "AddMetalFactor", 0);
			AddWoodFactor = DBCUtil.ExtractNumeric<int>(node, "AddWoodFactor", 0);
			AddWaterFactor = DBCUtil.ExtractNumeric<int>(node, "AddWaterFactor", 0);
			AddFireFactor = DBCUtil.ExtractNumeric<int>(node, "AddFireFactor", 0);
			AddEarthFactor = DBCUtil.ExtractNumeric<int>(node, "AddEarthFactor", 0);
			AddFullDamageFactor = DBCUtil.ExtractNumeric<int>(node, "AddFullDamageFactor", 0);
			AddHpMax = DBCUtil.ExtractNumeric<int>(node, "AddHpMax", 0);
			AddArmor = DBCUtil.ExtractNumeric<int>(node, "AddArmor", 0);
			AddMiss = DBCUtil.ExtractNumeric<int>(node, "AddMiss", 0);
			AddMetalResist = DBCUtil.ExtractNumeric<int>(node, "AddMetalResist", 0);
			AddWoodResist = DBCUtil.ExtractNumeric<int>(node, "AddWoodResist", 0);
			AddWaterResist = DBCUtil.ExtractNumeric<int>(node, "AddWaterResist", 0);
			AddFireResist = DBCUtil.ExtractNumeric<int>(node, "AddFireResist", 0);
			AddEarthResist = DBCUtil.ExtractNumeric<int>(node, "AddEarthResist", 0);
			AddAccuracyRecover = DBCUtil.ExtractNumeric<int>(node, "AddAccuracyRecover", 0);
			AddDamageDerate = DBCUtil.ExtractNumeric<int>(node, "AddDamageDerate", 0);
			AddMoveSpeed = DBCUtil.ExtractNumeric<int>(node, "AddMoveSpeed", 0);
			AddFullElementResist = DBCUtil.ExtractNumeric<int>(node, "AddFullElementResist", 0);
			AddFullElementFactor = DBCUtil.ExtractNumeric<int>(node, "AddFullElementFactor", 0);
			AddHpFactor = DBCUtil.ExtractNumeric<int>(node, "AddHpFactor", 0);
			AfterCollectData();
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			AttributeConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Describe = DBCUtil.ExtractString(table, record.Describe, "");
			AddEnergyRecover = DBCUtil.ExtractInt(table, record.AddEnergyRecover, 0);
			AddEnergyMax = DBCUtil.ExtractInt(table, record.AddEnergyMax, 0);
			AddMaxAd = DBCUtil.ExtractInt(table, record.AddMaxAd, 0);
			AddMinAd = DBCUtil.ExtractInt(table, record.AddMinAd, 0);
			AddCritical = DBCUtil.ExtractInt(table, record.AddCritical, 0);
			AddCriticalFactor = DBCUtil.ExtractInt(table, record.AddCriticalFactor, 0);
			AddMetalFactor = DBCUtil.ExtractInt(table, record.AddMetalFactor, 0);
			AddWoodFactor = DBCUtil.ExtractInt(table, record.AddWoodFactor, 0);
			AddWaterFactor = DBCUtil.ExtractInt(table, record.AddWaterFactor, 0);
			AddFireFactor = DBCUtil.ExtractInt(table, record.AddFireFactor, 0);
			AddEarthFactor = DBCUtil.ExtractInt(table, record.AddEarthFactor, 0);
			AddFullDamageFactor = DBCUtil.ExtractInt(table, record.AddFullDamageFactor, 0);
			AddHpMax = DBCUtil.ExtractInt(table, record.AddHpMax, 0);
			AddArmor = DBCUtil.ExtractInt(table, record.AddArmor, 0);
			AddMiss = DBCUtil.ExtractInt(table, record.AddMiss, 0);
			AddMetalResist = DBCUtil.ExtractInt(table, record.AddMetalResist, 0);
			AddWoodResist = DBCUtil.ExtractInt(table, record.AddWoodResist, 0);
			AddWaterResist = DBCUtil.ExtractInt(table, record.AddWaterResist, 0);
			AddFireResist = DBCUtil.ExtractInt(table, record.AddFireResist, 0);
			AddEarthResist = DBCUtil.ExtractInt(table, record.AddEarthResist, 0);
			AddAccuracyRecover = DBCUtil.ExtractInt(table, record.AddAccuracyRecover, 0);
			AddDamageDerate = DBCUtil.ExtractInt(table, record.AddDamageDerate, 0);
			AddMoveSpeed = DBCUtil.ExtractInt(table, record.AddMoveSpeed, 0);
			AddFullElementResist = DBCUtil.ExtractInt(table, record.AddFullElementResist, 0);
			AddFullElementFactor = DBCUtil.ExtractInt(table, record.AddFullElementFactor, 0);
			AddHpFactor = DBCUtil.ExtractInt(table, record.AddHpFactor, 0);
			AfterCollectData();
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			AttributeConfigRecord record = new AttributeConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Describe = DBCUtil.SetValue(table, Describe, "");
			record.AddEnergyRecover = DBCUtil.SetValue(table, AddEnergyRecover, 0);
			record.AddEnergyMax = DBCUtil.SetValue(table, AddEnergyMax, 0);
			record.AddMaxAd = DBCUtil.SetValue(table, AddMaxAd, 0);
			record.AddMinAd = DBCUtil.SetValue(table, AddMinAd, 0);
			record.AddCritical = DBCUtil.SetValue(table, AddCritical, 0);
			record.AddCriticalFactor = DBCUtil.SetValue(table, AddCriticalFactor, 0);
			record.AddMetalFactor = DBCUtil.SetValue(table, AddMetalFactor, 0);
			record.AddWoodFactor = DBCUtil.SetValue(table, AddWoodFactor, 0);
			record.AddWaterFactor = DBCUtil.SetValue(table, AddWaterFactor, 0);
			record.AddFireFactor = DBCUtil.SetValue(table, AddFireFactor, 0);
			record.AddEarthFactor = DBCUtil.SetValue(table, AddEarthFactor, 0);
			record.AddFullDamageFactor = DBCUtil.SetValue(table, AddFullDamageFactor, 0);
			record.AddHpMax = DBCUtil.SetValue(table, AddHpMax, 0);
			record.AddArmor = DBCUtil.SetValue(table, AddArmor, 0);
			record.AddMiss = DBCUtil.SetValue(table, AddMiss, 0);
			record.AddMetalResist = DBCUtil.SetValue(table, AddMetalResist, 0);
			record.AddWoodResist = DBCUtil.SetValue(table, AddWoodResist, 0);
			record.AddWaterResist = DBCUtil.SetValue(table, AddWaterResist, 0);
			record.AddFireResist = DBCUtil.SetValue(table, AddFireResist, 0);
			record.AddEarthResist = DBCUtil.SetValue(table, AddEarthResist, 0);
			record.AddAccuracyRecover = DBCUtil.SetValue(table, AddAccuracyRecover, 0);
			record.AddDamageDerate = DBCUtil.SetValue(table, AddDamageDerate, 0);
			record.AddMoveSpeed = DBCUtil.SetValue(table, AddMoveSpeed, 0);
			record.AddFullElementResist = DBCUtil.SetValue(table, AddFullElementResist, 0);
			record.AddFullElementFactor = DBCUtil.SetValue(table, AddFullElementFactor, 0);
			record.AddHpFactor = DBCUtil.SetValue(table, AddHpFactor, 0);
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe AttributeConfigRecord GetRecord(BinaryTable table, int index)
		{
			AttributeConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(AttributeConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(AttributeConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(AttributeConfigRecord)];
			fixed (byte* p = bytes) {
				AttributeConfigRecord* temp = (AttributeConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class AttributeConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_AttributeConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_AttributeConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_AttributeConfigMgr.CollectDataFromBinary(file);
			} else {
				m_AttributeConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_AttributeConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_AttributeConfigMgr.Clear();
		}

		public DataDictionaryMgr2<AttributeConfig> AttributeConfigMgr
		{
			get { return m_AttributeConfigMgr; }
		}

		public int GetAttributeConfigCount()
		{
			return m_AttributeConfigMgr.GetDataCount();
		}

		public AttributeConfig GetAttributeConfig(int id)
		{
			return m_AttributeConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<AttributeConfig> m_AttributeConfigMgr = new DataDictionaryMgr2<AttributeConfig>();

		public static AttributeConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static AttributeConfigProvider s_Instance = new AttributeConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class Blocks : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 56)]
		private struct BlocksRecord
		{
			internal int Id;
			internal int Description;
			internal float Length;
			internal float Width;
			internal int Res;
			internal int Type;
			internal int LeftPortType;
			internal int LeftOffeset;
			internal int RightPortType;
			internal int RightOffset;
			internal int ForwardPortType;
			internal int ForwardOffset;
			internal int BackPortType;
			internal int BackOffset;
		}

		public int Id;
		public string Description;
		public float Length;
		public float Width;
		public string Res;
		public int Type;
		public int LeftPortType;
		public float[] LeftOffeset;
		public int RightPortType;
		public float[] RightOffset;
		public int ForwardPortType;
		public float[] ForwardOffset;
		public int BackPortType;
		public float[] BackOffset;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Length = DBCUtil.ExtractNumeric<float>(node, "Length", 0);
			Width = DBCUtil.ExtractNumeric<float>(node, "Width", 0);
			Res = DBCUtil.ExtractString(node, "Res", "");
			Type = DBCUtil.ExtractNumeric<int>(node, "Type", 0);
			LeftPortType = DBCUtil.ExtractNumeric<int>(node, "LeftPortType", 0);
			LeftOffeset = DBCUtil.ExtractNumericArray<float>(node, "LeftOffeset", null);
			RightPortType = DBCUtil.ExtractNumeric<int>(node, "RightPortType", 0);
			RightOffset = DBCUtil.ExtractNumericArray<float>(node, "RightOffset", null);
			ForwardPortType = DBCUtil.ExtractNumeric<int>(node, "ForwardPortType", 0);
			ForwardOffset = DBCUtil.ExtractNumericArray<float>(node, "ForwardOffset", null);
			BackPortType = DBCUtil.ExtractNumeric<int>(node, "BackPortType", 0);
			BackOffset = DBCUtil.ExtractNumericArray<float>(node, "BackOffset", null);
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			BlocksRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Length = DBCUtil.ExtractFloat(table, record.Length, 0);
			Width = DBCUtil.ExtractFloat(table, record.Width, 0);
			Res = DBCUtil.ExtractString(table, record.Res, "");
			Type = DBCUtil.ExtractInt(table, record.Type, 0);
			LeftPortType = DBCUtil.ExtractInt(table, record.LeftPortType, 0);
			LeftOffeset = DBCUtil.ExtractFloatArray(table, record.LeftOffeset, null);
			RightPortType = DBCUtil.ExtractInt(table, record.RightPortType, 0);
			RightOffset = DBCUtil.ExtractFloatArray(table, record.RightOffset, null);
			ForwardPortType = DBCUtil.ExtractInt(table, record.ForwardPortType, 0);
			ForwardOffset = DBCUtil.ExtractFloatArray(table, record.ForwardOffset, null);
			BackPortType = DBCUtil.ExtractInt(table, record.BackPortType, 0);
			BackOffset = DBCUtil.ExtractFloatArray(table, record.BackOffset, null);
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			BlocksRecord record = new BlocksRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Length = DBCUtil.SetValue(table, Length, 0);
			record.Width = DBCUtil.SetValue(table, Width, 0);
			record.Res = DBCUtil.SetValue(table, Res, "");
			record.Type = DBCUtil.SetValue(table, Type, 0);
			record.LeftPortType = DBCUtil.SetValue(table, LeftPortType, 0);
			record.LeftOffeset = DBCUtil.SetValue(table, LeftOffeset, null);
			record.RightPortType = DBCUtil.SetValue(table, RightPortType, 0);
			record.RightOffset = DBCUtil.SetValue(table, RightOffset, null);
			record.ForwardPortType = DBCUtil.SetValue(table, ForwardPortType, 0);
			record.ForwardOffset = DBCUtil.SetValue(table, ForwardOffset, null);
			record.BackPortType = DBCUtil.SetValue(table, BackPortType, 0);
			record.BackOffset = DBCUtil.SetValue(table, BackOffset, null);
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe BlocksRecord GetRecord(BinaryTable table, int index)
		{
			BlocksRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(BlocksRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(BlocksRecord record)
		{
			byte[] bytes = new byte[sizeof(BlocksRecord)];
			fixed (byte* p = bytes) {
				BlocksRecord* temp = (BlocksRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class BlocksProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_Blocks);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_Blocks);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_BlocksMgr.CollectDataFromBinary(file);
			} else {
				m_BlocksMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_BlocksMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_BlocksMgr.Clear();
		}

		public DataDictionaryMgr2<Blocks> BlocksMgr
		{
			get { return m_BlocksMgr; }
		}

		public int GetBlocksCount()
		{
			return m_BlocksMgr.GetDataCount();
		}

		public Blocks GetBlocks(int id)
		{
			return m_BlocksMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<Blocks> m_BlocksMgr = new DataDictionaryMgr2<Blocks>();

		public static BlocksProvider Instance
		{
			get { return s_Instance; }
		}
		private static BlocksProvider s_Instance = new BlocksProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class BuffConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 20)]
		private struct BuffConfigRecord
		{
			internal int Id;
			internal int Description;
			internal int Script;
			internal int AttrId;
			internal int MaxCount;
		}

		public int Id;
		public string Description;
		public string Script;
		public int AttrId;
		public int MaxCount;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Script = DBCUtil.ExtractString(node, "Script", "");
			AttrId = DBCUtil.ExtractNumeric<int>(node, "AttrId", 0);
			MaxCount = DBCUtil.ExtractNumeric<int>(node, "MaxCount", 0);
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			BuffConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Script = DBCUtil.ExtractString(table, record.Script, "");
			AttrId = DBCUtil.ExtractInt(table, record.AttrId, 0);
			MaxCount = DBCUtil.ExtractInt(table, record.MaxCount, 0);
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			BuffConfigRecord record = new BuffConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Script = DBCUtil.SetValue(table, Script, "");
			record.AttrId = DBCUtil.SetValue(table, AttrId, 0);
			record.MaxCount = DBCUtil.SetValue(table, MaxCount, 0);
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe BuffConfigRecord GetRecord(BinaryTable table, int index)
		{
			BuffConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(BuffConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(BuffConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(BuffConfigRecord)];
			fixed (byte* p = bytes) {
				BuffConfigRecord* temp = (BuffConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class BuffConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_BuffConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_BuffConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_BuffConfigMgr.CollectDataFromBinary(file);
			} else {
				m_BuffConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_BuffConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_BuffConfigMgr.Clear();
		}

		public DataDictionaryMgr2<BuffConfig> BuffConfigMgr
		{
			get { return m_BuffConfigMgr; }
		}

		public int GetBuffConfigCount()
		{
			return m_BuffConfigMgr.GetDataCount();
		}

		public BuffConfig GetBuffConfig(int id)
		{
			return m_BuffConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<BuffConfig> m_BuffConfigMgr = new DataDictionaryMgr2<BuffConfig>();

		public static BuffConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static BuffConfigProvider s_Instance = new BuffConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class CameraConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 20)]
		private struct CameraConfigRecord
		{
			internal int Id;
			internal int Description;
			internal float Pitch;
			internal float Yaw;
			internal float Distance;
		}

		public int Id;
		public string Description;
		public float Pitch;
		public float Yaw;
		public float Distance;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Pitch = DBCUtil.ExtractNumeric<float>(node, "Pitch", 0);
			Yaw = DBCUtil.ExtractNumeric<float>(node, "Yaw", 0);
			Distance = DBCUtil.ExtractNumeric<float>(node, "Distance", 0);
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			CameraConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Pitch = DBCUtil.ExtractFloat(table, record.Pitch, 0);
			Yaw = DBCUtil.ExtractFloat(table, record.Yaw, 0);
			Distance = DBCUtil.ExtractFloat(table, record.Distance, 0);
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			CameraConfigRecord record = new CameraConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Pitch = DBCUtil.SetValue(table, Pitch, 0);
			record.Yaw = DBCUtil.SetValue(table, Yaw, 0);
			record.Distance = DBCUtil.SetValue(table, Distance, 0);
			byte[] bytes = GetRecordBytes(record);
			table.Records.Add(bytes);
		}

		public int GetId()
		{
			return Id;
		}

		private unsafe CameraConfigRecord GetRecord(BinaryTable table, int index)
		{
			CameraConfigRecord record;
			byte[] bytes = table.Records[index];
			fixed (byte* p = bytes) {
				record = *(CameraConfigRecord*)p;
			}
			return record;
		}
		private static unsafe byte[] GetRecordBytes(CameraConfigRecord record)
		{
			byte[] bytes = new byte[sizeof(CameraConfigRecord)];
			fixed (byte* p = bytes) {
				CameraConfigRecord* temp = (CameraConfigRecord*)p;
				*temp = record;
			}
			return bytes;
		}
	}

	public sealed partial class CameraConfigProvider
	{
		public void LoadForClient()
		{
			Load(FilePathDefine_Client.C_CameraConfig);
		}
		public void LoadForServer()
		{
			Load(FilePathDefine_Server.C_CameraConfig);
		}
		public void Load(string file)
		{
			if (BinaryTable.IsValid(HomePath.Instance.GetAbsolutePath(file))) {
				m_CameraConfigMgr.CollectDataFromBinary(file);
			} else {
				m_CameraConfigMgr.CollectDataFromDBC(file);
			}
		}
		public void Save(string file)
		{
		#if DEBUG
			m_CameraConfigMgr.SaveToBinary(file);
		#endif
		}
		public void Clear()
		{
			m_CameraConfigMgr.Clear();
		}

		public DataDictionaryMgr2<CameraConfig> CameraConfigMgr
		{
			get { return m_CameraConfigMgr; }
		}

		public int GetCameraConfigCount()
		{
			return m_CameraConfigMgr.GetDataCount();
		}

		public CameraConfig GetCameraConfig(int id)
		{
			return m_CameraConfigMgr.GetDataById(id);
		}

		private DataDictionaryMgr2<CameraConfig> m_CameraConfigMgr = new DataDictionaryMgr2<CameraConfig>();

		public static CameraConfigProvider Instance
		{
			get { return s_Instance; }
		}
		private static CameraConfigProvider s_Instance = new CameraConfigProvider();
	}
}

namespace Entitas.Data
{
	public sealed partial class CharacterConfig : IData2
	{
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 36)]
		private struct CharacterConfigRecord
		{
			internal int Id;
			internal int Description;
			internal int Model;
			internal float Scale;
			internal int ActionId;
			internal int ActionPrefix;
			internal int AIScript;
			internal int AttrId;
			internal int Skills;
		}

		public int Id;
		public string Description;
		public string Model;
		public float Scale;
		public int ActionId;
		public string ActionPrefix;
		public string AIScript;
		public int AttrId;
		public int[] Skills;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Model = DBCUtil.ExtractString(node, "Model", "");
			Scale = DBCUtil.ExtractNumeric<float>(node, "Scale", 0);
			ActionId = DBCUtil.ExtractNumeric<int>(node, "ActionId", 0);
			ActionPrefix = DBCUtil.ExtractString(node, "ActionPrefix", "");
			AIScript = DBCUtil.ExtractString(node, "AIScript", "");
			AttrId = DBCUtil.ExtractNumeric<int>(node, "AttrId", 0);
			Skills = DBCUtil.ExtractNumericArray<int>(node, "Skills", null);
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
			AIScript = DBCUtil.ExtractString(table, record.AIScript, "");
			AttrId = DBCUtil.ExtractInt(table, record.AttrId, 0);
			Skills = DBCUtil.ExtractIntArray(table, record.Skills, null);
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
			record.AIScript = DBCUtil.SetValue(table, AIScript, "");
			record.AttrId = DBCUtil.SetValue(table, AttrId, 0);
			record.Skills = DBCUtil.SetValue(table, Skills, null);
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
		[StructLayout(LayoutKind.Auto, Pack = 1, Size = 20)]
		private struct SceneConfigRecord
		{
			internal int Id;
			internal int Description;
			internal int Name;
			internal int Script;
			internal int Navmesh;
		}

		public int Id;
		public string Description;
		public string Name;
		public string Script;
		public string Navmesh;

		public bool CollectDataFromDBC(DBC_Row node)
		{
			Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0);
			Description = DBCUtil.ExtractString(node, "Description", "");
			Name = DBCUtil.ExtractString(node, "Name", "");
			Script = DBCUtil.ExtractString(node, "Script", "");
			Navmesh = DBCUtil.ExtractString(node, "Navmesh", "");
			return true;
		}

		public bool CollectDataFromBinary(BinaryTable table, int index)
		{
			SceneConfigRecord record = GetRecord(table,index);
			Id = DBCUtil.ExtractInt(table, record.Id, 0);
			Description = DBCUtil.ExtractString(table, record.Description, "");
			Name = DBCUtil.ExtractString(table, record.Name, "");
			Script = DBCUtil.ExtractString(table, record.Script, "");
			Navmesh = DBCUtil.ExtractString(table, record.Navmesh, "");
			return true;
		}

		public void AddToBinary(BinaryTable table)
		{
			SceneConfigRecord record = new SceneConfigRecord();
			record.Id = DBCUtil.SetValue(table, Id, 0);
			record.Description = DBCUtil.SetValue(table, Description, "");
			record.Name = DBCUtil.SetValue(table, Name, "");
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
