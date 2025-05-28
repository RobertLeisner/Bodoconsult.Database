// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;

namespace Bodoconsult.Database.Ef.Interfaces
{
    /// <summary>
    /// Base interface for a DbContext unit of work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Current app globals with database settings
        /// </summary>
        IAppGlobalsWithDatabase AppGlobals { get; }

        /// <summary>
        ///  Start the migration work flow
        /// </summary>
        void StartMigrationWorkFlow();

        /// <summary>
        /// Get the repository for type TEntity
        /// </summary>
        /// <typeparam name="TEntity">POCO entity type with an integer ID column</typeparam>
        /// <returns>The requested generic repository</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntityRequirements, new();


        /// <summary>
        /// Get the repository for type TEntity
        /// </summary>
        /// <typeparam name="TEntity">POCO entity type</typeparam>
        /// <returns>The requested generic repository with uniqueidentifier ID column</returns>
        IRepositoryGuid<TEntity> GetRepositoryGuid<TEntity>() where TEntity : class, IEntityRequirementsGuid, new();

        /// <summary>
        /// Add a direct access repository
        /// </summary>
        /// <param name="repoName"></param>
        /// <param name="directAccessRepository">Direct access repository to load</param>
        void AddDirectAccessRepository(string repoName, IDirectAccessRepository directAccessRepository);

        /// <summary>
        /// Get an instance of <see cref="IDirectAccessRepository"/>
        /// </summary>
        IDirectAccessRepository GetDirectAccessRepository(string repoName);

        /// <summary>
        /// Run an online backup from the databases
        /// </summary>
        /// <param name="backupFileName">backup file name</param>
        /// <returns>Backup file name with successful, null if the backup failed</returns>
        string RunBackup(string backupFileName);


        /// <summary>
        /// Restore a database from a backup file
        /// </summary>
        /// <param name="backupFileName">Full local file path for the backup</param>
        bool RestoreBackup(string backupFileName);


        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        IContextScope GetContextScope();

        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        IContextScope GetContextScope(bool autoDetectChanges);

        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction);

        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction, IsolationLevel isolationLevel);

        /// <summary>
        /// Get a read only context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        IContextReadOnlyScope GetReadOnlyContextScope();

        /// <summary>
        /// Run a plain SQL statement
        /// </summary>
        /// <param name="sql"></param>
        bool RunSql(string sql);

        /// <summary>
        /// Run plain SQL statement on one open connection. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>true if statement ran successfully</returns>
        bool RunSqlBatch(IEnumerable<string> sql);

        /// <summary>
        /// Get a list with backup file from <see cref="IAppStartParameter.BackupPath"/> ordered by date
        /// </summary>
        /// <returns>List with backup files</returns>
        IList<string> GetBackupFiles();

        /// <summary>
        /// Get a list with backup file orderd by date
        /// </summary>
        /// <param name="backupPath">Path for the backup files. Default: null, means <see cref="IAppStartParameter.BackupPath"/></param>
        /// <returns>List with backup files</returns>
        IList<string> GetBackupFiles(string backupPath);

        /// <summary>
        /// Shrink the database. Takes effect only after backup
        /// </summary>
        void ShrinkDatabase();

    }
}