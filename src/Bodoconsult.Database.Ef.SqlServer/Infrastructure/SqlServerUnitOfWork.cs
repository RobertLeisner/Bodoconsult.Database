// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.SqlServer.Infrastructure
{
    /// <summary>
    /// SqlServer implementation of a unit of work
    /// </summary>
    /// <typeparam name="T">Database context</typeparam>
    public class SqlServerUnitOfWork<T>: BaseUnitOfWork<T> where T : DbContext
    {

        private const string ExecuteTemplate = "Execute";

        private readonly string _executeSql = ResourceHelper.GetSqlResource(ExecuteTemplate);

        /// <summary>
        /// Ctor with additional backup engine
        /// </summary>
        /// <param name="dbContextScopeFactory"></param>
        /// <param name="logger"></param>
        /// <param name="ambientDbContextLocator"></param>
        /// <param name="backupEngine"></param>
        /// <param name="migrationController">Current migration controller</param>
        public SqlServerUnitOfWork(IContextScopeFactory<T> dbContextScopeFactory, IAppLoggerProxy logger,
            IAmbientDbContextLocator ambientDbContextLocator, IBackupEngine backupEngine, IMigrationController migrationController) : 
            base(dbContextScopeFactory, logger, ambientDbContextLocator, backupEngine, migrationController)
        { }



        /// <summary>
        /// Get the generic repository for type TEntity
        /// </summary>
        /// <typeparam name="TEntity">POCO entity type</typeparam>
        /// <returns>The request generic repository</returns>
        public override IRepository<TEntity> GetRepository<TEntity>()
        {
            // Checks if the Dictionary Key contains the Model class
            if (Repositories.Keys.Contains(typeof(TEntity)))
            {
                // Return the repository for that Model class
                return Repositories[typeof(TEntity)] as IRepository<TEntity>;
            }

            // If the repository for that Model class doesn't exist, create it
            var repository = new Repository<TEntity, T>(AmbientDbContextLocator, Log);

            // Add it to the dictionary
            Repositories.Add(typeof(TEntity), repository);

            return repository;

        }

        public override IRepositoryGuid<TEntity> GetRepositoryGuid<TEntity>()
        {
            // Checks if the Dictionary Key contains the Model class
            if (RepositoriesGuid.Keys.Contains(typeof(TEntity)))
            {
                // Return the repository for that Model class
                return RepositoriesGuid[typeof(TEntity)] as IRepositoryGuid<TEntity>;
            }

            // If the repository for that Model class doesn't exist, create it
            var repository = new RepositoryGuid<TEntity, T>(AmbientDbContextLocator, Log);

            // Add it to the dictionary
            RepositoriesGuid.Add(typeof(TEntity), repository);

            return repository;
        }

        /// <summary>
        /// Run a plain SQL statement. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>true if statement ran successfully</returns>
        public override bool RunSql(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return true;
            }

            string sql1 = null;

            try
            {


                using (var conn = new SqlConnection(ContextConfig.ConnectionString))
                {
                    conn.Open();

                    if (sql.Contains("\r\nGO\r\n", StringComparison.OrdinalIgnoreCase))
                    {
                        var cmds = sql.Split(new[] { '\r', '\n', 'G', 'O', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        for (var index = 0; index < cmds.Length; index++)
                        {
                            sql1 = cmds[index];


                            using (var command = new SqlCommand(_executeSql, conn))
                            {
                                command.Parameters.Add("@Data", SqlDbType.NVarChar).Value = sql1;
                                command.CommandTimeout = ContextConfig.CommandTimeout;
                                command.ExecuteNonQuery();
                            }

                        }
                    }
                    else
                    {
                        sql1 = sql;


                        using (var command = new SqlCommand(_executeSql, conn))
                        {
                            command.Parameters.Add("@Data", SqlDbType.NVarChar).Value = sql1;
                            command.CommandTimeout = ContextConfig.CommandTimeout;
                            command.ExecuteNonQuery();
                        }

                    }


                    conn.Close();
                    conn.Dispose();

                }

                return true;
            }

            catch (Exception e)
            {
                // Debug.Print($"Error: Runsql: {sql}: {e.Message}");
                Log.LogError($"Runsql: {sql1}", e);
                return false;
            }


        }

        /// <summary>
        /// Run plain SQL statement on one open connection. Watchout or cross plattform issues. Never use inconjunction with usr input due to SqlInjection.
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>true if statement ran successfully</returns>
        public override bool RunSqlBatch(IEnumerable<string> sql)
        {
            if (sql == null)
            {
                return false;
            }


            var cmds = sql.ToList();

            if (!cmds.Any())
            {
                return false;
            }

            string cmd = null;

            try
            {

                using (var conn = new SqlConnection(ContextConfig.ConnectionString))
                {
                    conn.Open();

                    for (var index = 0; index < cmds.Count; index++)
                    {
                        cmd = cmds[index];

                        using (var command = new SqlCommand(_executeSql, conn))
                        {
                            command.Parameters.Add("@Data", SqlDbType.NVarChar).Value = cmd;
                            command.CommandTimeout = ContextConfig.CommandTimeout;
                            command.ExecuteNonQuery();
                        }

                    }
                    conn.Close();
                    conn.Dispose();
                }

                return true;
            }

            catch (Exception e)
            {
                // Debug.Print($"Error: Runsql: {sql}: {cmd}: {e.Message}");
                Log.LogError($"Runsql: {sql}: {cmd}", e);
                return false;
            }

        }

        /// <summary>
        /// Shrink the database. Takes effect only after backup
        /// </summary>
        public override void ShrinkDatabase()
        {
            var raw = ContextConfig.ConnectionString;

            var i = raw.ToUpperInvariant().IndexOf("INITIAL CATALOG=", StringComparison.Ordinal);

            var j = raw.ToUpperInvariant().IndexOf(";", i + 1, StringComparison.Ordinal);

            var databaseName = raw.Substring(i + 16, j - i - 16);

            var sql = $"DBCC SHRINKDATABASE ({databaseName}, 10);";

            RunSql(sql);
        }
    }
}