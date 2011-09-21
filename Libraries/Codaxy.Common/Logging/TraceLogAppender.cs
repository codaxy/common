using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Codaxy.Common.Logging
{    
    public class TraceLogAppender : ILogAppender
    {
        public void Log(LogEntry le)
        {
            Trace.WriteLine(String.Format("{0:yyyy-MM-dd HH:mm:ss.fff} {1:-10} {2}: {3}", le.Message.Time, le.Message.Level.ToString(), le.LoggerName, le.Message.Message));
            if (le.Message.StackTrace != null)
            {
                Trace.Write("\t\t\t");
                Trace.WriteLine(le.Message.StackTrace);
            }
        }
    }
}
