using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;

namespace Codaxy.Common.SqlServer
{
    class SqlSchemaCreateManager : SqlSchemaManager
    {
        public void CreateSchema(SqlScript[] scripts)
        {
            Validate();
            string databaseName;
            var server = GetServer(out databaseName);
            try
            {
                var db = server.Databases[databaseName];
                if (db == null)
                    throw new InvalidDatabaseOperationException(String.Format("Database '{0}' not found on server.", databaseName));

                foreach (var script in scripts)
                    try
                    {
                        db.ExecuteNonQuery(script.SQL);
                    }
                    catch (Exception ex)
                    {
                        throw new ExecuteDatabaseScriptException(String.Format("Create database script '{0}' failed. See inner exception for more details.", script.Name), ex, script);
                    }
            }
            finally
            {
                server.ConnectionContext.Disconnect();
            }
        }
    }
}
