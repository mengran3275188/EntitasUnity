using System;

namespace Util
{
    public enum Log_Type
    {
        LT_Debug,
        LT_Info,
        LT_Warn,
        LT_Monitor,
        LT_Error,
        LT_Assert,
    }
    public delegate void LogSystemOutputDelegation(Log_Type type, string msg);
    public delegate void LogSystemOutputWithConsoleColorDelegation(Log_Type type, ConsoleColor cc, string msg);
    /**
     * @brief 日志系统
     */
    public class LogUtil
    {
        public static LogSystemOutputDelegation OnOutput;
        public static LogSystemOutputWithConsoleColorDelegation OnOutputWithConsoleColor;
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Debug]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Debug, str);
        }
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(ConsoleColor cc, string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Debug]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Debug, str);
        }
        public static void Info(string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Info]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Info, str);
        }
        public static void Info(ConsoleColor cc, string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Info]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Info, cc, str);
        }
        public static void Monitor(string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Monitor]";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Monitor, str);
        }
        public static void Monitor(ConsoleColor cc, string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Monitor]";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Monitor, cc, str);
        }
        public static void Warn(string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Warn]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Warn, str);
        }
        public static void Warn(ConsoleColor cc, string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Warn]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Warn, cc, str);
        }
        public static void Error(string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Error]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Error, str);
        }
        public static void Error(ConsoleColor cc, string format, params object[] args)
        {
            string assit = DateTime.Now.ToString() + "[Error]:";
            string str = string.Format(assit + format, args);
            Output(Log_Type.LT_Error, cc, str);
        }
        public static void Assert(bool check, string format, params object[] args)
        {
            if (!check)
            {
                string str = string.Format("[Assert]:" + format, args);
                Output(Log_Type.LT_Assert, str);
            }
        }
        public static void Assert(bool check, ConsoleColor cc, string format, params object[] args)
        {
            if (!check)
            {
                string str = string.Format("[Assert]:" + format, args);
                Output(Log_Type.LT_Assert, cc, str);
            }
        }
        public static void CallStack()
        {
            Error("Call stack :\n{0}\n", Environment.StackTrace);
        }

        private static void Output(Log_Type type, string msg)
        {
            if (null != OnOutput)
            {
                OnOutput(type, msg);
            }
        }
        private static void Output(Log_Type type, ConsoleColor cc, string msg)
        {
            if(null != OnOutputWithConsoleColor)
            {
                OnOutputWithConsoleColor(type, cc, msg);
            }
        }
    }
}
