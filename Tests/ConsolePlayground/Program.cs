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
                ConnectionString = "Server=.;Database=ECardDev;Integrated Security=True",                
                GetVersionSqlCommandText = "SELECT [Current] FROM ec.[Version]",
                SetVersionSqlCommandText = "UPDATE ec.[Version] SET [Current]='{Version}'",
                UpgradeBackupAndRestoreOnError = true,
                Logger = new Logger(new ConsoleLogAppender(), "")                
            })
            {                
                driver.UpgradeSchema(new SqlScript[] {  
                     new SqlScript { 
                         SQL = "AAAFFF", 
                         Name = "1000.sql"
                     }
                });
            }
        }
    }
}
