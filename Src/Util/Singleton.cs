using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public abstract class Singleton<T> where T : class, new()
    {
        static Singleton() { }
        public static T Instance
        {
            get { return s_Instance; }
        }

        private readonly static T s_Instance = Activator.CreateInstance<T>();

    }
}
