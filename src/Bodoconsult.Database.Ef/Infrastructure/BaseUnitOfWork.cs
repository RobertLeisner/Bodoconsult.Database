// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Enums;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bodoconsult.Database.Ef.Infrastructure;

/// <summary>
/// Base implementation of a unit of work for EF DbContexts and repositories
/// </summary>
public class BaseUnitOfWork<T> : IUnitOfWork where T : DbContext
{
    private readonly IContextScopeFactory<T> _dbContextScopeFactory;

    protected readonly Dictionary<Type, object> Repositories;

    protected readonly Dictionary<Type, object> RepositoriesGuid;

    protected readonly IAppLoggerProxy Log;

    protected readonly IAmbientDbContextLocator AmbientDbContextLocator;

    private readonly IBackupEngine _backupEngine;

    private readonly IMigrationController _migrationController;



    private readonly Dictionary<string, IDirectAccessRepository> _directAccessRepositories = new();

    /// <summary>
    /// Ctor with additional backup engine
    /// </summary>
    /// <param name="dbContextScopeFactory"></param>
    /// <param name="logger"></param>
    /// <param name="ambientDbContextLocator"></param>
    /// <param name="backupEngine"></param>
    /// <param name="migrationController">Current migration controller</param>
    public BaseUnitOfWork(IContextScopeFactory<T> dbContextScopeFactory, IAppLoggerProxy logger, IAmbientDbContextLocator ambientDbContextLocator, IBackupEngine backupEngine, IMigrationController migrationController)
    {
        _dbContextScopeFactory = dbContextScopeFactory;
        AmbientDbContextLocator = ambientDbContextLocator;
        AppGlobals = dbContextScopeFactory.AppGlobals;

        Log = logger ?? throw new ArgumentNullException(nameof(logger));
        Log.LogWarning(AppGlobals.ContextConfig.ConnectionString);

        ambientDbContextLocator ??= new AmbientDbContextLocator();
        AmbientDbContextLocator = ambientDbContextLocator;
        _migrationController = migrationController;
        _migrationController.LoadUnitOfWork(this);

        Repositories = new Dictionary<Type, object>();

        RepositoriesGuid = new Dictionary<Type, object>();

        _backupEngine = backupEngine;

        StartMigrationWorkFlow();
    }


    public void StartMigrationWorkFlow()
    {
        if (_migrationController == null)
        {
            return;
        }

        Log.LogInformation(AppGlobals.ContextConfig.ConnectionString);

        // Basics
        if (_migrationController.IsNewDatabase)
        {

            // Migrate empty database to current schema
            if (!AppGlobals.ContextConfig.TurnOffMigrations)
            {
                _migrationController.MigrateDatabase();
            }
        }
        else
        {
            if (!AppGlobals.ContextConfig.TurnOffMigrations)
            {
                // Take a backup from the database if required
                _migrationController.SaveDatabase();

                // Migrate database to current schema
                _migrationController.MigrateDatabase();
            }
        }

        // Apply data changes to database
        if (!ContextConfig.TurnOffConverters)
        {
            _migrationController.ApplyDatabaseUpdates();
        }
    }

    /// <summary>
    /// Number of backups kept additonally to the current one. Default: 30
    /// </summary>
    public int NumberOfBackups { get; set; } = 30;

    /// <summary>
    /// Get the generic repository for type TEntity
    /// </summary>
    /// <typeparam name="TEntity">POCO entity type</typeparam>
    /// <returns>The request generic repository</returns>
    public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntityRequirements, new()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Get the repository for type TEntity
    /// </summary>
    /// <typeparam name="TEntity">POCO entity type</typeparam>
    /// <returns>The requested generic repository with uniqueidentifier ID column</returns>
    public virtual IRepositoryGuid<TEntity> GetRepositoryGuid<TEntity>() where TEntity : class, IEntityRequirementsGuid, new()
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Add a direct access repository
    /// </summary>
    /// <param name="repoName"></param>
    /// <param name="directAccessRepository">Direct access repository to load</param>
    public void AddDirectAccessRepository(string repoName, IDirectAccessRepository directAccessRepository)
    {
        _directAccessRepositories.Add(repoName, directAccessRepository);
    }

    /// <summary>
    /// Get an instance of <see cref="IDirectAccessRepository"/>
    /// </summary>
    public IDirectAccessRepository GetDirectAccessRepository(string repoName)
    {
        var success = _directAccessRepositories.TryGetValue(repoName, out var result);
        return success ? result : null;
    }

    /// <summary>
    /// Current app globals with database settings
    /// </summary>
    public IAppGlobalsWithDatabase AppGlobals { get; }

    /// <summary>
    /// Current context config
    /// </summary>
    public IContextConfig ContextConfig => AppGlobals.ContextConfig;

    /// <summary>
    /// Shrink the database. Takes effect only after backup
    /// </summary>
    public virtual void ShrinkDatabase()
    {
        throw new NotSupportedException();
    }



    #region Backup

       
    /// <summary>
    /// Run a online backup from the database
    /// </summary>
    /// <param name="backupFileName">backup file name</param>
    /// <returns>Backup file name with successful, null if the backup failed</returns>
    public string RunBackup(string backupFileName)
    {
        try
        {
            if (string.IsNullOrEmpty(backupFileName))
            {

                // Check if path is existing
                if (!Directory.Exists(AppGlobals.AppStartParameter.BackupPath))
                {
                    Directory.CreateDirectory(AppGlobals.AppStartParameter.BackupPath);
                }

                // Check number of backups and clean backup folder if necessary
                var files = new DirectoryInfo(AppGlobals.AppStartParameter.BackupPath).GetFiles("*.bak").OrderByDescending(x => x.Name).ToList();

                if (files.Count > NumberOfBackups)
                {
                    var mustDelete = files.Count - NumberOfBackups;

                    for (var i = mustDelete; i > 0; i--)
                    {
                        files[^i].Delete();
                    }
                }

                // Set backup file name
                backupFileName = GetBackupFileName(DateTime.Now);

            }

            Log.LogInformation($"BACKUP database to {backupFileName} started...");

            using (GetReadOnlyContextScope())
            {
                _backupEngine?.RunBackup(backupFileName);
            }

            Log.LogInformation($"BACKUP database done!");

            return backupFileName;
        }
        catch (Exception e)
        {
            Log.Log(LogLevel.Error, e, $"Backup to {backupFileName}");
            return null;
        }


    }

    public bool RestoreBackup(string backupFileName)
    {
        bool result;

        try
        {
            using (GetReadOnlyContextScope())
            {
                result = _backupEngine.RestoreDatabase(backupFileName);
            }
        }

        catch (Exception e)
        {
            Log.Log(LogLevel.Error, e, $"Restore from {backupFileName} failed");
            result = false;
        }

        return result;
    }

    /// <summary>
    /// Get a default backup file name SMDTower_yyyyMMddHHmmss.bak for the requested date
    /// </summary>
    /// <param name="dateTime">Date for the timestamp</param>
    /// <returns></returns>
    public string GetBackupFileName(DateTime dateTime)
    {
        var s = $"SMDTower_{dateTime:yyyyMMddHHmmss}.bak";

        return Path.Combine(AppGlobals.AppStartParameter.BackupPath, s);
    }


    /// <summary>
    /// Get a list with backup file from <see cref="IAppStartParameter.BackupPath"/> ordered by date
    /// </summary>
    /// <returns>List with backup files</returns>
    public IList<string> GetBackupFiles()
    {
        return GetBackupFilesIntern(AppGlobals.AppStartParameter.BackupPath);
    }


    /// <summary>
    /// Get a list with backup file orderd by date
    /// </summary>
    /// <param name="backupPath">Path for the backup files. Null means <see cref="IAppStartParameter.BackupPath"/></param>
    /// <returns>List with backup files</returns>
    public IList<string> GetBackupFiles(string backupPath)
    {
        return GetBackupFilesIntern(backupPath);
    }


    /// <summary>
    /// Get a list with backup file orderd by date
    /// </summary>
    /// <param name="backupPath">Path for the backup files. Null means <see cref="IAppStartParameter.BackupPath"/></param>
    /// <returns>List with backup files</returns>
    private static List<string> GetBackupFilesIntern(string backupPath)
    {
        var result = new List<string>();

        var files = new DirectoryInfo(backupPath).GetFiles("*.bak").OrderBy(x => x.Name).ToList();

        foreach (var file in files)
        {
            result.Add(file.FullName);
        }

        return result;
    }

    #endregion


    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BaseUnitOfWork()
    {
        Dispose(false);
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }



        try
        {
            Repositories.Clear();
            //_dbFactory?.Dispose();

        }
        catch
        {
            // ignored
        }

    }


    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope()
    {
        return _dbContextScopeFactory.Create(DbContextScopeOption.JoinExisting, true);
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope(bool autoDetectChanges)
    {
        return _dbContextScopeFactory.Create(DbContextScopeOption.JoinExisting, autoDetectChanges);
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction)
    {
        return useTransaction ?
            _dbContextScopeFactory.CreateWithTransaction(IsolationLevel.Unspecified, autoDetectChanges) :
            _dbContextScopeFactory.Create(DbContextScopeOption.JoinExisting, autoDetectChanges);
    }

    /// <summary>
    /// Get a context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction, IsolationLevel isolationLevel)
    {
        return useTransaction ?
            _dbContextScopeFactory.CreateWithTransaction(isolationLevel, autoDetectChanges) :
            _dbContextScopeFactory.Create(DbContextScopeOption.JoinExisting, autoDetectChanges);
    }


    /// <summary>
    /// Get a read only context scope
    /// </summary>
    /// <returns>Returns a context scope</returns>
    public IContextReadOnlyScope GetReadOnlyContextScope()
    {
        return _dbContextScopeFactory.CreateReadOnly();
    }

    /// <summary>
    /// Run a plain SQL statement. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
    /// </summary>
    /// <param name="sql">SQL statement</param>
    /// <returns>true if statement ran successfully</returns>
    public virtual bool RunSql(string sql)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Run plain SQL statement on one open connection. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
    /// </summary>
    /// <param name="sql">SQL statement</param>
    /// <returns>true if statement ran successfully</returns>
    public virtual bool RunSqlBatch(IEnumerable<string> sql)
    {
        throw new NotSupportedException("Implement this method");
    }

}