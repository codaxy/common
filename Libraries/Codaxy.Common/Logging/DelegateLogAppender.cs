using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Codaxy.Common.Logging
{
    public class DelegateLogAppender : ILogAppender
    {
        public Action<LogEntry> LogAction { get; set; }

        public DelegateLogAppender() { }
        public DelegateLogAppender(Action<LogEntry> logAction)
        {
            LogAction = logAction;
        }

        public void Log(LogEntry entry)
        {
            var a = LogAction;
            if (a != null)
                try
                {
                    a(entry);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DelegateLogAppender error: " + ex.Message);
                    Debug.WriteLine(ex.StackTrace);                    
                }
        }
    }
}
