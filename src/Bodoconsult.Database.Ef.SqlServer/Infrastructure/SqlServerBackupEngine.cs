// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using System.Diagnostics;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.SqlServer.Infrastructure
{
    /// <summary>
    /// Backup engine for LocalDB and SqlServer to backup the database online
    /// </summary>
    public class SqlServerBackupEngine<TContext> : IBackupEngine where TContext : DbContext
    {
        private readonly IAmbientDbContextLocator _contextLocator;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="contextLocator">current data context</param>
        public SqlServerBackupEngine(IAmbientDbContextLocator contextLocator)
        {
            _contextLocator = contextLocator ?? throw new ArgumentNullException(nameof(contextLocator));
        }

        /// <summary>
        /// Connection and command timout
        /// </summary>
        public int ConnectionTimeout { get; set; } = 600;


        /// <summary>
        /// Run a online backup from the database
        /// </summary>
        /// <param name="backupFileName">backup file name</param>
        public void RunBackup(string backupFileName)
        {

            var db = _contextLocator.GetContext<TContext>();

            var sql = "";

            //The Backup TSQL must run on a non-transactional connection. There context.Database.ExecuteSqlCommand(sql) maxy not work.
            // Use the underlying ADO.NET connection of the DbContext
            var conn = (SqlConnection)db.Database.GetDbConnection();   //.ExecuteSqlCommand(sql, "", "", $"{DateTime.Now:YYYYMMddHHmmss}");
            var initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Open(); // open connection if not already open
                }


                var cmdSql = ResourceHelper.GetSqlResource("Execute");

                sql = $"BACKUP DATABASE [{conn.Database}] TO DISK = N'{backupFileName}' WITH  INIT, SKIP, FORMAT,  NOUNLOAD,  NAME = N'Database {conn.Database} full backup {2}'";

                //Debug.WriteLine(sql);



                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = ConnectionTimeout;
                    cmd.CommandText = cmdSql;
                    cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error running SQL: {sql}", ex);
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Close(); // only close connection if not initially open
                }
            }
        }


        // ToDo: Restore
        /// <summary>
        /// Restore a database from a backup file
        /// </summary>
        /// <param name="backupFileName">Full local file path for the backup</param>
        /// <returns>Path to the current backup file</returns>
        /// <remarks>If there should be an error check if datebase option AUTO_UPDATE_STATISTICS_ASYNC is on. If yes turn if off.
        /// See https://docs.microsoft.com/en-us/sql/relational-databases/databases/set-a-database-to-single-user-mode?view=sql-server-ver15</remarks>
        public bool RestoreDatabase(string backupFileName)
        {

            var db = _contextLocator.GetContext<TContext>();

            var connectionString = db.Database.GetDbConnection().ConnectionString;

            var database = GetDatabaseName(connectionString);

            //The Backup TSQL must run on an non-transactional connection. There context.Database.ExecuteSqlCommand(sql) maxy not work.
            // Use a plain ADO.NET connection instead ob DbContext
            using (var conn = new SqlConnection(GetMasterConnectionString(connectionString)))
            {
                var initialState = conn.State;
                try
                {
                    if (initialState != ConnectionState.Open)
                    {
                        conn.Open(); // open connection if not already open
                    }



                    var cmdSql = ResourceHelper.GetSqlResource("Execute");

                    // Set database to single user mode
                    var sql = $"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = ConnectionTimeout;
                        cmd.CommandText = cmdSql;
                        cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = sql;
                        cmd.ExecuteNonQuery();
                    }

                    // Restore
                    sql = $"RESTORE DATABASE [{database}] FROM DISK = N'{backupFileName}'";

                    Debug.Print(sql);

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = ConnectionTimeout;
                        cmd.CommandText = cmdSql;
                        cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = sql;
                        cmd.ExecuteNonQuery();
                    }

                    // Set back to multiuser
                    sql = $"ALTER DATABASE [{database}] SET MULTI_USER;";

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = ConnectionTimeout;
                        cmd.CommandText = cmdSql;
                        cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = sql;
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }

                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    return false;
                }

            }
        }




        private static string GetDatabaseName(string connectionString)
        {
            var i = connectionString.IndexOf("Initial Catalog=", StringComparison.OrdinalIgnoreCase);

            var j = connectionString.IndexOf(";", i, StringComparison.OrdinalIgnoreCase);

            var result = connectionString.Substring(i + 16, j - i - 16);

            return result;
        }

        private static string GetMasterConnectionString(string connectionString)
        {
            var i = connectionString.IndexOf("Initial Catalog=", StringComparison.OrdinalIgnoreCase);

            var j = connectionString.IndexOf(";", i, StringComparison.OrdinalIgnoreCase);

            var result = connectionString[..(i + 16)] + "Master" + connectionString[j..];

            return result;
        }
    }
}
