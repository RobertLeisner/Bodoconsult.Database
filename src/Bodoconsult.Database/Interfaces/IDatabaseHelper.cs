// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;

namespace Bodoconsult.Database.Interfaces
{
    /// <summary>
    /// Database helper provides not EF based database functionality
    /// </summary>
    public interface IDatabaseHelper : IDisposable
    {

        /// <summary>
        /// Connection string to database
        /// </summary>
        string ConnectionString { get;  }


        /// <summary>
        /// Connection string to Master database
        /// </summary>
        string MasterConnectionString { get; set; }

        /// <summary>
        /// Name of the current database
        /// </summary>
        string DatabaseName { get; set; }


        /// <summary>
        /// Check if the connection to MASTER database may be opened
        /// </summary>
        /// <returns>true if connection is successful</returns>
        bool CheckConnection();

        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        // <returns>true if the database backup was succesful</returns>
        bool BackupDatabase(string fileName);

        /// <summary>
        /// Backup the database to a file
        /// </summary>
        /// <param name="fileName">full path for the backup file</param>
        /// <param name="databaseName"></param>
        // <returns>true if the database backup was succesful</returns>
        bool BackupDatabase(string fileName, string databaseName);


        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database exists</returns>
        bool DatabaseExists();

        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database exists</returns>
        bool DatabaseExists(string databaseName);


        /// <summary>
        /// Drop the current database
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        bool DropDatabase();


        /// <summary>
        /// Exists the database already?
        /// </summary>
        /// <returns>true if the database does not exist anymore</returns>
        bool DropDatabase(string databaseName );

        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <returns>true if the backup was restored successfully</returns>
        bool? RestoreDatabase(string backupFile);


        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <param name="databaseName">Name of the database to restore in</param>
        /// <returns>true if the backup was restored successfully</returns>
        bool? RestoreDatabase(string backupFile, string databaseName);


        /// <summary>
        /// Create a empty database
        /// </summary>
        void CreateEmptyDatabase();

        /// <summary>
        /// Check if a certain sql statement delivers data
        /// </summary>
        /// <param name="sql">Statement delivering records, ie.e. SELECT * FROM TTower</param>
        /// <returns>true if there are rows, false if not</returns>
        bool CheckHasData(string sql);

        /// <summary>
        /// Shrink the current database
        /// </summary>
        void ShrinkDatabase();
    }
}