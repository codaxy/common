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
            Debug.Write(le.ToString());
        }
#else
        public void Log(LogEntry le) {}
#endif
    }
}
