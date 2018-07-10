//----------------------------------------------------------------------------
//！！！不要手动修改此文件，此文件由LogicDataGenerator按AttributeConfig.txt生成！！！
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Util;

namespace Entitas.Data
{
	public sealed partial class AttributeConfig : IData2
	{
	private enum ValueType : int
		{
			AbsoluteValue = 0,
			PercentValue,
			LevelRateValue,
		}
		public const int c_MaxAbsoluteValue = 1000000000;
		public const int c_MaxPercentValue = 2000000000;
		public const float c_Rate = 100.0f;

		private float m_AddEnergyRecover = 0;
		private int m_EnergyRecoverType = 0;
		private float m_AddEnergyMax = 0;
		private int m_EnergyMaxType = 0;
		private float m_AddMaxAd = 0;
		private int m_MaxAdType = 0;
		private float m_AddMinAd = 0;
		private int m_MinAdType = 0;
		private float m_AddCritical = 0;
		private int m_CriticalType = 0;
		private float m_AddCriticalFactor = 0;
		private int m_CriticalFactorType = 0;
		private float m_AddMetalFactor = 0;
		private int m_MetalFactorType = 0;
		private float m_AddWoodFactor = 0;
		private int m_WoodFactorType = 0;
		private float m_AddWaterFactor = 0;
		private int m_WaterFactorType = 0;
		private float m_AddFireFactor = 0;
		private int m_FireFactorType = 0;
		private float m_AddEarthFactor = 0;
		private int m_EarthFactorType = 0;
		private float m_AddFullDamageFactor = 0;
		private int m_FullDamageFactorType = 0;
		private float m_AddHpMax = 0;
		private int m_HpMaxType = 0;
		private float m_AddArmor = 0;
		private int m_ArmorType = 0;
		private float m_AddMiss = 0;
		private int m_MissType = 0;
		private float m_AddMetalResist = 0;
		private int m_MetalResistType = 0;
		private float m_AddWoodResist = 0;
		private int m_WoodResistType = 0;
		private float m_AddWaterResist = 0;
		private int m_WaterResistType = 0;
		private float m_AddFireResist = 0;
		private int m_FireResistType = 0;
		private float m_AddEarthResist = 0;
		private int m_EarthResistType = 0;
		private float m_AddAccuracyRecover = 0;
		private int m_AccuracyRecoverType = 0;
		private float m_AddDamageDerate = 0;
		private int m_DamageDerateType = 0;
		private float m_AddMoveSpeed = 0;
		private int m_MoveSpeedType = 0;
		private float m_AddFullElementResist = 0;
		private int m_FullElementResistType = 0;
		private float m_AddFullElementFactor = 0;
		private int m_FullElementFactorType = 0;
		private float m_AddHpFactor = 0;
		private int m_HpFactorType = 0;

		private void AfterCollectData()
		{
			m_AddEnergyRecover = CalcRealValue(AddEnergyRecover, out m_EnergyRecoverType);
			m_AddEnergyMax = CalcRealValue(AddEnergyMax, out m_EnergyMaxType);
			m_AddMaxAd = CalcRealValue(AddMaxAd, out m_MaxAdType);
			m_AddMinAd = CalcRealValue(AddMinAd, out m_MinAdType);
			m_AddCritical = CalcRealValue(AddCritical, out m_CriticalType);
			m_AddCriticalFactor = CalcRealValue(AddCriticalFactor, out m_CriticalFactorType);
			m_AddMetalFactor = CalcRealValue(AddMetalFactor, out m_MetalFactorType);
			m_AddWoodFactor = CalcRealValue(AddWoodFactor, out m_WoodFactorType);
			m_AddWaterFactor = CalcRealValue(AddWaterFactor, out m_WaterFactorType);
			m_AddFireFactor = CalcRealValue(AddFireFactor, out m_FireFactorType);
			m_AddEarthFactor = CalcRealValue(AddEarthFactor, out m_EarthFactorType);
			m_AddFullDamageFactor = CalcRealValue(AddFullDamageFactor, out m_FullDamageFactorType);
			m_AddHpMax = CalcRealValue(AddHpMax, out m_HpMaxType);
			m_AddArmor = CalcRealValue(AddArmor, out m_ArmorType);
			m_AddMiss = CalcRealValue(AddMiss, out m_MissType);
			m_AddMetalResist = CalcRealValue(AddMetalResist, out m_MetalResistType);
			m_AddWoodResist = CalcRealValue(AddWoodResist, out m_WoodResistType);
			m_AddWaterResist = CalcRealValue(AddWaterResist, out m_WaterResistType);
			m_AddFireResist = CalcRealValue(AddFireResist, out m_FireResistType);
			m_AddEarthResist = CalcRealValue(AddEarthResist, out m_EarthResistType);
			m_AddAccuracyRecover = CalcRealValue(AddAccuracyRecover, out m_AccuracyRecoverType);
			m_AddDamageDerate = CalcRealValue(AddDamageDerate, out m_DamageDerateType);
			m_AddMoveSpeed = CalcRealValue(AddMoveSpeed, out m_MoveSpeedType);
			m_AddFullElementResist = CalcRealValue(AddFullElementResist, out m_FullElementResistType);
			m_AddFullElementFactor = CalcRealValue(AddFullElementFactor, out m_FullElementFactorType);
			m_AddHpFactor = CalcRealValue(AddHpFactor, out m_HpFactorType);
		}

		public float GetEnergyRecover(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddEnergyRecover, m_EnergyRecoverType);
		}
		public float GetEnergyMax(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddEnergyMax, m_EnergyMaxType);
		}
		public float GetMaxAd(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddMaxAd, m_MaxAdType);
		}
		public float GetMinAd(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddMinAd, m_MinAdType);
		}
		public float GetCritical(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddCritical, m_CriticalType);
		}
		public float GetCriticalFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddCriticalFactor, m_CriticalFactorType);
		}
		public float GetMetalFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddMetalFactor, m_MetalFactorType);
		}
		public float GetWoodFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddWoodFactor, m_WoodFactorType);
		}
		public float GetWaterFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddWaterFactor, m_WaterFactorType);
		}
		public float GetFireFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddFireFactor, m_FireFactorType);
		}
		public float GetEarthFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddEarthFactor, m_EarthFactorType);
		}
		public float GetFullDamageFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddFullDamageFactor, m_FullDamageFactorType);
		}
		public float GetHpMax(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddHpMax, m_HpMaxType);
		}
		public float GetArmor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddArmor, m_ArmorType);
		}
		public float GetMiss(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddMiss, m_MissType);
		}
		public float GetMetalResist(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddMetalResist, m_MetalResistType);
		}
		public float GetWoodResist(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddWoodResist, m_WoodResistType);
		}
		public float GetWaterResist(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddWaterResist, m_WaterResistType);
		}
		public float GetFireResist(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddFireResist, m_FireResistType);
		}
		public float GetEarthResist(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddEarthResist, m_EarthResistType);
		}
		public float GetAccuracyRecover(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddAccuracyRecover, m_AccuracyRecoverType);
		}
		public float GetDamageDerate(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddDamageDerate, m_DamageDerateType);
		}
		public float GetMoveSpeed(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddMoveSpeed, m_MoveSpeedType);
		}
		public float GetFullElementResist(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddFullElementResist, m_FullElementResistType);
		}
		public float GetFullElementFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddFullElementFactor, m_FullElementFactorType);
		}
		public float GetHpFactor(float refVal, int refLevel)
		{
			return CalcAddedAttrValue(refVal, refLevel, m_AddHpFactor, m_HpFactorType);
		}

		private float CalcRealValue(int tableValue, out int type)
		{
			float retVal = 0;
			int val = tableValue;
			bool isNegative = false;
			if(tableValue < 0){;
				isNegative =true;
				val = -val;
			};
			if(val < c_MaxAbsoluteValue) {
				retVal = val / c_Rate;
				type = (int)ValueType.AbsoluteValue;
			}else if(val < c_MaxPercentValue) {
				retVal = (val - c_MaxAbsoluteValue) / c_Rate / 100;
				type = (int)ValueType.PercentValue;
			}else{
				retVal = (val - c_MaxPercentValue) / c_Rate / 100;
				type = (int)ValueType.LevelRateValue;
			}
			if(isNegative)
				retVal = -retVal;
			return retVal;
		}

		private float CalcAddedAttrValue(float refVal, int refLevel, float addVal, int type)
		{
			float retVal = 0;
			switch(type){
				case (int)ValueType.AbsoluteValue:
					retVal = addVal;
					break;
				case (int)ValueType.PercentValue:
					retVal = refVal * addVal;
					break;
				case (int)ValueType.LevelRateValue:
					retVal = refLevel * addVal;
					break;
			}
			return retVal;
		}
	}
}

