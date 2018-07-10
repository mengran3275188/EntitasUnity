//----------------------------------------------------------------------------
//！！！不要手动修改此文件，此文件由LogicDataGenerator按AttributeConfig.txt生成！！！
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Entitas.Data
{
	public sealed class AttributeData
	{
		//----------------------------------------------------------------------------
		//基础属性以及读写接口
		//----------------------------------------------------------------------------
		public void SetEnergyRecover(Operate_Type opType, int tVal)
		{
			m_EnergyRecover = (int)UpdateAttr(m_EnergyRecover, m_EnergyRecover, opType, tVal);
		}
		public int EnergyRecover
		{
			get { return m_EnergyRecover / s_Key; }
		}
		private int m_EnergyRecover;

		public void SetEnergyMax(Operate_Type opType, int tVal)
		{
			m_EnergyMax = (int)UpdateAttr(m_EnergyMax, m_EnergyMax, opType, tVal);
		}
		public int EnergyMax
		{
			get { return m_EnergyMax / s_Key; }
		}
		private int m_EnergyMax;

		public void SetMaxAd(Operate_Type opType, int tVal)
		{
			m_MaxAd = (int)UpdateAttr(m_MaxAd, m_MaxAd, opType, tVal);
		}
		public int MaxAd
		{
			get { return m_MaxAd / s_Key; }
		}
		private int m_MaxAd;

		public void SetMinAd(Operate_Type opType, int tVal)
		{
			m_MinAd = (int)UpdateAttr(m_MinAd, m_MinAd, opType, tVal);
		}
		public int MinAd
		{
			get { return m_MinAd / s_Key; }
		}
		private int m_MinAd;

		public void SetCritical(Operate_Type opType, int tVal)
		{
			m_Critical = (int)UpdateAttr(m_Critical, m_Critical, opType, tVal);
		}
		public int Critical
		{
			get { return m_Critical / s_Key; }
		}
		private int m_Critical;

		public void SetCriticalFactor(Operate_Type opType, int tVal)
		{
			m_CriticalFactor = (int)UpdateAttr(m_CriticalFactor, m_CriticalFactor, opType, tVal);
		}
		public int CriticalFactor
		{
			get { return m_CriticalFactor / s_Key; }
		}
		private int m_CriticalFactor;

		public void SetMetalFactor(Operate_Type opType, int tVal)
		{
			m_MetalFactor = (int)UpdateAttr(m_MetalFactor, m_MetalFactor, opType, tVal);
		}
		public int MetalFactor
		{
			get { return m_MetalFactor / s_Key; }
		}
		private int m_MetalFactor;

		public void SetWoodFactor(Operate_Type opType, int tVal)
		{
			m_WoodFactor = (int)UpdateAttr(m_WoodFactor, m_WoodFactor, opType, tVal);
		}
		public int WoodFactor
		{
			get { return m_WoodFactor / s_Key; }
		}
		private int m_WoodFactor;

		public void SetWaterFactor(Operate_Type opType, int tVal)
		{
			m_WaterFactor = (int)UpdateAttr(m_WaterFactor, m_WaterFactor, opType, tVal);
		}
		public int WaterFactor
		{
			get { return m_WaterFactor / s_Key; }
		}
		private int m_WaterFactor;

		public void SetFireFactor(Operate_Type opType, int tVal)
		{
			m_FireFactor = (int)UpdateAttr(m_FireFactor, m_FireFactor, opType, tVal);
		}
		public int FireFactor
		{
			get { return m_FireFactor / s_Key; }
		}
		private int m_FireFactor;

		public void SetEarthFactor(Operate_Type opType, int tVal)
		{
			m_EarthFactor = (int)UpdateAttr(m_EarthFactor, m_EarthFactor, opType, tVal);
		}
		public int EarthFactor
		{
			get { return m_EarthFactor / s_Key; }
		}
		private int m_EarthFactor;

		public void SetFullDamageFactor(Operate_Type opType, int tVal)
		{
			m_FullDamageFactor = (int)UpdateAttr(m_FullDamageFactor, m_FullDamageFactor, opType, tVal);
		}
		public int FullDamageFactor
		{
			get { return m_FullDamageFactor / s_Key; }
		}
		private int m_FullDamageFactor;

		public void SetHpMax(Operate_Type opType, int tVal)
		{
			m_HpMax = (int)UpdateAttr(m_HpMax, m_HpMax, opType, tVal);
		}
		public int HpMax
		{
			get { return m_HpMax / s_Key; }
		}
		private int m_HpMax;

		public void SetArmor(Operate_Type opType, int tVal)
		{
			m_Armor = (int)UpdateAttr(m_Armor, m_Armor, opType, tVal);
		}
		public int Armor
		{
			get { return m_Armor / s_Key; }
		}
		private int m_Armor;

		public void SetMiss(Operate_Type opType, int tVal)
		{
			m_Miss = (int)UpdateAttr(m_Miss, m_Miss, opType, tVal);
		}
		public int Miss
		{
			get { return m_Miss / s_Key; }
		}
		private int m_Miss;

		public void SetMetalResist(Operate_Type opType, int tVal)
		{
			m_MetalResist = (int)UpdateAttr(m_MetalResist, m_MetalResist, opType, tVal);
		}
		public int MetalResist
		{
			get { return m_MetalResist / s_Key; }
		}
		private int m_MetalResist;

		public void SetWoodResist(Operate_Type opType, int tVal)
		{
			m_WoodResist = (int)UpdateAttr(m_WoodResist, m_WoodResist, opType, tVal);
		}
		public int WoodResist
		{
			get { return m_WoodResist / s_Key; }
		}
		private int m_WoodResist;

		public void SetWaterResist(Operate_Type opType, int tVal)
		{
			m_WaterResist = (int)UpdateAttr(m_WaterResist, m_WaterResist, opType, tVal);
		}
		public int WaterResist
		{
			get { return m_WaterResist / s_Key; }
		}
		private int m_WaterResist;

		public void SetFireResist(Operate_Type opType, int tVal)
		{
			m_FireResist = (int)UpdateAttr(m_FireResist, m_FireResist, opType, tVal);
		}
		public int FireResist
		{
			get { return m_FireResist / s_Key; }
		}
		private int m_FireResist;

		public void SetEarthResist(Operate_Type opType, int tVal)
		{
			m_EarthResist = (int)UpdateAttr(m_EarthResist, m_EarthResist, opType, tVal);
		}
		public int EarthResist
		{
			get { return m_EarthResist / s_Key; }
		}
		private int m_EarthResist;

		public void SetAccuracyRecover(Operate_Type opType, int tVal)
		{
			m_AccuracyRecover = (int)UpdateAttr(m_AccuracyRecover, m_AccuracyRecover, opType, tVal);
		}
		public int AccuracyRecover
		{
			get { return m_AccuracyRecover / s_Key; }
		}
		private int m_AccuracyRecover;

		public void SetDamageDerate(Operate_Type opType, int tVal)
		{
			m_DamageDerate = (int)UpdateAttr(m_DamageDerate, m_DamageDerate, opType, tVal);
		}
		public int DamageDerate
		{
			get { return m_DamageDerate / s_Key; }
		}
		private int m_DamageDerate;

		public void SetMoveSpeed(Operate_Type opType, int tVal)
		{
			m_MoveSpeed = (int)UpdateAttr(m_MoveSpeed, m_MoveSpeed, opType, tVal);
		}
		public int MoveSpeed
		{
			get { return m_MoveSpeed / s_Key; }
		}
		private int m_MoveSpeed;

		public void SetFullElementResist(Operate_Type opType, int tVal)
		{
			m_FullElementResist = (int)UpdateAttr(m_FullElementResist, m_FullElementResist, opType, tVal);
		}
		public int FullElementResist
		{
			get { return m_FullElementResist / s_Key; }
		}
		private int m_FullElementResist;

		public void SetFullElementFactor(Operate_Type opType, int tVal)
		{
			m_FullElementFactor = (int)UpdateAttr(m_FullElementFactor, m_FullElementFactor, opType, tVal);
		}
		public int FullElementFactor
		{
			get { return m_FullElementFactor / s_Key; }
		}
		private int m_FullElementFactor;

		public void SetHpFactor(Operate_Type opType, int tVal)
		{
			m_HpFactor = (int)UpdateAttr(m_HpFactor, m_HpFactor, opType, tVal);
		}
		public int HpFactor
		{
			get { return m_HpFactor / s_Key; }
		}
		private int m_HpFactor;

		//------------------------------------------------------------------------
		//指定属性枚举获取属性值，全部转化为float类型,返回值需要自行转换类型
		//------------------------------------------------------------------------
		public float GetAttributeByType(AttrbuteEnum attrType)
		{
			float attValue = 0;
			switch (attrType) {
				case AttrbuteEnum.Actual_EnergyRecover:
					attValue = EnergyRecover;
					break;
				case AttrbuteEnum.Actual_EnergyMax:
					attValue = EnergyMax;
					break;
				case AttrbuteEnum.Actual_MaxAd:
					attValue = MaxAd;
					break;
				case AttrbuteEnum.Actual_MinAd:
					attValue = MinAd;
					break;
				case AttrbuteEnum.Actual_Critical:
					attValue = Critical;
					break;
				case AttrbuteEnum.Actual_CriticalFactor:
					attValue = CriticalFactor;
					break;
				case AttrbuteEnum.Actual_MetalFactor:
					attValue = MetalFactor;
					break;
				case AttrbuteEnum.Actual_WoodFactor:
					attValue = WoodFactor;
					break;
				case AttrbuteEnum.Actual_WaterFactor:
					attValue = WaterFactor;
					break;
				case AttrbuteEnum.Actual_FireFactor:
					attValue = FireFactor;
					break;
				case AttrbuteEnum.Actual_EarthFactor:
					attValue = EarthFactor;
					break;
				case AttrbuteEnum.Actual_FullDamageFactor:
					attValue = FullDamageFactor;
					break;
				case AttrbuteEnum.Actual_HpMax:
					attValue = HpMax;
					break;
				case AttrbuteEnum.Actual_Armor:
					attValue = Armor;
					break;
				case AttrbuteEnum.Actual_Miss:
					attValue = Miss;
					break;
				case AttrbuteEnum.Actual_MetalResist:
					attValue = MetalResist;
					break;
				case AttrbuteEnum.Actual_WoodResist:
					attValue = WoodResist;
					break;
				case AttrbuteEnum.Actual_WaterResist:
					attValue = WaterResist;
					break;
				case AttrbuteEnum.Actual_FireResist:
					attValue = FireResist;
					break;
				case AttrbuteEnum.Actual_EarthResist:
					attValue = EarthResist;
					break;
				case AttrbuteEnum.Actual_AccuracyRecover:
					attValue = AccuracyRecover;
					break;
				case AttrbuteEnum.Actual_DamageDerate:
					attValue = DamageDerate;
					break;
				case AttrbuteEnum.Actual_MoveSpeed:
					attValue = MoveSpeed;
					break;
				case AttrbuteEnum.Actual_FullElementResist:
					attValue = FullElementResist;
					break;
				case AttrbuteEnum.Actual_FullElementFactor:
					attValue = FullElementFactor;
					break;
				case AttrbuteEnum.Actual_HpFactor:
					attValue = HpFactor;
					break;
				default:
					attValue = -1;
					break;
			}
			return attValue;
		}
		//------------------------------------------------------------------------
		//指定属性枚举值设置属性值,属性值参数为float类型，内部根据属性类型自行转换
		//------------------------------------------------------------------------
		public void SetAttributeByType(AttrbuteEnum attrType, Operate_Type opType, float tVal)
		{
			switch (attrType) {
				case AttrbuteEnum.Actual_EnergyRecover:
					SetEnergyRecover(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_EnergyMax:
					SetEnergyMax(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_MaxAd:
					SetMaxAd(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_MinAd:
					SetMinAd(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_Critical:
					SetCritical(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_CriticalFactor:
					SetCriticalFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_MetalFactor:
					SetMetalFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_WoodFactor:
					SetWoodFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_WaterFactor:
					SetWaterFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_FireFactor:
					SetFireFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_EarthFactor:
					SetEarthFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_FullDamageFactor:
					SetFullDamageFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_HpMax:
					SetHpMax(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_Armor:
					SetArmor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_Miss:
					SetMiss(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_MetalResist:
					SetMetalResist(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_WoodResist:
					SetWoodResist(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_WaterResist:
					SetWaterResist(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_FireResist:
					SetFireResist(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_EarthResist:
					SetEarthResist(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_AccuracyRecover:
					SetAccuracyRecover(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_DamageDerate:
					SetDamageDerate(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_MoveSpeed:
					SetMoveSpeed(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_FullElementResist:
					SetFullElementResist(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_FullElementFactor:
					SetFullElementFactor(opType, (int)tVal);
					break;
				case AttrbuteEnum.Actual_HpFactor:
					SetHpFactor(opType, (int)tVal);
					break;
			default:
				break;
			}
		}
		//------------------------------------------------------------------------
		//属性修改接口
		//------------------------------------------------------------------------
		public static float UpdateAttr(float val, float maxVal, Operate_Type opType, float tVal)
		{
			float ret = val;
			if (opType == Operate_Type.OT_PercentMax) {
				float t = maxVal * (tVal / 100.0f);
				ret = t;
			} else {
				ret = UpdateAttr(val, opType, tVal);
			}
			return ret;
		}

		public static float UpdateAttr(float val, Operate_Type opType, float tVal)
		{
			float ret = val;
			if (opType == Operate_Type.OT_Absolute) {
				ret = tVal * s_Key;
			} else if (opType == Operate_Type.OT_Relative) {
				float t = (ret + tVal * s_Key);
				if (t < 0) {
					t = 0;
				}
				ret = t;
			} else if (opType == Operate_Type.OT_PercentCurrent) {
				float t = (ret * (tVal / 100.0f));
				ret = t;
			}
			return ret;
		}
		//------------------------------------------------------------------------
		//注意：Key的修改应该在所有对象创建前执行，否则属性会乱！！！
		//------------------------------------------------------------------------
		public static int Key
		{
			get { return s_Key; }
			set { s_Key = value; }
		}
		private static int s_Key = 1;

	}
}
