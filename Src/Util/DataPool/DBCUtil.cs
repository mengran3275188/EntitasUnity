using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public sealed partial class DBCUtil
    {
        public static string ExtractString(DBC_Row node, string nodeName, string defaultVal)
        {
            string result = defaultVal;
            if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                result = nodeText;
            }
            return result;
        }
        public static List<string> ExtractStringList(DBC_Row node, string nodeName, string[] defaultVal)
        {
            List<string> result = new List<string>();
            if (node == null || !node.HasFields)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                result = Converter.ConvertStringList(nodeText);
            }
            else if (null != defaultVal)
            {
                result.AddRange(defaultVal);
            }
            return result;
        }
        public static string[] ExtractStringArray(DBC_Row node, string nodeName, string[] defaultVal)
        {
            List<string> list = ExtractStringList(node, nodeName, defaultVal);
            if (null != list)
            {
                return list.ToArray();
            }
            else
            {
                return null;
            }
        }
        public static bool ExtractBool(DBC_Row node, string nodeName, bool defaultVal)
        {
            bool result = defaultVal;
            if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                if (nodeText.Trim().ToLower() == "true" || nodeText.Trim().ToLower() == "1")
                {
                    result = true;
                }
                if (nodeText.Trim().ToLower() == "false" || nodeText.Trim().ToLower() == "0")
                {
                    result = false;
                }
            }
            return result;
        }
        public static T ExtractNumeric<T>(DBC_Row node, string nodeName, T defaultVal)
        {
            T result = defaultVal;
            if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                try
                {
                    result = (T)Convert.ChangeType(nodeText, typeof(T));
                }
                catch (System.Exception ex)
                {
                    LogUtil.Error("ExtractNumeric {0} {1} Exception:{2}/{3}", nodeName, nodeText, ex.Message, ex.StackTrace);
                    throw;
                }
            }
            return result;
        }
        public static List<T> ExtractNumericList<T>(DBC_Row node, string nodeName, T[] defaultVal)
        {
            List<T> result = new List<T>();
            if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                result = Converter.ConvertNumericList<T>(nodeText);
            }
            else if (null != defaultVal)
            {
                result.AddRange(defaultVal);
            }
            return result;
        }
        public static T[] ExtractNumericArray<T>(DBC_Row node, string nodeName, T[] defaultVal)
        {
            List<T> list = ExtractNumericList<T>(node, nodeName, defaultVal);
            if (null != list)
            {
                return list.ToArray();
            }
            else
            {
                return null;
            }
        }
        public static List<string> ExtractNodeByPrefix(DBC_Row node, string prefix)
        {
            if (node == null || !node.HasFields)
            {
                return null;
            }
            return node.SelectFieldsByPrefix(prefix);
        }

        public static string ExtractString(DBC_Row node, string nodeName, string defaultVal, bool isMust)
        {
            return ExtractString(node, nodeName, defaultVal);
        }
        public static List<string> ExtractStringList(DBC_Row node, string nodeName, string defaultVal, bool isMust)
        {
            List<string> result = new List<string>();
            if (node == null || !node.HasFields)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                result = Converter.ConvertStringList(nodeText);
            }
            return result;
        }
        public static bool ExtractBool(DBC_Row node, string nodeName, bool defaultVal, bool isMust)
        {
            return ExtractBool(node, nodeName, defaultVal);
        }
        public static T ExtractNumeric<T>(DBC_Row node, string nodeName, T defaultVal, bool isMust)
        {
            return ExtractNumeric<T>(node, nodeName, defaultVal);
        }
        public static List<T> ExtractNumericList<T>(DBC_Row node, string nodeName, T defaultVal, bool isMust)
        {
            List<T> result = new List<T>();
            if (node == null || !node.HasFields || node.SelectFieldByName(nodeName) == null)
            {
                return result;
            }
            string nodeText = node.SelectFieldByName(nodeName);
            if (!String.IsNullOrEmpty(nodeText))
            {
                result = Converter.ConvertNumericList<T>(nodeText);
            }
            return result;
        }
        public static int ExtractInt(BinaryTable table, int recordVal, int defaultVal)
        {
            return recordVal;
        }
        public static long ExtractLong(BinaryTable table, int recordVal, long defaultVal)
        {
            return recordVal;
        }
        public static float ExtractFloat(BinaryTable table, float recordVal, float defaultVal)
        {
            return recordVal;
        }
        public static bool ExtractBool(BinaryTable table, int recordVal, bool defaultVal)
        {
            return recordVal != 0;
        }
        public static string ExtractString(BinaryTable table, int recordVal, string defaultVal)
        {
            string ret = table.GetString(recordVal);
            if (string.IsNullOrEmpty(ret))
            {
                ret = defaultVal;
            }
            return ret;
        }
        public static List<int> ExtractIntList(BinaryTable table, int recordVal, int[] defaultVal)
        {
            int[] vals = ExtractIntArray(table, recordVal, defaultVal);
            if (null != vals)
                return new List<int>(vals);
            else
                return new List<int>();
        }
        public static int[] ExtractIntArray(BinaryTable table, int recordVal, int[] defaultVal)
        {
            int[] ret = table.GetIntList(recordVal);
            if (null == ret)
            {
                ret = defaultVal;
            }
            return ret;
        }
        public static List<float> ExtractFloatList(BinaryTable table, int recordVal, float[] defaultVal)
        {
            float[] vals = ExtractFloatArray(table, recordVal, defaultVal);
            if (null != vals)
                return new List<float>(vals);
            else
                return new List<float>();
        }
        public static float[] ExtractFloatArray(BinaryTable table, int recordVal, float[] defaultVal)
        {
            float[] ret = table.GetFloatList(recordVal);
            if (null == ret)
            {
                ret = defaultVal;
            }
            return ret;
        }
        public static List<string> ExtractStringList(BinaryTable table, int recordVal, string[] defaultVal)
        {
            string[] vals = ExtractStringArray(table, recordVal, defaultVal);
            if (null != vals)
                return new List<string>(vals);
            else
                return new List<string>();
        }
        public static string[] ExtractStringArray(BinaryTable table, int recordVal, string[] defaultVal)
        {
            string[] ret = table.GetStrList(recordVal);
            if (null == ret)
            {
                ret = defaultVal;
            }
            return ret;
        }

        public static int SetValue(BinaryTable table, bool val, bool defaultVal)
        {
            return val ? 1 : 0;
        }
        public static int SetValue(BinaryTable table, int val, int defaultVal)
        {
            return val;
        }
        public static int SetValue(BinaryTable table, long val, long defaultVal)
        {
            return (int)val;
        }
        public static float SetValue(BinaryTable table, float val, float defaultVal)
        {
            return val;
        }
        public static int SetValue(BinaryTable table, string val, string defaultVal)
        {
            if (0 == val.CompareTo(defaultVal))
            {
                return -1;
            }
            return table.AddString(val);
        }
        public static int SetValue(BinaryTable table, int[] vals, int[] defaultVal)
        {
            if (IsEqual(vals, defaultVal))
            {
                return -1;
            }
            return table.AddIntList(vals);
        }
        public static int SetValue(BinaryTable table, List<int> vals, int[] defaultVal)
        {
            if (IsEqual(vals, defaultVal))
            {
                return -1;
            }
            return table.AddIntList(vals.ToArray());
        }
        public static int SetValue(BinaryTable table, float[] vals, float[] defaultVal)
        {
            if (IsEqual(vals, defaultVal))
            {
                return -1;
            }
            return table.AddFloatList(vals);
        }
        public static int SetValue(BinaryTable table, List<float> vals, float[] defaultVal)
        {
            if (IsEqual(vals, defaultVal))
            {
                return -1;
            }
            return table.AddFloatList(vals.ToArray());
        }
        public static int SetValue(BinaryTable table, string[] vals, string[] defaultVal)
        {
            if (IsEqual(vals, defaultVal))
            {
                return -1;
            }
            return table.AddStrList(vals);
        }
        public static int SetValue(BinaryTable table, List<string> vals, string[] defaultVal)
        {
            if (IsEqual(vals, defaultVal))
            {
                return -1;
            }
            return table.AddStrList(vals.ToArray());
        }

        public static void ExtractValues<T>(IList<T> list, int ct, params T[] vals)
        {
            for (int i = 0; i < ct && i < vals.Length; ++i)
            {
                list.Add(vals[i]);
            }
        }
        public static T CloneValue<T>(T ins)
        {
            return ins;
        }
        public static List<T> CloneList<T>(List<T> ins)
        {
            List<T> clone = new List<T>();
            foreach (T item in ins)
            {
                clone.Add(item);
            }
            return clone;
        }
        public static T[] CloneArray<T>(T[] ins)
        {
            T[] clone = new T[ins.Length];
            for (int idx = 0; idx < ins.Length; idx++)
            {
                clone[idx] = ins[idx];
            }
            return clone;
        }

        private static bool IsEqual(IList<int> a, IList<int> b)
        {
            if (null == a && null != b || null != a && null == b)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; ++i)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
        private static bool IsEqual(IList<float> a, IList<float> b)
        {
            if (null == a && null != b || null != a && null == b)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; ++i)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
        private static bool IsEqual(IList<string> a, IList<string> b)
        {
            if (null == a && null != b || null != a && null == b)
                return false;
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; ++i)
            {
                if (0 != a[i].CompareTo(b[i]))
                    return false;
            }
            return true;
        }
    }
}
