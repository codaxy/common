using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codaxy.Common.Logging
{
    public class ForwardAppender : ILogAppender
    {
        public ForwardAppender(params ILogAppender[] appenders)
        {
            Appenders = new List<ILogAppender>(appenders);
        }

        public List<ILogAppender> Appenders { get; set; }
        public LogLevel? LevelFilter { get; set; }

        public void Log(LogEntry entry)
        {
            if (entry.Message.Level < LevelFilter)
                return;

            foreach (var appender in Appenders)
                appender.Log(entry);
        }
    }
}
