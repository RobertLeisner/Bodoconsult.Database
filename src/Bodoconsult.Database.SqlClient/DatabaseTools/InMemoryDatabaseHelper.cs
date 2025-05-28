// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.SqlClient.DatabaseTools
{
    /// <summary>
    /// In-memory database helper for testing purposes
    /// </summary>
    public sealed class InMemoryDatabaseHelper : IDatabaseHelper
    {
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            DatabaseName = "InMemoryDb";
        }

        /// <summary>
        /// Connection string to database
        /// </summary>
        public string ConnectionString => DatabaseName;

        /// <summary>
        /// Connection string to Master database
        /// </summary>
        public string MasterConnectionString { get; set; }

        /// <summary>
        /// Name of the current database
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Check if the connection to MASTER database may be opened
        /// </summary>
        /// <returns>true if connection is successful</returns>
        public bool CheckConnection()
        {
            return true;
        }

        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        public bool BackupDatabase(string fileName)
        {
            return true;
        }

        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        /// <param name="databaseName"></param>
        public bool BackupDatabase(string fileName, string databaseName)
        {
            return true;
        }

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database exists</returns>
        public bool DatabaseExists()
        {
            return true;
        }

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database exists</returns>
        public bool DatabaseExists(string databaseName)
        {
            return true;
        }

        /// <summary>
        /// Drop the current database
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        public bool DropDatabase()
        {
            return true;
        }

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        public bool DropDatabase(string databaseName)
        {
            return true;
        }

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <returns>true if the backup was restored successfully</returns>
        public bool? RestoreDatabase(string backupFile)
        {
            return true;
        }

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <param name="databaseName">Name of the database to restore in</param>
        /// <returns>true if the backup was restored successfully</returns>
        public bool? RestoreDatabase(string backupFile, string databaseName)
        {
            return true;
        }

        /// <summary>
        /// Create a empty database
        /// </summary>
        public void CreateEmptyDatabase()
        {
            // Not applicabale for InMemory database
        }

        /// <summary>
        /// Check if a certain sql statement delivers data
        /// </summary>
        /// <param name="sql">Statement delivering records, ie.e. SELECT * FROM TTower</param>
        /// <returns>true if there are rows, false if not</returns>
        public bool CheckHasData(string sql)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Shrink the current database
        /// </summary>
        public void ShrinkDatabase()
        {
            // Do nothing
        }
    }
}