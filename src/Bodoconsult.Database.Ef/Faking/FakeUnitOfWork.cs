// Copyright (c) Mycronic. All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.Faking
{


    /// <summary>
    /// Unit of work for mocking
    /// </summary>
    public class FakeUnitOfWork : IUnitOfWork
    {

        private readonly Dictionary<Type, object> _repositories;

        private readonly Dictionary<Type, object> _repositoriesGuid;

        private readonly IContextScopeFactory<FakeDbContext> _contextScopeFactory;


        private IDirectAccessRepository _directAccessRepository;

        /// <summary>
        /// Default ctor with slot data
        /// </summary>
        public FakeUnitOfWork(IContextScopeFactory<FakeDbContext> contextScopeFactory)
        {
            // For simple tests leave here
            if (contextScopeFactory == null)
            {
                return;
            }

            _contextScopeFactory = contextScopeFactory;
            AppGlobals = contextScopeFactory.AppGlobals;

            _repositories = new Dictionary<Type, object>();

            _repositoriesGuid = new Dictionary<Type, object>();

        }

        /// <summary>
        ///  Start the migration work flow
        /// </summary>
        public void StartMigrationWorkFlow()
        {
            // Do nothing
        }

        /// <summary>
        /// Get the generic repository for type TEntity
        /// </summary>
        /// <typeparam name="TEntity">POCO entity type</typeparam>
        /// <returns>The request generic repository</returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntityRequirements, new()
        {
            // Checks if the Dictionary Key contains the Model class
            if (_repositories.Keys.Contains(typeof(TEntity)))
            {
                // Return the repository for that Model class
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;
            }

            // If the repository for that Model class doesn't exist, create it
            var repository = new FakeRepository<TEntity>(new List<TEntity>(), this);

            // Add it to the dictionary
            _repositories.Add(typeof(TEntity), repository);

            return repository;

        }

        public IRepositoryGuid<TEntity> GetRepositoryGuid<TEntity>() where TEntity : class, IEntityRequirementsGuid, new()
        {
            // Checks if the Dictionary Key contains the Model class
            if (_repositoriesGuid.Keys.Contains(typeof(TEntity)))
            {
                // Return the repository for that Model class
                return _repositoriesGuid[typeof(TEntity)] as IRepositoryGuid<TEntity>;
            }

            // If the repository for that Model class doesn't exist, create it
            var repository = new FakeRepositoryGuid<TEntity>(new List<TEntity>(), this);

            // Add it to the dictionary
            _repositoriesGuid.Add(typeof(TEntity), repository);

            return repository;
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

        public IAppGlobalsWithDatabase AppGlobals { get; }

        /// <summary>
        /// Current context config
        /// </summary>
        public IContextConfig ContextConfig => AppGlobals.ContextConfig;

        /// <summary>
        /// Number of backups kept additonally to the current one. Default: 30
        /// </summary>
        public int NumberOfBackups { get; set; }


        /////// <summary>
        /////// Get the current data provider name
        /////// </summary>
        ////public string ProviderName => "System.Data.Mocking";

        /// <summary>
        /// Run an online backup from the database
        /// </summary>
        /// <param name="backupFileName">backup file name</param>
        /// <returns>Backup file name with successful, null if the backup failed</returns>
        public string RunBackup(string backupFileName)
        {
            //if (File.Exists(backupFileName)) { File.Delete(backupFileName); }
            //File.WriteAllText(backupFileName, "TestBackupFile");
            //return backupFileName;
            throw new NotSupportedException();
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
            return new FakeContextScope<FakeDbContext>(ContextConfig);
        }

        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        public IContextScope GetContextScope(bool autoDetectChanges)
        {
            return new FakeContextScope<FakeDbContext>(ContextConfig);
        }

        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        public IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction)
        {
            return new FakeContextScope<FakeDbContext>(ContextConfig);
        }

        /// <summary>
        /// Get a context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>
        public IContextScope GetContextScope(bool autoDetectChanges, bool useTransaction, IsolationLevel isolationLevel)
        {
            return new FakeContextScope<FakeDbContext>(ContextConfig);
        }


        /// <summary>s
        /// Get a read only context scope
        /// </summary>
        /// <returns>Returns a context scope</returns>

        public IContextReadOnlyScope GetReadOnlyContextScope()
        {
            return new FakeContextReadOnlyScope<FakeDbContext>(ContextConfig);
        }

        public bool RunSql(string sql)
        {
            return true;
        }

        /// <summary>
        /// Run plain SQL statement on one open connection. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>true if statement ran successfully</returns>
        public bool RunSqlBatch(IEnumerable<string> sql)
        {
            // Do nothing
            return true;
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


        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~FakeUnitOfWork()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                try
                {
                    _repositories.Clear();
                    //_dbFactory?.Dispose();

                }
                catch
                {
                    // ignored
                }

            }
        }
    }
}