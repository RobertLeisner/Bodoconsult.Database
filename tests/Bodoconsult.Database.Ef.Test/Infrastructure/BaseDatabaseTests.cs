// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using Bodoconsult.Database.Ef.SqlServer.DatabaseTools;
using Bodoconsult.Database.Test.Utilities.App;

namespace Bodoconsult.Database.Ef.Test.Infrastructure
{
    /// <summary>
    /// Base class for services tests
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    [SingleThreaded]
    public abstract class BaseDatabaseTests
    {

        private readonly string _userProfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        protected string ConnectionString = Globals.Instance.AppStartParameter.DefaultConnectionString;

        /// <summary>
        /// Path for databases
        /// </summary>
        protected string DataPath;


        /// <summary>
        /// Cleanup test environment
        /// </summary>
        [OneTimeTearDown]
        public virtual void FinalCleanup()
        {

            PrepareServer();

            //KillOldDbFiles();

            //using (var databaseHelper = new LocalSqlDatabaseHelper(ConnectionString))
            //{
            //    if (databaseHelper.DatabaseExists())
            //    {
            //        databaseHelper.DropDatabase();
            //    }
            //}
        }


        /// <summary>
        /// Drop the database on the server
        /// </summary>
        protected void PrepareServer()
        {

            KillOldDbFiles();

            using (var databaseHelper = new LocalSqlDatabaseHelper(ConnectionString))
            {
                if (!databaseHelper.DatabaseExists())
                {
                    return;
                }

                databaseHelper.DropDatabase();

                if (databaseHelper.DatabaseExists())
                {
                    throw new DataException($"Database {databaseHelper.DatabaseName} still exists!!");
                }
            }
        }

        /// <summary>
        /// Restore the database given in <see cref="ConnectionString"/> from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <returns>true if the backup was restored successfully</returns>
        protected virtual bool RestoreDatabase(string backupFile)
        {

            KillOldDbFiles();

            using (var databaseHelper = new LocalSqlDatabaseHelper(ConnectionString))
            {
                databaseHelper.DatabasePath = DataPath;

                if (!databaseHelper.DatabaseExists())
                {
                    return databaseHelper.RestoreDatabase(backupFile) ?? false;
                }

                databaseHelper.DropDatabase();

                if (databaseHelper.DatabaseExists())
                {
                    throw new DataException($"Database {databaseHelper.DatabaseName} still exists!!");
                }

                return databaseHelper.RestoreDatabase(backupFile) ?? false;
            }
        }



        /// <summary>
        /// Restore the database from a full backup file
        /// </summary>
        /// <param name="backupFile">full backup file to use</param>
        /// <param name="databaseName">Name of the database to restore in</param>
        /// <returns>true if the backup was restored successfully</returns>
        protected virtual bool RestoreDatabase(string backupFile, string databaseName)
        {

            KillOldDbFiles();

            using (var databaseHelper = new LocalSqlDatabaseHelper(ConnectionString))
            {
                if (!databaseHelper.DatabaseExists())
                {
                    return false;
                }

                databaseHelper.DropDatabase();

                if (databaseHelper.DatabaseExists())
                {
                    throw new DataException($"Database {databaseHelper.DatabaseName} still exists!!");
                }

                return databaseHelper.RestoreDatabase(backupFile, databaseName) ?? false;
            }
        }

        /// <summary>
        /// Check if the DB files are still there
        /// </summary>
        protected void KillOldDbFiles()
        {
            DeleteFileIfExist("XX.mdf");
            DeleteFileIfExist("XX_log.ldf");
        }

        /// <summary>
        ///
        /// Check if the files are still there and delete if they are
        /// </summary>
        /// <param name="fileName">Name of file to check and delete</param>
        private void DeleteFileIfExist(string fileName)
        {
            var filePath = Path.Combine(_userProfilePath, fileName);
            if (!File.Exists(filePath))
            {
                return;
            }
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                // Do nothing
            }
        }
    }
}
