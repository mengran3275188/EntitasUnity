using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Util
{
    public class HomePath : Singleton<HomePath>
    {
        public HomePath()
        {
          string tmpPath = Assembly.GetExecutingAssembly().Location;
          m_HomePath = tmpPath.Substring(0, tmpPath.LastIndexOfAny(new char[] { '\\', '/' }));
        }
        public string GetAbsolutePath(string path)
        {
          return Path.GetFullPath(Path.Combine(m_HomePath, path));
        }
        public void SetHomePath(string path)
        {
            m_HomePath = path;
        }
        public static string RelativePath(string absPath, string relTo)
        {
            string[] absDirs = absPath.Split(Path.DirectorySeparatorChar);
            string[] relDirs = relTo.Split(Path.DirectorySeparatorChar);

            int len = Math.Min(absDirs.Length, relDirs.Length);
            int lastCommonRoot = -1;
            int index = 0;
            for(index = 0; index < len; ++index)
            {
                if (absDirs[index] == relDirs[index])
                    lastCommonRoot = index;
                else
                    break;
            }
            if (lastCommonRoot == -1)
                throw new ArgumentException("Paths do not have a common base.");

            StringBuilder relativePath = new StringBuilder();
            for(index = lastCommonRoot + 1; index < absDirs.Length; index++)
            {
                if (absDirs[index].Length > 0) relativePath.Append("../");
            }
            for(index = lastCommonRoot + 1; index < relDirs.Length - 1; index++)
            {
                relativePath.Append(relDirs[index] + "/");
            }
            relativePath.Append(relDirs[relDirs.Length - 1]);

            return relativePath.ToString();
        }

        private string m_HomePath = "";
    }
}
