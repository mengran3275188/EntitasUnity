using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    public sealed class TimeUtility : Singleton<TimeUtility>
    {
        public float GetLocalSeconds()
        {
            return (GetElapsedTimeUs() - m_StartTimeUs) / 1000000f;
        }
        public long GetLocalMilliseconds()
        {
            return (GetElapsedTimeUs() - m_StartTimeUs) / 1000;
        }
        public long GetElapsedTimeUs()
        {
            return DateTime.Now.Ticks / 10;
        }
        public TimeUtility()
        {
            m_StartTimeUs = GetElapsedTimeUs();
        }

        private long m_StartTimeUs = 0;
    }
}
