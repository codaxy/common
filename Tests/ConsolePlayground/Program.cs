using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codaxy.Common.SqlServer;
using Codaxy.Common.Logging;

namespace ConsolePlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new Codaxy.Common.SqlServer.SqlSchemaUpgradeManager
            {
                BackupLocation = ".",
                ConnectionString = "Server=.;Database=UpgradeTesting;Integrated Security=True",
                GetVersionSqlCommandText = "SELECT [Current] FROM dbo.[Version]",
                SetVersionSqlCommandText = "UPDATE dbo.[Version] SET [Current]='{Version}'",
                UpgradeBackupAndRestoreOnError = true,
                IgnoreScriptFailures = true,
                Logger = new Logger(new ConsoleLogAppender())
            })
            {
                driver.UpgradeSchema(new SqlScript[] {  
                     new SqlScript { 
                         SQL = "SELECT 1", 
                         Name = "1000.sql"
                     },
                     new SqlScript { 
                         SQL = "ERROR", 
                         Name = "1001.sql"
                     },
                     new SqlScript { 
                         SQL = "SELECT 2", 
                         Name = "1002.sql"
                     }
                });
            }
        }
    }
}
