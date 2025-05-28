// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.SqlClient.Helpers;
using Microsoft.Data.SqlClient;

namespace Bodoconsult.Database.SqlClient.DatabaseTools
{


    /// <summary>
    /// Basic tools for handling databases
    /// </summary>
    public class BaseSqlDatabaseHelper : IDatabaseHelper
    {

        #region Private variables

        private string _databasePath;

        // Internal constants with resource or parameter names
        private const string ExecuteTemplate = "Execute";
        private const string DataParameterName = "@Data";

        // Internal constants with length values
        private const int MaxFilePathLength = 900;
        private const int DefaultStringLength = 255;


        private readonly string _executeSql = ResourceHelper.GetSqlResource(ExecuteTemplate);

        /// <summary>
        /// Current master connection
        /// </summary>
        internal SqlConnection DatabaseConnection { get; set; }


        #endregion


        #region Public properties

        /// <summary>
        /// Connection and command timout
        /// </summary>
        public int ConnectionTimeout { get; set; } = 600;

        public string ConnectionString { get; }

        /// <summary>
        /// Connection string to Master database
        /// </summary>
        public string MasterConnectionString { get; set; }

        /// <summary>
        /// Name of the current database
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Path, the database files should be saved to
        /// </summary>
        public string DatabasePath
        {
            get => _databasePath;
            set
            {
                _databasePath = value;
                if (!Directory.Exists(_databasePath))
                {
                    Directory.CreateDirectory(_databasePath);
                }
            }
        }

        /// <summary>
        /// Current master connection
        /// </summary>
        internal SqlConnection MasterConnection { get; set; }

        #endregion


        #region Private methods

        private void GetDatabaseConnection()
        {
            if (DatabaseConnection == null)
            {
                DatabaseConnection = new SqlConnection(ConnectionString);
                DatabaseConnection.Open();
            }
        }





        /// <summary>
        /// Get the connection string to the Master database
        /// </summary>
        private bool GetMasterConnection()
        {

            if (MasterConnection != null)
            {
                return MasterConnection.State == ConnectionState.Open;
            }

            try
            {
                var raw = ConnectionString;

                var i = raw.ToUpperInvariant().IndexOf("INITIAL CATALOG=", StringComparison.Ordinal);

                var j = raw.ToUpperInvariant().IndexOf(";", i + 1, StringComparison.Ordinal);

                DatabaseName = raw.Substring(i + 16, j - i - 16);

                raw = raw.Substring(0, i + 16) + "Master" + raw.Substring(j);

                MasterConnectionString = raw;

                MasterConnection = new SqlConnection(MasterConnectionString);
                MasterConnection.Open();

                return MasterConnection.State == ConnectionState.Open;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(ConnectionString, e);

            }
        }

        #endregion

        #region Ctors

        /// <summary>
        /// Ctor with customized connection string
        /// </summary>
        /// <param name="nameOrConnectionString">connection string or name of a connection string</param>
        public BaseSqlDatabaseHelper(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
            {
                throw new ArgumentNullException(nameof(nameOrConnectionString));
            }

            if (nameOrConnectionString.ToUpperInvariant().StartsWith("NAME=", StringComparison.InvariantCulture))
            {
                throw new NotSupportedException("Connection string starting with NAME= are support by StSysV3");
            }

            ConnectionString = nameOrConnectionString;
            GetMasterConnection();

        }

        /// <summary>
        /// Default ctor: loads StSysEntities connection string
        /// </summary>
        public BaseSqlDatabaseHelper()
        {
            throw new NotSupportedException();
        }

        #endregion


        #region Public methods

        /// <summary>
        /// Check on SQL level whether a table contains rows
        /// </summary>
        /// <param name="tableName">Name of the table without schema (dbo.)</param>
        /// <returns></returns>
        public bool Any(string tableName)
        {
            GetDatabaseConnection();

            var sql = $"if exists (select * from dbo.{tableName}) select 1 as erg else select 0 as erg";

            var cmdSql = ResourceHelper.GetSqlResource(ExecuteTemplate);

#pragma warning disable CA2100
            using (var cmd = new SqlCommand(cmdSql, DatabaseConnection))
            {
                cmd.CommandTimeout = ConnectionTimeout;
                cmd.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;

                var erg = (int)cmd.ExecuteScalar();

                var result = erg == 1;

                return result;
            }
#pragma warning restore CA2100

        }

        /// <summary>
        /// Count on SQL level the number of rows a table contains
        /// </summary>
        /// <param name="tableName">Name of the table without schema (dbo.)</param>
        /// <returns>row number</returns>
        public int Count(string tableName)
        {
            GetDatabaseConnection();

            var sql = $"select count(*) from dbo.{tableName}";

            var cmdSql = ResourceHelper.GetSqlResource(ExecuteTemplate);

#pragma warning disable CA2100

            using (var cmd = new SqlCommand(cmdSql, DatabaseConnection))
            {
                cmd.CommandTimeout = ConnectionTimeout;
                cmd.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;

                var result = (int)cmd.ExecuteScalar();

                return result;
            }

#pragma warning restore CA2100

        }

        /// <summary>
        /// Execute a SQL statement and return a scalar value
        /// </summary>
        /// <param name="sql">SQL statement return one row tih one column</param>
        /// <returns>row number</returns>
        public object ExecuteScalar(string sql)
        {
            GetDatabaseConnection();

            var cmdSql = ResourceHelper.GetSqlResource(ExecuteTemplate);

#pragma warning disable CA2100

            using (var cmd = new SqlCommand(cmdSql, DatabaseConnection))
            {
                cmd.CommandTimeout = ConnectionTimeout;
                cmd.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;

                var result = cmd.ExecuteScalar();

                return result;
            }

#pragma warning restore CA2100
        }


        #endregion


        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        /// <returns>true if the database backup was succesful</returns>
        public virtual bool BackupDatabase(string fileName)
        {
            return BackupDatabaseIntern(fileName);
        }

        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        /// <param name="databaseName"></param>
        /// <returns>true if the database backup was succesful</returns>
        public virtual bool BackupDatabase(string fileName, string databaseName)
        {
            return BackupDatabaseIntern(fileName, databaseName);
        }


        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        /// <param name="databaseName"></param>
        // <returns>true if the database backup was succesful</returns>
        private bool BackupDatabaseIntern(string fileName, string databaseName = null)
        {

            try
            {
                if (string.IsNullOrEmpty(databaseName))
                {
                    databaseName = DatabaseName;
                }

                var sql = ResourceHelper.GetSqlResource("Backup");

                using (var cmd = new SqlCommand(sql, MasterConnection))
                {
                    cmd.CommandTimeout = ConnectionTimeout;
                    cmd.Parameters.Add("@database", SqlDbType.NVarChar, DefaultStringLength).Value = databaseName;
                    cmd.Parameters.Add("@backup_set_name", SqlDbType.NVarChar, DefaultStringLength).Value = $"Database {databaseName} full backup {DateTime.Now:yyyyMMddHHmmss}";
                    cmd.Parameters.Add("@outputfile", SqlDbType.NVarChar, MaxFilePathLength).Value = fileName;

                    cmd.ExecuteNonQuery();
                }

                return true;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException(fileName, e);
            }
        }

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database exists</returns>
        public bool DatabaseExists()
        {
            return DatabaseExistsIntern(null);
        }

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <param name="databaseName">Datebase name to check existence</param>
        /// <returns>true if the database exists</returns>
        public bool DatabaseExists(string databaseName)
        {
            return DatabaseExistsIntern(databaseName);
        }


        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database exists</returns>
        private bool DatabaseExistsIntern(string databaseName = null)
        {

            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = DatabaseName;
            }

            GetMasterConnection();

            var sql = $"IF EXISTS(SELECT * FROM master.sys.sysdatabases WHERE name='{databaseName}') BEGIN SELECT cast(1 as bit) as RetValue END else begin SELECT cast(0 as bit) as RetValue end";

            // Run the command
            return RunScalar(sql);
        }

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        public bool DropDatabase()
        {
            return DropDatabaseIntern(DatabaseName);
        }


        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        public bool DropDatabase(string databaseName)
        {
            return DropDatabaseIntern(databaseName);
        }



        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        private bool DropDatabaseIntern(string databaseName)
        {
            try
            {
                if (string.IsNullOrEmpty(databaseName))
                {
                    databaseName = DatabaseName;
                }

                if (string.Equals("Master", DatabaseName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (!DatabaseExists(databaseName))
                {
                    return true;
                }

                GetMasterConnection();

                var files = GetDbFiles();



#pragma warning disable CA2100

                var sql = ResourceHelper.GetSqlResource("DropDatabase");

                try
                {
                    using (var cmd = new SqlCommand(sql, MasterConnection))
                    {
                        cmd.CommandTimeout = ConnectionTimeout;
                        cmd.Parameters.Add("@database", SqlDbType.NVarChar, DefaultStringLength).Value = databaseName;
                        cmd.ExecuteNonQuery();
                    }


                }
                catch (Exception e)
                {
                    Console.Write($"Drop database failed: {e.Message}");
                    //throw new InvalidOperationException("Drop database does not exist", e);
                    return false;
                }

                foreach (var file in files)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // Do nothing
                    }
                }

                return true;

#pragma warning restore CA2100

            }
#pragma warning disable CA1031
            catch // (Exception e)
            {
                //throw new InvalidOperationException(sql, e);
                return true;
            }
#pragma warning restore CA1031
        }


        /// <summary>
        /// Get all physical files of a database
        /// </summary>
        /// <returns>List with file paths of all physical files of a database</returns>
        public IList<string> GetDbFiles()
        {
            var sql = ResourceHelper.GetSqlResource("GetDbFiles");

            var filelistCmd = new SqlCommand(sql, MasterConnection);
            filelistCmd.Parameters.Add("@database", SqlDbType.NVarChar, 255).Value = DatabaseName;

            var reader = filelistCmd.ExecuteReader();
            var dt = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            dt.Load(reader);
            reader.Close();
            reader.Dispose();
            filelistCmd.Dispose();

            var result = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                var fileName = row["physical_name"].ToString();
                result.Add(fileName);
            }

            dt.Dispose();

            return result;
        }




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            try
            {
                DatabaseConnection?.Dispose();
                MasterConnection?.Dispose();
            }
#pragma warning disable CA1031
            catch //(Exception e)
            {
                // ignored
            }
#pragma warning restore CA1031

        }

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <returns>true if the backup was restored successfully</returns>
        public virtual bool? RestoreDatabase(string backupFile)
        {
            return RestoreDatabaseIntern(backupFile, DatabaseName);
        }

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <param name="databaseName">Name of the database to restore in</param>
        /// <returns>true if the backup was restored successfully</returns>
        public virtual bool? RestoreDatabase(string backupFile, string databaseName)
        {
            return RestoreDatabaseIntern(backupFile, databaseName);
        }

        private bool? RestoreDatabaseIntern(string backupFile, string databaseName)
        {
            var state = "";

            try
            {

                // Get database files first 
                GetMasterConnection();

                state = $"Database exists: {DatabaseExists(databaseName)} ";

                var sql = ResourceHelper.GetSqlResource("Restore");

#pragma warning disable CA2100
                var filelistCmd = new SqlCommand(sql, MasterConnection);
                filelistCmd.Parameters.Add("@filename", SqlDbType.NVarChar, MaxFilePathLength).Value = backupFile;
#pragma warning restore CA2100

                var reader = filelistCmd.ExecuteReader();
                var dt = new DataTable
                {
                    Locale = CultureInfo.InvariantCulture
                };
                dt.Load(reader);
                reader.Close();
                reader.Dispose();

                filelistCmd.Dispose();

                var move = " WITH";

                foreach (DataRow row in dt.Rows)
                {

                    var type = row["Type"].ToString();

                    var ext = type == "D" ? ".mdf" : "_log.ldf";

                    var path = Path.Combine(DatabasePath, databaseName + ext);

                    move += $" MOVE N'{row["LogicalName"]}' TO N'{path}',";

                    state += $"File {path} exists: {File.Exists(path)} ";

                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(path);
                    }
#pragma warning disable CA1031
                    catch (Exception e)
                    {
                        state += $"File {path} deleting failed: {e.Message} ";
                    }
#pragma warning restore CA1031



                }

                dt.Dispose();

                if (move.EndsWith(",", StringComparison.CurrentCulture))
                {
                    move = move[..^1];
                }

                // Create restore command with moving database files
                sql = $"RESTORE DATABASE {databaseName} FROM DISK = N'{backupFile}'{move}";

                Debug.WriteLine(sql);

                // Run the command
                return Execute(sql);


            }
            catch (Exception e)
            {
                throw new DatabaseHelperException($"Restore {databaseName} database from {backupFile} failed: {state}", e);
            }
        }

        /// <summary>
        /// Create a empty database
        /// </summary>
        public void CreateEmptyDatabase()
        {
            var sql = $"CREATE DATABASE [{DatabaseName}];";

            Execute(sql);
        }



        /// <summary>
        /// Shrink the current database
        /// </summary>
        public void ShrinkDatabase()
        {
            try
            {
                var sql = $"DBCC SHRINKDATABASE ({DatabaseName}, 10);";
                Execute(sql);
            }
            catch (Exception e)
            {
                Debug.Print($"ShrinkDatabase: {e.Message}");
            }

        }


        /// <summary>
        /// Run a SQL statement on the database. Do not use with SQL generated from user input!!! May be used by SQL Injection.
        /// </summary>
        /// <param name="sql">Non querying SQL statement</param>
        public void RunSql(string sql)
        {
            try
            {
                GetDatabaseConnection();

#pragma warning disable CA2100
                using (var command = new SqlCommand(_executeSql, DatabaseConnection))
                {
                    command.CommandTimeout = ConnectionTimeout;
                    command.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;
                    command.ExecuteNonQuery();
                }
#pragma warning restore CA2100
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(sql, e);
            }
        }

        /// <summary>
        /// Check if the connection to MASTER database may be opened
        /// </summary>
        /// <returns>true if connection is successful</returns>
        public bool CheckConnection()
        {
            return GetMasterConnection();
        }

        /// <summary>
        /// Run a SQL statement. 
        /// </summary>
        /// <remarks>Do not use with user generated SQL due to danger of SQL Injection</remarks>
        /// <param name="sql">SQL statement</param>
        /// <returns></returns>
        internal bool RunScalar(string sql)
        {

            try
            {
                GetMasterConnection();

#pragma warning disable CA2100
                using (var cmd = new SqlCommand(_executeSql, MasterConnection))
                {
                    cmd.CommandTimeout = ConnectionTimeout;
                    cmd.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;
                    var value = cmd.ExecuteScalar();
                    var val = value != null ? value.ToString() : "0";
                    return Convert.ToBoolean(val, CultureInfo.InvariantCulture);
                }
#pragma warning restore CA2100
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(sql, e);
            }
        }

        /// <summary>
        /// Execute a SQL statement against the database
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>true if there is no error</returns>
        internal bool Execute(string sql)
        {
            try
            {
                GetMasterConnection();

#pragma warning disable CA2100
                using (var command = new SqlCommand(_executeSql, MasterConnection))
                {
                    command.CommandTimeout = ConnectionTimeout;
                    command.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;
                    command.ExecuteNonQuery();
                }
#pragma warning restore CA2100
                return true;
            }
            catch (Exception e)
            {
                throw new DatabaseHelperException(sql, e);
            }
        }


        /// <summary>
        /// Check if a table exists in the database
        /// </summary>
        /// <param name="schema">Schema</param>
        /// <param name="tableName">Table name without brackets</param>
        /// <returns></returns>
        public bool TableExists(string schema, string tableName)
        {
            var sql = "SET NOCOUNT ON IF (EXISTS (SELECT * " +
                      "FROM INFORMATION_SCHEMA.TABLES " +
                      $" WHERE TABLE_SCHEMA = '{schema}' " +
                      $" AND TABLE_NAME = '{tableName}')) " +
                      "select cast(1 as bit) as result else select cast(0 as bit) as result";

            try
            {
                GetDatabaseConnection();

#pragma warning disable CA2100
                using (var command = new SqlCommand(_executeSql, DatabaseConnection))
                {
                    command.CommandTimeout = ConnectionTimeout;
                    command.Parameters.Add(DataParameterName, SqlDbType.NVarChar).Value = sql;

                    var result = command.ExecuteScalar();

                    return (bool)result;
                }
#pragma warning restore CA2100
            }
#pragma warning disable CA1031
            catch
            {
                return false;
            }
#pragma warning restore CA1031
        }

        /// <summary>
        /// Check if a certain sql statement delivers data
        /// </summary>
        /// <param name="sql">Statement delivering records, ie.e. SELECT * FROM TTower</param>
        /// <returns>true if there are rows, false if not</returns>
        public virtual bool CheckHasData(string sql)
        {

            string result;

            GetDatabaseConnection();

            sql = $"if exists ({sql}) select cast(1 as bit) as [Result] else select cast(0 as bit) as [Result]";

#pragma warning disable CA2100
            using (var filelistCmd = new SqlCommand(_executeSql, DatabaseConnection))
            {
                filelistCmd.Parameters.Add("@Data", SqlDbType.NVarChar, 10000).Value = sql;

                using (var reader = filelistCmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return false;
                    }

                    reader.Read();

                    result = reader["Result"].ToString();

                    reader.Close();
                }
            }

            if (string.IsNullOrEmpty(result))
            {
                return false;
            }

            return result != "False";
        }

#pragma warning restore CA2100

    }
}
