using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Codaxy.Common.Logging;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;

namespace Codaxy.Common.SqlServer
{
    public class SqlScript
    {
        public String Name { get; set; }
        public String SQL { get; set; }

    }
    public interface ISqlScriptRepository
    {
        SqlScript[] GetScripts();
    }

    public class SqlDatabaseUpgradeManager : DatabaseManager
    {        
        public String BackupLocation { get; set; }
        public bool UpgradeBackupAndRestoreOnError { get; set; }
        public Func<String, String> ScriptVersionNumberGetter { get; set; }
        public Logger Logger { get; set; }
        public String GetVersionSqlCommandText { get; set; }
        public String SetVersionSqlCommandText { get; set; }

        class ScriptInfo
        {
            public String Version { get; set; }
            public SqlScript Script { get; set; }
        }

        protected override void Validate()
        {
            base.Validate();

            if (ScriptVersionNumberGetter == null)
                throw new InvalidDatabaseManagerSettingsException("ScriptsVersionNumberGetter");

            if (GetVersionSqlCommandText==null)
                throw new InvalidDatabaseManagerSettingsException("GetVersionSqlCommandText");

            if (SetVersionSqlCommandText == null)
                throw new InvalidDatabaseManagerSettingsException("SetVersionSqlCommandText");
        }

        public bool Upgrade(SqlScript[] scripts)
        {
            Validate();
            if (scripts.Length == 0)
                return false;
            bool upgradeStarted = false;

            string databaseName;
            var server = GetServer(out databaseName);
            var db = server.Databases[databaseName];
            if (db == null)
                throw new InvalidDatabaseOperationException(String.Format("Database '{0}' not found on server.", databaseName));


            String currentVersion;
            try
            {
                var ds = db.ExecuteWithResults(GetVersionSqlCommandText);
                currentVersion = ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidDatabaseOperationException("Could not determine current version of the database. Check get version command.", ex);
            }

            ScriptInfo[] sortedScripts;
            String targetVersion;
            try
            {
                sortedScripts = scripts.Select(a => new ScriptInfo
                    {
                        Version = ScriptVersionNumberGetter(a.Name),
                        Script = a
                    }).OrderBy(a => a.Version).ThenBy(a => a.Script.Name).ToArray();
                targetVersion = sortedScripts.Last().Version;
            }
            catch (Exception ex)
            {
                throw new InvalidDatabaseOperationException("Could not determine script version.", ex);
            }

            if (String.Compare(currentVersion, targetVersion) >= 0)
            {
                Logger.InfoFormat("Database schema is up to date. (Version: '{0}')", currentVersion);
                return false;
            }

            try
            {
                PrepareForUpgrade(server, databaseName, currentVersion);
                upgradeStarted = true;
            }
            catch (Exception ex)
            {
                throw new InvalidDatabaseOperationException("Could not make database backup. Check inner exception for more details.", ex);
            }

            try
            {
                foreach (var script in sortedScripts)
                    if (String.Compare(script.Version, currentVersion) > 0)
                        try
                        {
                            db.ExecuteNonQuery(script.Script.SQL);
                        }
                        catch (Exception ex)
                        {
                            throw new ExecuteDatabaseScriptException("Database script '{0}' failed. Check inner exception for more details.", ex, script.Script);
                        }

                try
                {
                    db.ExecuteNonQuery(SetVersionSqlCommandText.Replace("{Version}", targetVersion));
                }
                catch (Exception ex)
                {
                    throw new InvalidDatabaseOperationException("Setting the new database version failed. Check set version command and inner exception for more details.", ex);
                }
                Logger.InfoFormat("Database has been successfully upgraded to version '{0}'.", targetVersion);
                FreeUpgradeResources();
                return true;
            }
            catch
            {
                try
                {
                    if (upgradeStarted)
                        try
                        {
                            RollbackUpgrade(server, databaseName);
                            Logger.Error("Database restored to original version.");
                            FreeUpgradeResources();
                        }
                        catch (Exception rbex)
                        {
                            Logger.Exception(String.Format("Database rollback failed! Please perform the rollback manually. Database backup file is stored at '{0}'.", backupFilePath), rbex.InnerException);
                        }
                }
                finally
                {
                    server.ConnectionContext.Disconnect();
                }
                throw;
            }
        }

        private void FreeUpgradeResources()
        {
            try
            {
                if (System.IO.File.Exists(backupFilePath))
                    System.IO.File.Delete(backupFilePath);
            }
            catch (Exception ex)
            {
                Logger.Exception(String.Format("Could not delete backup file '{0}'.", backupFilePath), ex);
            }
        }

        string backupFilePath;

        private void PrepareForUpgrade(Server server, String databaseName, String currentVersion)
        {
            if (UpgradeBackupAndRestoreOnError)
            {
                backupFilePath = BackupLocation.TrimEnd('\\') + "\\" + databaseName + String.Format("-{0}", currentVersion) + "-migration" + String.Format("-{0:yyyy-MM-dd hh-mm-ss-fff}", DateTime.Now) + ".bak";

                Logger.InfoFormat("Creating database backup at '{0}'.", backupFilePath);

                Backup bkpDBFull = new Backup();

                bkpDBFull.Action = BackupActionType.Database;
                bkpDBFull.Database = databaseName;

                bkpDBFull.Devices.AddDevice(backupFilePath, DeviceType.File);
                bkpDBFull.BackupSetName = String.Format("{0} database Backup. (Schema version: {1})", databaseName, currentVersion);
                bkpDBFull.BackupSetDescription = String.Format("{0} database - Full Backup. (Schema version: {1})", databaseName, currentVersion);
                bkpDBFull.ExpirationDate = DateTime.Today.AddDays(1);
                bkpDBFull.Initialize = true;
                bkpDBFull.PercentComplete += bkpDBFull_PercentComplete;
                bkpDBFull.Complete += bkpDBFull_Complete;
                bkpDBFull.SqlBackup(server);
            }
            else
                backupFilePath = null;
        }

        void bkpDBFull_Complete(object sender, ServerMessageEventArgs e)
        {
            Logger.Info("Database backup complete.");
        }

        void bkpDBFull_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            Logger.TraceFormat("Database backup {0}% complete.", e.Percent);
        }

        private void RollbackUpgrade(Server server, String databaseName)
        {
            if (backupFilePath != null)
            {
                Logger.InfoFormat("Restoring database from '{0}'.", backupFilePath);

                var db = server.Databases[databaseName];
                db.SetOffline();
                try
                {
                    Restore restoreDB = new Restore();
                    restoreDB.Database = databaseName;
                    /* Specify whether you want to restore database, files or log */
                    restoreDB.Action = RestoreActionType.Database;
                    restoreDB.Devices.AddDevice(backupFilePath, DeviceType.File);

                    restoreDB.ReplaceDatabase = true;
                    restoreDB.NoRecovery = false;

                    restoreDB.PercentComplete += restoreDB_PercentComplete;
                    restoreDB.Complete += restoreDB_Complete;

                    restoreDB.SqlRestore(server);
                }
                finally
                {
                    db.SetOnline();
                }
            }
        }

        void restoreDB_PercentComplete(object sender, PercentCompleteEventArgs e)
        {
            Logger.TraceFormat("Database restore {0}% complete.", e.Percent);
        }

        void restoreDB_Complete(object sender, ServerMessageEventArgs e)
        {
            Logger.Info("Database restored.");
        }
    }

}
