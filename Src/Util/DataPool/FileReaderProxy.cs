using System;
using System.IO;

namespace Util
{
    public delegate byte[] delegate_ReadFile(string path);

    public static class FileReaderProxy
    {
        private static delegate_ReadFile handlerReadFile;

        public static MemoryStream ReadFileAsMemoryStream(string filePath)
        {
            try
            {
                byte[] buffer = ReadFileAsArray(filePath);
                if (buffer == null)
                {
                    LogUtil.Error("Err ReadFileAsMemoryStream failed:{0}\n", filePath);
                    return null;
                }
                return new MemoryStream(buffer);
            }
            catch (Exception e)
            {
                LogUtil.Error("Exception:{0}\n", e.Message);
                LogUtil.CallStack();
                return null;
            }
        }

        public static byte[] ReadFileAsArray(string filePath)
        {
            byte[] buffer = null;
            try
            {
                if (handlerReadFile != null)
                {
                    buffer = handlerReadFile(filePath);
                }
                else
                {
                    LogUtil.Error("ReadFileByEngine handler have not register: {0}", filePath);
                }
            }
            catch (Exception e)
            {
                LogUtil.Error("Exception:{0}\n", e.Message);
                LogUtil.CallStack();
                return null;
            }
            return buffer;
        }

        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void RegisterReadFileHandler(delegate_ReadFile handler)
        {
            handlerReadFile = handler;
        }

    }
}
