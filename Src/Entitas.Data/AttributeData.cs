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
		public void SetEnergyRecover(Operate_Type opType, float tVal)
		{
			m_EnergyRecover = UpdateAttr(m_EnergyRecover, m_EnergyRecover, opType, tVal);
		}
		public float EnergyRecover
		{
			get { return m_EnergyRecover / s_Key; }
		}
		private float m_EnergyRecover;

		public void SetEnergyMax(Operate_Type opType, float tVal)
		{
			m_EnergyMax = UpdateAttr(m_EnergyMax, m_EnergyMax, opType, tVal);
		}
		public float EnergyMax
		{
			get { return m_EnergyMax / s_Key; }
		}
		private float m_EnergyMax;

		public void SetMaxAd(Operate_Type opType, float tVal)
		{
			m_MaxAd = UpdateAttr(m_MaxAd, m_MaxAd, opType, tVal);
		}
		public float MaxAd
		{
			get { return m_MaxAd / s_Key; }
		}
		private float m_MaxAd;

		public void SetMinAd(Operate_Type opType, float tVal)
		{
			m_MinAd = UpdateAttr(m_MinAd, m_MinAd, opType, tVal);
		}
		public float MinAd
		{
			get { return m_MinAd / s_Key; }
		}
		private float m_MinAd;

		public void SetCritical(Operate_Type opType, float tVal)
		{
			m_Critical = UpdateAttr(m_Critical, m_Critical, opType, tVal);
		}
		public float Critical
		{
			get { return m_Critical / s_Key; }
		}
		private float m_Critical;

		public void SetCriticalFactor(Operate_Type opType, float tVal)
		{
			m_CriticalFactor = UpdateAttr(m_CriticalFactor, m_CriticalFactor, opType, tVal);
		}
		public float CriticalFactor
		{
			get { return m_CriticalFactor / s_Key; }
		}
		private float m_CriticalFactor;

		public void SetMetalFactor(Operate_Type opType, float tVal)
		{
			m_MetalFactor = UpdateAttr(m_MetalFactor, m_MetalFactor, opType, tVal);
		}
		public float MetalFactor
		{
			get { return m_MetalFactor / s_Key; }
		}
		private float m_MetalFactor;

		public void SetWoodFactor(Operate_Type opType, float tVal)
		{
			m_WoodFactor = UpdateAttr(m_WoodFactor, m_WoodFactor, opType, tVal);
		}
		public float WoodFactor
		{
			get { return m_WoodFactor / s_Key; }
		}
		private float m_WoodFactor;

		public void SetWaterFactor(Operate_Type opType, float tVal)
		{
			m_WaterFactor = UpdateAttr(m_WaterFactor, m_WaterFactor, opType, tVal);
		}
		public float WaterFactor
		{
			get { return m_WaterFactor / s_Key; }
		}
		private float m_WaterFactor;

		public void SetFireFactor(Operate_Type opType, float tVal)
		{
			m_FireFactor = UpdateAttr(m_FireFactor, m_FireFactor, opType, tVal);
		}
		public float FireFactor
		{
			get { return m_FireFactor / s_Key; }
		}
		private float m_FireFactor;

		public void SetEarthFactor(Operate_Type opType, float tVal)
		{
			m_EarthFactor = UpdateAttr(m_EarthFactor, m_EarthFactor, opType, tVal);
		}
		public float EarthFactor
		{
			get { return m_EarthFactor / s_Key; }
		}
		private float m_EarthFactor;

		public void SetFullDamageFactor(Operate_Type opType, float tVal)
		{
			m_FullDamageFactor = UpdateAttr(m_FullDamageFactor, m_FullDamageFactor, opType, tVal);
		}
		public float FullDamageFactor
		{
			get { return m_FullDamageFactor / s_Key; }
		}
		private float m_FullDamageFactor;

		public void SetHpMax(Operate_Type opType, float tVal)
		{
			m_HpMax = UpdateAttr(m_HpMax, m_HpMax, opType, tVal);
		}
		public float HpMax
		{
			get { return m_HpMax / s_Key; }
		}
		private float m_HpMax;

		public void SetArmor(Operate_Type opType, float tVal)
		{
			m_Armor = UpdateAttr(m_Armor, m_Armor, opType, tVal);
		}
		public float Armor
		{
			get { return m_Armor / s_Key; }
		}
		private float m_Armor;

		public void SetMiss(Operate_Type opType, float tVal)
		{
			m_Miss = UpdateAttr(m_Miss, m_Miss, opType, tVal);
		}
		public float Miss
		{
			get { return m_Miss / s_Key; }
		}
		private float m_Miss;

		public void SetMetalResist(Operate_Type opType, float tVal)
		{
			m_MetalResist = UpdateAttr(m_MetalResist, m_MetalResist, opType, tVal);
		}
		public float MetalResist
		{
			get { return m_MetalResist / s_Key; }
		}
		private float m_MetalResist;

		public void SetWoodResist(Operate_Type opType, float tVal)
		{
			m_WoodResist = UpdateAttr(m_WoodResist, m_WoodResist, opType, tVal);
		}
		public float WoodResist
		{
			get { return m_WoodResist / s_Key; }
		}
		private float m_WoodResist;

		public void SetWaterResist(Operate_Type opType, float tVal)
		{
			m_WaterResist = UpdateAttr(m_WaterResist, m_WaterResist, opType, tVal);
		}
		public float WaterResist
		{
			get { return m_WaterResist / s_Key; }
		}
		private float m_WaterResist;

		public void SetFireResist(Operate_Type opType, float tVal)
		{
			m_FireResist = UpdateAttr(m_FireResist, m_FireResist, opType, tVal);
		}
		public float FireResist
		{
			get { return m_FireResist / s_Key; }
		}
		private float m_FireResist;

		public void SetEarthResist(Operate_Type opType, float tVal)
		{
			m_EarthResist = UpdateAttr(m_EarthResist, m_EarthResist, opType, tVal);
		}
		public float EarthResist
		{
			get { return m_EarthResist / s_Key; }
		}
		private float m_EarthResist;

		public void SetAccuracyRecover(Operate_Type opType, float tVal)
		{
			m_AccuracyRecover = UpdateAttr(m_AccuracyRecover, m_AccuracyRecover, opType, tVal);
		}
		public float AccuracyRecover
		{
			get { return m_AccuracyRecover / s_Key; }
		}
		private float m_AccuracyRecover;

		public void SetDamageDerate(Operate_Type opType, float tVal)
		{
			m_DamageDerate = UpdateAttr(m_DamageDerate, m_DamageDerate, opType, tVal);
		}
		public float DamageDerate
		{
			get { return m_DamageDerate / s_Key; }
		}
		private float m_DamageDerate;

		public void SetMoveSpeed(Operate_Type opType, float tVal)
		{
			m_MoveSpeed = UpdateAttr(m_MoveSpeed, m_MoveSpeed, opType, tVal);
		}
		public float MoveSpeed
		{
			get { return m_MoveSpeed / s_Key; }
		}
		private float m_MoveSpeed;

		public void SetFullElementResist(Operate_Type opType, float tVal)
		{
			m_FullElementResist = UpdateAttr(m_FullElementResist, m_FullElementResist, opType, tVal);
		}
		public float FullElementResist
		{
			get { return m_FullElementResist / s_Key; }
		}
		private float m_FullElementResist;

		public void SetFullElementFactor(Operate_Type opType, float tVal)
		{
			m_FullElementFactor = UpdateAttr(m_FullElementFactor, m_FullElementFactor, opType, tVal);
		}
		public float FullElementFactor
		{
			get { return m_FullElementFactor / s_Key; }
		}
		private float m_FullElementFactor;

		public void SetHpFactor(Operate_Type opType, float tVal)
		{
			m_HpFactor = UpdateAttr(m_HpFactor, m_HpFactor, opType, tVal);
		}
		public float HpFactor
		{
			get { return m_HpFactor / s_Key; }
		}
		private float m_HpFactor;

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
		// 属性初始化接口
		//------------------------------------------------------------------------
		public void SetAbsoluteByConfig(AttributeConfig attr)
		{
			float aEnergyRecover = attr.GetEnergyRecover(0, 0);
			float aEnergyMax = attr.GetEnergyMax(0, 0);
			float aMaxAd = attr.GetMaxAd(0, 0);
			float aMinAd = attr.GetMinAd(0, 0);
			float aCritical = attr.GetCritical(0, 0);
			float aCriticalFactor = attr.GetCriticalFactor(0, 0);
			float aMetalFactor = attr.GetMetalFactor(0, 0);
			float aWoodFactor = attr.GetWoodFactor(0, 0);
			float aWaterFactor = attr.GetWaterFactor(0, 0);
			float aFireFactor = attr.GetFireFactor(0, 0);
			float aEarthFactor = attr.GetEarthFactor(0, 0);
			float aFullDamageFactor = attr.GetFullDamageFactor(0, 0);
			float aHpMax = attr.GetHpMax(0, 0);
			float aArmor = attr.GetArmor(0, 0);
			float aMiss = attr.GetMiss(0, 0);
			float aMetalResist = attr.GetMetalResist(0, 0);
			float aWoodResist = attr.GetWoodResist(0, 0);
			float aWaterResist = attr.GetWaterResist(0, 0);
			float aFireResist = attr.GetFireResist(0, 0);
			float aEarthResist = attr.GetEarthResist(0, 0);
			float aAccuracyRecover = attr.GetAccuracyRecover(0, 0);
			float aDamageDerate = attr.GetDamageDerate(0, 0);
			float aMoveSpeed = attr.GetMoveSpeed(0, 0);
			float aFullElementResist = attr.GetFullElementResist(0, 0);
			float aFullElementFactor = attr.GetFullElementFactor(0, 0);
			float aHpFactor = attr.GetHpFactor(0, 0);
			SetEnergyRecover(Operate_Type.OT_Absolute, aEnergyRecover);
			SetEnergyMax(Operate_Type.OT_Absolute, aEnergyMax);
			SetMaxAd(Operate_Type.OT_Absolute, aMaxAd);
			SetMinAd(Operate_Type.OT_Absolute, aMinAd);
			SetCritical(Operate_Type.OT_Absolute, aCritical);
			SetCriticalFactor(Operate_Type.OT_Absolute, aCriticalFactor);
			SetMetalFactor(Operate_Type.OT_Absolute, aMetalFactor);
			SetWoodFactor(Operate_Type.OT_Absolute, aWoodFactor);
			SetWaterFactor(Operate_Type.OT_Absolute, aWaterFactor);
			SetFireFactor(Operate_Type.OT_Absolute, aFireFactor);
			SetEarthFactor(Operate_Type.OT_Absolute, aEarthFactor);
			SetFullDamageFactor(Operate_Type.OT_Absolute, aFullDamageFactor);
			SetHpMax(Operate_Type.OT_Absolute, aHpMax);
			SetArmor(Operate_Type.OT_Absolute, aArmor);
			SetMiss(Operate_Type.OT_Absolute, aMiss);
			SetMetalResist(Operate_Type.OT_Absolute, aMetalResist);
			SetWoodResist(Operate_Type.OT_Absolute, aWoodResist);
			SetWaterResist(Operate_Type.OT_Absolute, aWaterResist);
			SetFireResist(Operate_Type.OT_Absolute, aFireResist);
			SetEarthResist(Operate_Type.OT_Absolute, aEarthResist);
			SetAccuracyRecover(Operate_Type.OT_Absolute, aAccuracyRecover);
			SetDamageDerate(Operate_Type.OT_Absolute, aDamageDerate);
			SetMoveSpeed(Operate_Type.OT_Absolute, aMoveSpeed);
			SetFullElementResist(Operate_Type.OT_Absolute, aFullElementResist);
			SetFullElementFactor(Operate_Type.OT_Absolute, aFullElementFactor);
			SetHpFactor(Operate_Type.OT_Absolute, aHpFactor);
		}
		public void SetRelativeByConfig(AttributeConfig attr)
		{
			SetEnergyRecover(Operate_Type.OT_Relative, attr.GetEnergyRecover(EnergyRecover, 0));
			SetEnergyMax(Operate_Type.OT_Relative, attr.GetEnergyMax(EnergyMax, 0));
			SetMaxAd(Operate_Type.OT_Relative, attr.GetMaxAd(MaxAd, 0));
			SetMinAd(Operate_Type.OT_Relative, attr.GetMinAd(MinAd, 0));
			SetCritical(Operate_Type.OT_Relative, attr.GetCritical(Critical, 0));
			SetCriticalFactor(Operate_Type.OT_Relative, attr.GetCriticalFactor(CriticalFactor, 0));
			SetMetalFactor(Operate_Type.OT_Relative, attr.GetMetalFactor(MetalFactor, 0));
			SetWoodFactor(Operate_Type.OT_Relative, attr.GetWoodFactor(WoodFactor, 0));
			SetWaterFactor(Operate_Type.OT_Relative, attr.GetWaterFactor(WaterFactor, 0));
			SetFireFactor(Operate_Type.OT_Relative, attr.GetFireFactor(FireFactor, 0));
			SetEarthFactor(Operate_Type.OT_Relative, attr.GetEarthFactor(EarthFactor, 0));
			SetFullDamageFactor(Operate_Type.OT_Relative, attr.GetFullDamageFactor(FullDamageFactor, 0));
			SetHpMax(Operate_Type.OT_Relative, attr.GetHpMax(HpMax, 0));
			SetArmor(Operate_Type.OT_Relative, attr.GetArmor(Armor, 0));
			SetMiss(Operate_Type.OT_Relative, attr.GetMiss(Miss, 0));
			SetMetalResist(Operate_Type.OT_Relative, attr.GetMetalResist(MetalResist, 0));
			SetWoodResist(Operate_Type.OT_Relative, attr.GetWoodResist(WoodResist, 0));
			SetWaterResist(Operate_Type.OT_Relative, attr.GetWaterResist(WaterResist, 0));
			SetFireResist(Operate_Type.OT_Relative, attr.GetFireResist(FireResist, 0));
			SetEarthResist(Operate_Type.OT_Relative, attr.GetEarthResist(EarthResist, 0));
			SetAccuracyRecover(Operate_Type.OT_Relative, attr.GetAccuracyRecover(AccuracyRecover, 0));
			SetDamageDerate(Operate_Type.OT_Relative, attr.GetDamageDerate(DamageDerate, 0));
			SetMoveSpeed(Operate_Type.OT_Relative, attr.GetMoveSpeed(MoveSpeed, 0));
			SetFullElementResist(Operate_Type.OT_Relative, attr.GetFullElementResist(FullElementResist, 0));
			SetFullElementFactor(Operate_Type.OT_Relative, attr.GetFullElementFactor(FullElementFactor, 0));
			SetHpFactor(Operate_Type.OT_Relative, attr.GetHpFactor(HpFactor, 0));
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
