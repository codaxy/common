using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Codaxy.Common.Logging
{
    public enum LogLevel
    {
        Trace, Debug, Info, Warning, Error
    }

    public class LogMessage
    {
        public DateTime Time { get; internal set; }
        public LogLevel Level { get; set; }
        public String Message { get; set; }
        public String StackTrace { get; set; }
    }

    public class LogEntry
    {
        public String LoggerName { get; set; }
        public LogMessage Message { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("{0:yyyy-MM-dd HH:mm:ss.fff} {1:-10} {2}: {3}", Message.Time, Message.Level.ToString(), LoggerName, Message.Message));
            if (Message.StackTrace != null)
            {
                sb.Append("\t\t\t");
                sb.AppendLine(Message.StackTrace);
                sb.AppendLine(); //extra line after the mess that stack trace made
            }
            return sb.ToString();
        }
    }
   

	public class LogEntryEventArgs : EventArgs
	{
		public LogEntry LogEntry { get; set; }
	}

    public class Logger
    {		
		ILogAppender LogAppender { get; set; }
		
		public Logger(ILogAppender appender, String name = "")
		{			
			Enabled = true;
			LogAppender = appender;
			LogName = name;			
		}
        
		public String LogName { get; private set; }

		internal Logger CreateLogger(String childLogName)
		{
		    return new Logger(LogAppender, LogName + "/" + childLogName);            
		}
		
		public Boolean Enabled { get; set; }

		public event EventHandler<LogEntryEventArgs> OnLogEntry;

		internal void Log(LogMessage lm)
		{
			if (!Enabled)
				return;

			if (lm.Time == default(DateTime))
				lm.Time = DateTime.Now;

			var le = new LogEntry
			{
				Message = lm,
				LoggerName = LogName
			};		

			LogEntry(le);

			if (OnLogEntry != null)
				OnLogEntry(this, new LogEntryEventArgs { LogEntry = le });
		}

		internal void LogFormat(LogLevel level, String format, params object[] args)
		{
			try
			{
				Log(new LogMessage
				{
					Level = level,
					Message = String.Format(format, args)
				});
			}
			catch
			{
				Log(new LogMessage
				{
					Level = LogLevel.Error,
					Message = String.Format("Log entry '{0}' formatting failed.", format)
				});
			}
		}

		protected virtual void LogEntry(LogEntry le)
		{
			LogAppender.Log(le);
		}
    }

	public static class LoggerExtensions
	{
		public static void LogMessage(this Logger logger, LogMessage lm)
		{
			if (logger != null)
				logger.Log(lm);
		}

		public static void Trace(this Logger logger, string message)
		{
			if (logger != null)
				logger.Log(new LogMessage { Message = message, Level = LogLevel.Trace });
		}

		public static void Info(this Logger logger, string message)
		{
			if (logger != null)
				logger.Log(new LogMessage { Message = message, Level = LogLevel.Info });
		}

		[Conditional("DEBUG")]
		public static void Debug(this Logger logger, string message)
		{
			if (logger != null)
				logger.Log(new LogMessage { Message = message, Level = LogLevel.Debug });
		}

		public static void Warning(this Logger logger, string message)
		{
			if (logger != null)
				logger.Log(new LogMessage { Message = message, Level = LogLevel.Warning });
		}

		public static void Error(this Logger logger, string message)
		{
			if (logger != null)
				logger.Log(new LogMessage { Message = message, Level = LogLevel.Error });
		}

		public static void Error(this Logger logger, string message, String stackTrace)
		{
			if (logger != null)
				logger.Log(new LogMessage { Message = message, Level = LogLevel.Error, StackTrace = stackTrace });
		}

		public static void TraceFormat(this Logger logger, string format, params object[] par)
		{
			if (logger != null)
				logger.LogFormat(LogLevel.Trace, format, par);
		}

		public static void WarningFormat(this Logger logger, string format, params object[] par)
		{
			if (logger != null)
				logger.LogFormat(LogLevel.Warning, format, par);
		}

		public static void InfoFormat(this Logger logger, string format, params object[] par)
		{
			if (logger != null)
				logger.LogFormat(LogLevel.Info, format, par);
		}

		[Conditional("DEBUG")]
		public static void DebugFormat(this Logger logger, string format, params object[] par)
		{
			if (logger != null)
				logger.LogFormat(LogLevel.Debug, format, par);
		}

		public static void ErrorFormat(this Logger logger, string format, params object[] par)
		{
			if (logger != null)
				logger.LogFormat(LogLevel.Error, format, par);
		}

		public static void Exception(this Logger logger, Exception ex)
		{
			if (logger != null && ex!=null)
				logger.Log(new LogMessage { Message = ex.Message, StackTrace = ex.StackTrace, Level = LogLevel.Error });
		}

		public static void Exception(this Logger logger, String message, Exception ex)
		{
			if (logger != null)
				logger.Log(GetExceptionMessage(message, ex));
		}

        public static LogMessage GetExceptionMessage(String message, Exception ex)
        {
            if (ex == null)
                return new LogMessage
                {
                    Message = message,
                    Level = LogLevel.Error
                };

            var stackTrace = ex.StackTrace;
            if (ex.InnerException != null)
                stackTrace += Environment.NewLine + Environment.NewLine + "Inner Exception: " + ex.InnerException.ToString();

            return new LogMessage
            {
                Message = message + " (" + ex.Message + ")",
                StackTrace = stackTrace,
                Level = LogLevel.Error
            };
        }

		public static Logger ChildLogger(this Logger logger, String childLoggerName)
		{
			if (logger != null)
				return logger.CreateLogger(childLoggerName);
			return null;
		}
	}
}
