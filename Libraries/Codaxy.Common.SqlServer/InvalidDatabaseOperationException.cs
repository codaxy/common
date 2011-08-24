using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Codaxy.Common.SqlServer
{
    public class InvalidDatabaseOperationException : InvalidOperationException
    {
        public InvalidDatabaseOperationException() { }
        public InvalidDatabaseOperationException(string message) : base(message) { }
        public InvalidDatabaseOperationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidDatabaseManagerSettingsException : InvalidDatabaseOperationException
    {
        public InvalidDatabaseManagerSettingsException(string message) : base(message) { }
    }

    public class ExecuteDatabaseScriptException : InvalidDatabaseOperationException
    {
        public ExecuteDatabaseScriptException(string message, Exception innerException, SqlScript script)
            : base(message, innerException)
        {
            Script = script;
        }

        public SqlScript Script { get; set; }
    }
}
