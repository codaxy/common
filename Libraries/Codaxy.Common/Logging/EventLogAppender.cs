using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Codaxy.Common.Logging
{
    public class EventLogAppender : ILogAppender
    {
        string source;

        public EventLogAppender(String source)
        {
            if (!EventLog.SourceExists(source))
                throw new InvalidOperationException(String.Format("Event source '{0}' is not registered on this computer."));

            this.source = source;
        }

        public void Log(LogEntry entry)
        {
            try
            {
                var entryType = GetEntryType(entry.Message.Level);

                StringBuilder mb = new StringBuilder();

                mb.AppendFormat("{0} {1}:", entry.Message.Time, entry.LoggerName).AppendLine();
                mb.AppendLine();
                mb.AppendLine(entry.Message.Message);

                if (entry.Message.StackTrace != null)
                {
                    mb.AppendLine();
                    mb.AppendLine(entry.Message.StackTrace);
                }

                EventLog.WriteEntry(source, mb.ToString(), entryType);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EventLogAppender.Log exception: " + ex);
            }
        }

        private EventLogEntryType GetEntryType(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error: return EventLogEntryType.Error;
                case LogLevel.Warning: return EventLogEntryType.Warning;
                default: return EventLogEntryType.Information;
            }
        }
    }
}
