using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace Codaxy.Common.SqlServer
{
    public class SqlSchemaManager : IDisposable
    {
        public String ConnectionString { get; set; }

        ServerConnection sc;
        Server server;

        protected virtual void Validate()
        {
            if (ConnectionString == null)
                throw new InvalidDatabaseManagerSettingsException("Connection string not set.");
        }

        protected Server GetServer(out String databaseName)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(ConnectionString);
            sc = new ServerConnection(csb.DataSource);
            if (!csb.IntegratedSecurity)
            {
                sc.LoginSecure = false;
                sc.Login = csb.UserID;
                sc.Password = csb.Password;
            }            
            databaseName = csb.InitialCatalog;
            server = new Server(sc);
            //TODO: Validate server connection
            return server;
        }

        public virtual void Dispose()
        {
			if (server != null && server.ConnectionContext!=null)
				server.ConnectionContext.Disconnect();			

            if (sc != null && sc.IsOpen)
                sc.Disconnect();
        }
    }
}
