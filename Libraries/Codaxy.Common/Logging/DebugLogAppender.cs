using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Codaxy.Common.Logging
{
    /// <summary>
    /// It will not work if you are using Nuget, as assembly needs to be compiled with DEBUG option
    /// </summary>
    public class DebugLogAppender : ILogAppender
    {
#if DEBUG
        public void Log(LogEntry le)
        {
            Debug.WriteLine(String.Format("{0:yyyy-MM-dd HH:mm:ss.fff} {1:-10} {2}: {3}", le.Message.Time, le.Message.Level.ToString(), le.LoggerName, le.Message.Message));
            if (le.Message.StackTrace != null)
            {
                Debug.Write("\t\t\t");
                Debug.WriteLine(le.Message.StackTrace);
            }
        }
#else
        public void Log(LogEntry le) {}
#endif
    }
}
