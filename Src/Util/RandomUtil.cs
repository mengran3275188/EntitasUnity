using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public static class RandomUtil
    {

        static public int Next()
        {
            return Instance.Next();
        }
        static public int Next(int max)
        {
            return Instance.Next(max);
        }
        static public int Next(int min, int max)
        {
            return Instance.Next(min, max);
        }
        static public float NextFloat()
        {
            return (float)Instance.NextDouble();
        }
        static private Random Instance
        {
            get
            {
                if (null == m_Random)
                    m_Random = new Random();
                return m_Random;
            }
        }
        [ThreadStatic]
        static private Random m_Random = null;
    }
}
