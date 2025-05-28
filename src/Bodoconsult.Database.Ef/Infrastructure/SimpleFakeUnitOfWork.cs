// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.Infrastructure;

/// <summary>
/// A very simple fake impl of <see cref="IUnitOfWork"/>
/// </summary>
public class SimpleFakeUnitOfWork : IUnitOfWork
{
    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Current app globals with database settings
    /// </summary>
    public IAppGlobalsWithDatabase AppGlobals { get; set; }

    /// <summary>
    ///  Start the migration work flow
    /// </summary>
    public void StartMigrationWorkFlow()
    {
        // Do nothing
    }

    /// <summary>
    /// Get the repository for type TEntity
    /// </summary>
    /// <typeparam name="TEntity">POCO entity type with an integer ID column</typeparam>
    /// <returns>The requested generic repository</returns>
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntityRequirements, new()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the repository for type TEntity
    /// </summary>
    /// <typeparam name="TEntity">POCO entity type</typeparam>
    /// <returns>The requested generic repository with uniqueidentifier ID column</returns>
    public IRepositoryGuid<TEntity> GetRepositoryGuid<TEntity>() where TEntity : class, IEntityRequirementsGuid, new()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Add a direct access repository
    /// </summary>
    /// <param name="repoName"></param>
    /// <param name="directAccessRepository">Direct access repository to load</param>
    public void AddDirectAccessRepository(string repoName, IDirectAccessRepository directAccessRepository)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get an instance of <see cref="IDirectAccessRepository"/>
    /// </summary>
    public IDirectAccessRepository GetDirectAccessRepository(string repoName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Run an online backup from the databases
    /// </summary>
    /// <param name="backupFileName">backup file name</param>
    /// <returns>Backup file name with successful, null if the backup failed</returns>
    public string RunBackup(string backupFileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Restore a database from a backup file
    /// </summary>
    /// <param name="backupFileName">Full local file path for the backup</param>
    public bool RestoreBackup(string backupFileName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope(bool autoDetectChanges)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction, IsolationLevel isolationLevel)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a read only context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextReadOnlyScope GetReadOnlyContextScope()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Run a plain SQL statement
    /// </summary>
    /// <param name="sql"></param>
    public bool RunSql(string sql)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Run plain SQL statement on one open connection. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
    /// </summary>
    /// <param name="sql">SQL statement</param>
    /// <returns>true if statement ran successfully</returns>
    public bool RunSqlBatch(IEnumerable<string> sql)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a list with backup file from <see cref="IAppStartParameter.BackupPath"/> ordered by date
    /// </summary>
    /// <returns>List with backup files</returns>
    public IList<string> GetBackupFiles()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get a list with backup file orderd by date
    /// </summary>
    /// <param name="backupPath">Path for the backup files. Default: null, means <see cref="IAppStartParameter.BackupPath"/></param>
    /// <returns>List with backup files</returns>
    public IList<string> GetBackupFiles(string backupPath)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Shrink the database. Takes effect only after backup
    /// </summary>
    public void ShrinkDatabase()
    {
        throw new NotImplementedException();
    }
}