
using System;
using System.Collections.Generic;
using Util.MyMath;

namespace Util
{

  /**
   * @brief 资源类解析工具
   */
  public class Converter
  {

    /**
     * @brief 将字符串解析为Vector2D
     *
     * @param vec 字符串,类似于"100,200"
     *
     * @return 
     */
    public static Vector2 ConvertVector2D(string vec)
    {
      string strPos = vec;
      string[] resut = strPos.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);
      Vector2 vector = new Vector2(Convert.ToSingle(resut[0]), Convert.ToSingle(resut[1]));
      return vector;
    }

    /**
     * @brief 将字符串解析为字符串数组
     *
     * @param vec 字符串,类似于"100,200,200"
     *
     * @return 
     */
    public static List<string> ConvertStringList(string vec)
    {
      string[] resut = vec.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);
      List<string> list = new List<string>();
      foreach(string str in resut)
      {
        list.Add(str);
      }

      return list;
    }

    /**
     * @brief 将字符串解析为Vector3D
     *
     * @param vec 字符串,类似于"100,200,200"
     *
     * @return 
     */
    public static Vector3 ConvertVector3D(string vec)
    {
      string strPos = vec;
      string[] resut = strPos.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);
      Vector3 vector = new Vector3(Convert.ToSingle(resut[0]), Convert.ToSingle(resut[1]), Convert.ToSingle(resut[2]));
      return vector;
    }

    /**
     * @brief 将字符串解析为Vector3D,忽略第一个字符串
     *
     * @param vec 字符串,类似于"100,200,200"
     *
     * @return 
     */
    public static Vector4 ConvertVector4D(string vec)
    {
      string strPos = vec;
      string[] resut = strPos.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);
      Vector4 vector = new Vector4(Convert.ToSingle(resut[0]), Convert.ToSingle(resut[1]), Convert.ToSingle(resut[2]),Convert.ToSingle(resut[3]));
      return vector;
    }

    /**
     * @brief 将字符串解析为int数组
     *
     * @param vec 字符串，类似于"1,2,3,4"
     *
     * @return 
     */
    public static List<T> ConvertNumericList<T>(string vec)
    {
      List<T> list = new List<T>();
      string strPos = vec;
      string[] resut = strPos.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);

      try
      {
        if (resut != null && resut.Length > 0 && resut[0] != "")
        {
          for (int index = 0; index < resut.Length; index++)
          {
            list.Add((T)Convert.ChangeType(resut[index], typeof(T)));
          }
        }
      }
      catch (System.Exception ex)
      {
        string info = string.Format("ConvertNumericList vec:{0} ex:{1} stacktrace:{2}",
          vec, ex.Message, ex.StackTrace);
        LogUtil.Debug(info);

        list.Clear();
      }

      return list;
    }

    /**
     * @brief 将字符串解析为Vector2D
     *
     * @param vec
     *
     * @return 
     */
    public static List<Vector2> ConvertVector2DList(string vec)
    {
      List<Vector2> path = new List<Vector2>();
      string strPos = vec;
      string[] resut = strPos.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);
      if (resut != null && resut.Length > 0 && resut[0] != "")
      {
        for (int index = 0; index < resut.Length; )
        {
          path.Add(new Vector2(Convert.ToSingle(resut[index]), Convert.ToSingle(resut[index + 1])));
          index += 2;
        }
      }

      return path;
    }

    /**
     * @brief 将字符串解析为Vector3D
     *
     * @param vec
     *
     * @return 
     */
    public static List<Vector3> ConvertVector3DList(string vec)
    {
      List<Vector3> path = new List<Vector3>();
      string strPos = vec;
      string[] resut = strPos.Split(s_ListSplitString, StringSplitOptions.RemoveEmptyEntries);
      if (resut != null && resut.Length > 0 && resut[0] != "")
      {
        for (int index = 0; index < resut.Length; )
        {
          path.Add(new Vector3(Convert.ToSingle(resut[index]), 
                Convert.ToSingle(resut[index + 1]),
                Convert.ToSingle(resut[index + 2])));
          index += 3;
        }
      }

      return path;
    }

    /**
     * @brief 将字符串解析为枚举类型
     *
     * @param T 枚举类型
     * @param name 枚举类型的名字
     *
     * @return 
     */
    public static T ConvertStrToEnum<T>(string name)
    {
      return (T)(Enum.Parse(typeof(T), name));
    }

    /**
     * @brief 将枚举类型解析为字符串
     *
     * @param T 枚举类型
     * @param name 枚举类型的名字
     *
     * @return 
     */
    public static string ConvertEnumToStr<T>(T tVal)
    {
      return Enum.GetName(typeof(T), tVal);
    }

    private static string[] s_ListSplitString = new string[] { ",", " ", ", ", "|" };
  }
}
