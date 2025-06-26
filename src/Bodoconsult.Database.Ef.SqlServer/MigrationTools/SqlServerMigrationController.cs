// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Extensions;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.SqlServer.MigrationTools
{

    /// <summary>
    /// Handles the migrations during update to the newest database version
    /// </summary>
    public class SqlServerMigrationController<T> : IMigrationController where T : DbContext
    {
        private readonly string _timestamp = $"{DateTime.Now:yyyyMMddhhmmss}";

        /// <summary>
        /// Logger
        /// </summary>
        private readonly IAppLoggerProxy _log;

        public SqlServerMigrationController( IModelDataConvertersHandlerFactory modelDataConvertersHandlerFactory, IAppLoggerProxy logger)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            ModelDataConvertersHandler = modelDataConvertersHandlerFactory.CreateInstance();
        }

        private void CheckEf6AndMigrate(DbContext context)
        {
            _log.LogInformation("Migrations: check if there are EF6 migrations");

            var sql = "select isnull(OBJECT_ID(N'[__MigrationHistory]'),-1)";
            var isEf6Table = context.ExecuteSqlWithResult<int>(sql);


            sql = "select isnull(OBJECT_ID(N'[__EFMigrationsHistory]'),-1)";
            var isEfCoreTable = context.ExecuteSqlWithResult<int>(sql);

            if (isEfCoreTable > -1)
            {
                return;
            }

            if (isEf6Table == -1)
            {
                //
                //                throw new NotSupportedException("Database schema is not compatible to this app (EF table missing)!");
                //
                return;
            }

            // Create table for migrations
            sql = ResourceHelper.GetSqlResource("Ef6Migration1");
            context.Database.ExecuteSql(FormattableStringFactory.Create(sql));

            // 1. migration
            sql = ResourceHelper.GetSqlResource("Ef6Migration2");
            context.Database.ExecuteSql(FormattableStringFactory.Create(sql));

            // 2. migration
            sql = ResourceHelper.GetSqlResource("Ef6Migration3");
            context.Database.ExecuteSql(FormattableStringFactory.Create(sql));
        }

        /// <summary>
        /// Current unit of work
        /// </summary>
        public IUnitOfWork UnitOfWork { get; private set; }


        /// <summary>
        /// Current database model data converter handler
        /// </summary>
        public IModelDataConvertersHandler ModelDataConvertersHandler { get; }

        /// <summary>
        /// Run a string containing SQL statements separated with \r\nGO\r\n before migrating an existing database
        /// </summary>
        public string MigrationRunBeforeExistingDb { get; set; }

        /// <summary>
        /// Is Database a new database? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
        /// </summary>
        public bool IsNewDatabase { get; set; }

        /// <summary>
        /// Has database tables? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
        /// </summary>
        public bool HasTables { get; set; }

        /// <summary>
        /// Are migrations pending? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
        /// </summary>
        public bool MigrationsPending { get; set; }


        /// <summary>
        /// Load the current <see cref="IUnitOfWork"/> instance
        /// </summary>
        /// <param name="unitOfWork">Current <see cref="IUnitOfWork"/> instance</param>
        public void LoadUnitOfWork(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            ModelDataConvertersHandler.LoadUnitOfWork(unitOfWork);

            _log.LogInformation("Migrations: prepare all for migrations");

            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

            if (UnitOfWork.AppGlobals.ContextConfig.TurnOffMigrations)
            {
                return;
            }

            using (var scope = UnitOfWork.GetContextScope())
            {

                var context = ((IDbContextScope<T>)scope).DbContexts.GetContext();

                // Check if the database already exists
                IsNewDatabase = !context.Database.CanConnect();

                // Check if the database has tables
                var sql = "SELECT count(*) FROM sys.tables WHERE type = 'U';";
                HasTables = !IsNewDatabase && context.ExecuteSqlWithResult<int>(sql) > 0;

                // Check for Ef6
                if (!IsNewDatabase && HasTables)
                {
                    CheckEf6AndMigrate(context);
                }


                // Migrations pending?
                MigrationsPending = context.Database.GetPendingMigrations().Any();

                if (MigrationsPending)
                {
                    _log.LogInformation("Migrations: script changes");

                    //// Use one migrator to script the changes to a file
                    //var migrator = context.GetService<IMigrator>();

//                    sql = migrator.GenerateScript();
//                    var scriptFile = GetScriptFileName(_timestamp);

//                    try
//                    {
//                        if (File.Exists(scriptFile))
//                        {
//                            File.Delete(scriptFile);
//                        }
//                    }

//
//                    catch
//                    {
//                        // ignored
//                    }
//

//                    File.AppendAllText(scriptFile, sql);


                    // ToDO: save SQL as file
                    ////var scripter = new FileMigratorScriptingDecorator(migrator, _timestamp);
                    ////scripter.Save();
                }
                else
                {
                    sql = MigrationRunBeforeExistingDb;

                    if (sql == null)
                    {
                        return;
                    }

                    var cmds = sql.Split("\r\nGO\r\n", StringSplitOptions.RemoveEmptyEntries);

                    var timeout = context.Database.GetCommandTimeout();
                    context.Database.SetCommandTimeout(6000);

                    foreach (var sql1 in cmds)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(sql1.Replace("\r\n", " ", StringComparison.OrdinalIgnoreCase).Trim()))
                            {
                                continue;
                            }
                            context.Database.ExecuteSql(FormattableStringFactory.Create(sql1));
                        }
                        catch (Exception e)
                        {
                            _log.LogError($"Running SQL {sql1} failed", e);
                            throw;
                        }
                    }

                    context.Database.SetCommandTimeout(timeout);
                }
            }
        }

        /// <summary>
        /// Backup a database to a file
        /// </summary>
        public void SaveDatabase()
        {

#if DEBUG
            if (UnitOfWork.AppGlobals.ContextConfig.TurnOffBackup)
            {
                return;
            }
#endif

            if (IsNewDatabase || !HasTables)
            {
                return;
            }

            if (!MigrationsPending)
            {
                return;
            }


            var fileName = Path.Combine(UnitOfWork.AppGlobals.AppStartParameter.BackupPath, $"MigrationSqlBackup{DateTime.Now:yyyyMMddHHmmss}.bak");


            _log.LogInformation("Migrations: save database before migration");

            try
            {
                UnitOfWork.RunBackup(fileName);
                _log.LogInformation($"Migrations: database save to {fileName}");
            }
            catch (Exception e)
            {
                _log.LogError("Migrations:BackupDatabase", e);
            }
        }

        /// <summary>
        /// Backup a database to a file
        /// </summary>
        /// <param name="fileName">Full path to the backup file</param>
        public void SaveDatabase(string fileName)
        {

            if (IsNewDatabase || !HasTables)
            {
                return;
            }

            if (!MigrationsPending)
            {
                return;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.Combine(UnitOfWork.AppGlobals.AppStartParameter.BackupPath, $"MigrationSqlBackup{DateTime.Now:yyyyMMddHHmmss}.bak");
            }


            _log.LogInformation("Migrations: save database before migration");

            try
            {
                UnitOfWork.RunBackup(fileName);
                _log.LogInformation($"Migrations: database save to {fileName}");
            }

            catch (Exception e)
            {
                _log.LogError("Migrations:BackupDatabase", e);
            }


        }



        /// <summary>
        /// Migrate database to current version: applies schema changes
        /// </summary>
        public void MigrateDatabase()
        {

            if (!MigrationsPending)
            {
                return;
            }

            try
            {

                //_log.Info("Migrations: script changes");


                _log.LogInformation("Migrations: update database");


                using (var scope = UnitOfWork.GetContextScope())
                {
                    var context = ((IDbContextScope<T>)scope).DbContexts.GetContext();

                    var timeout = context.Database.GetCommandTimeout();
                    context.Database.SetCommandTimeout(6000);

                    context.Database.Migrate();

                    context.Database.SetCommandTimeout(timeout);

                }


                var fileName = Path.Combine(UnitOfWork.AppGlobals.AppStartParameter.BackupPath, $"MigrationSqlBackup{DateTime.Now:yyyyMMddHHmmss}.bak");



                try
                {
                    UnitOfWork.RunBackup(fileName);

                    UnitOfWork.ShrinkDatabase();
                    _log.LogInformation($"Migrations: database save after migration to {fileName}");


                }

                catch (Exception e)
                {
                    _log.LogError("Migrations:BackupDatabase after migration", e);
                }


            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Update database app content to current version (if necessary). Use only for app content not user content!
        /// </summary>
        public void ApplyDatabaseUpdates()
        {

            if (UnitOfWork.AppGlobals.ContextConfig.TurnOffConverters)
            {
                return;
            }

            ModelDataConvertersHandler.RunConverters();

            // Collect all messages and save it in one step to logfile
            var s = new StringBuilder();

            foreach (var message in ModelDataConvertersHandler.Messages)
            {
                s.AppendLine(message);
            }

            var msg = s.ToString();
            if (!string.IsNullOrEmpty(msg))
            {
                _log.LogInformation(s.ToString());
            }

            //// Todo: apply other content changes if necessary
        }

        //#region Private methods

        ///// <summary>
        ///// The real workload to do in the ctors
        ///// </summary>
        ///// <param name="timestamp">timestamp like 20190419110556</param>
        ///// <returns></returns>
        //private string GetScriptFileName(string timestamp)
        //{
        //    return Path.Combine(UnitOfWork.AppGlobals.AppStartParameter.BackupPath, $"MigrationSqlBackup{timestamp}.sql");
        //}

        //#endregion

    }
}
