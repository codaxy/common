using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codaxy.Common.SqlServer;

namespace ConsolePlayground
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var driver = new Codaxy.Common.SqlServer.SqlSchemaUpgradeManager
            {
                BackupLocation = ".",
                ConnectionString = "Server=.;Database=EntranceDev;Integrated Security=True",                
                GetVersionSqlCommandText = "SELECT [Current] FROM [Version]",
                SetVersionSqlCommandText = "UPDATE [Version] SET [Current]='{Version}'",
                UpgradeBackupAndRestoreOnError = false
            })
            {                
                driver.UpgradeSchema(new SqlScript[] {  
                     new SqlScript { 
                         SQL = "SELECT 1", 
                         Name = "0001.sql"
                     }
                });
            }
        }
    }
}
