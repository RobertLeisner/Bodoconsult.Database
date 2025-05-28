// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using System.Diagnostics;
using Bodoconsult.Database.Ef.SqlServer.Helpers;
using Microsoft.Data.SqlClient;

namespace Bodoconsult.Database.Ef.SqlServer.DatabaseTools
{
    /// <summary>
    /// Tools for handling remote databases (only SqlServer Express and SqlServer)
    /// </summary>
    /// <remarks>To use <see cref="RemoteSqlDatabaseHelper"/> you should have a local folder on
    /// the database server which is shared to the network. </remarks>
    public class RemoteSqlDatabaseHelper : BaseSqlDatabaseHelper
    {
        /// <summary>
        /// Default ctor: loads StSysEntities connection string
        /// </summary>
        public RemoteSqlDatabaseHelper()
        {

        }

        /// <summary>
        /// Ctor with customized connection string
        /// </summary>
        /// <param name="nameOrConnectionString">connection string or name of a connection string</param>
        public RemoteSqlDatabaseHelper(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }



        /// <summary>
        /// The remote path seen from the network to store the backup files on for database restore.
        /// You should have a local folder on
        /// the database server which is shared to the network.
        /// </summary>
        public string RemoteNetworkPath { get; set; }


        /// <summary>
        /// The remote path to store the backup files on for database restore seen locally from remote system.
        /// You should have a local folder on
        /// the database server which is shared to the network.
        /// </summary>
        public string RemoteLocalPath { get; set; }

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <returns>true if the backup was restored successfully</returns>re
        public override bool? RestoreDatabase(string backupFile)
        {

            return RestoreDatabaseIntern(backupFile, DatabaseName);

        }


        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <param name="databaseName">Name of the database to restore in</param>
        /// <returns>true if the backup was restored successfully</returns>re
        public override bool? RestoreDatabase(string backupFile, string databaseName)
        {

            return RestoreDatabaseIntern(backupFile, databaseName);

        }

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <param name="databaseName">Name of the database to restore in</param>
        /// <returns>true if the backup was restored successfully</returns>re
        private bool? RestoreDatabaseIntern(string backupFile, string databaseName)
        {
            try
            {

                var dbName = databaseName ?? DatabaseName;

                // Copy backup file to remote folder
                var remoteFile = Path.Combine(RemoteNetworkPath, new FileInfo(backupFile).Name);

                if (File.Exists(remoteFile))
                {
                    File.Delete(remoteFile);
                }
                File.Copy(backupFile, remoteFile);

                var remoteBackupFile = Path.Combine(RemoteLocalPath, new FileInfo(backupFile).Name);


                var dataPath = DatabasePath;

                // Get database files first 
                //var sql = $"RESTORE FILELISTONLY FROM DISK = '{remoteBackupFile}'";

                var sql = ResourceHelper.GetSqlResource("Restore");

                var filelistCmd = new SqlCommand(sql, MasterConnection);
                filelistCmd.Parameters.Add("@filename", SqlDbType.NVarChar, 900).Value = remoteBackupFile;


                var reader = filelistCmd.ExecuteReader();
                var dt = new DataTable();
                dt.Load(reader);
                reader.Close();
                filelistCmd.Dispose();

                var move = " WITH";

                foreach (DataRow row in dt.Rows)
                {

                    var type = row["Type"].ToString();

                    var ext = type == "D" ? ".mdf" : ".ldf";

                    var path = Path.Combine(dataPath, dbName + ext);

                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch (Exception e)
                        {
                            throw new InvalidOperationException(sql, e);
                        }
                    }


                    move += $" MOVE N'{row["LogicalName"]}' TO N'{path}',";

                }

                dt.Dispose();

                if (move.EndsWith(",", StringComparison.InvariantCulture))
                {
                    move = move.Substring(0, move.Length - 1);
                }

                // Create restore command with moving database files
                sql = $"RESTORE DATABASE {dbName} FROM DISK = N'{remoteBackupFile}'" + move;

                Debug.WriteLine(sql);

                // Run the command
                return Execute(sql);

            }
            catch (Exception e)
            {
                throw new InvalidOperationException(backupFile, e);
            }
        }
    }
}