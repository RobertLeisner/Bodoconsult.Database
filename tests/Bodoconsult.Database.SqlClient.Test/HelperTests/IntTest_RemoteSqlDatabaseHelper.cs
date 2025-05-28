// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.IO;
using Bodoconsult.Database.SqlClient.DatabaseTools;
using Bodoconsult.Database.Test.Utilities.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test.HelperTests
{
    [Explicit]
    [NonParallelizable]
    [TestFixture]
    [SingleThreaded]
    // ReSharper disable once InconsistentNaming
    public class IntTest_RemoteSqlDatabaseHelper
    {

        private readonly string _tempPath = FileHelper.GetTempPath();

        private readonly string _backupPath = FileHelper.BackupPath;

        private readonly string _dataPath = FileHelper.DataPath;

        private readonly string _input = TestHelper.LocalDbConnectionString;



        //[Test]
        //public void TestBackupDatabase()
        //{

        //    string backupFile = Path.Combine(_tempPath, "backup.bak");

        //    if (File.Exists(backupFile)) File.Delete(backupFile);

        //    // arrange
        //    var dh = new LocalSqlDatabaseHelper();


        //    var conn = dh.MasterConnectionString;

        //    // Act
        //    var result = dh.DatabaseExists("Master");

        //    var result1 = dh.BackupDatabase(backupFile, "Master");

        //    // assert
        //    Assert.That(result);
        //    Assert.That(result1);
        //    Assert.That(!string.IsNullOrEmpty(conn));
        //    Assert.That(conn.Contains("Master"));

        //    Assert.That(!string.IsNullOrEmpty(dh.DatabaseName));
        //    Assert.That(File.Exists(backupFile));
        //}


        [Test]
        public void TestRestoreDatabase()
        {

            var backupFile = Path.Combine(_backupPath, "StSysDatabase_V1_02.bak");

            var databaseName = "StSysDatabaseV30";

            // arrange
            var dh = new RemoteSqlDatabaseHelper(_input)
            {
                DatabasePath = _dataPath,
                RemoteNetworkPath = _tempPath,
                RemoteLocalPath = _tempPath
            };


            var conn = dh.MasterConnectionString;


            if (dh.DatabaseExists(databaseName))
            {
                dh.DropDatabase(databaseName);
            }

            // Act
            var result = dh.RestoreDatabase(backupFile, databaseName);

            // assert
            Assert.That(result, Is.True);
            Assert.That(!string.IsNullOrEmpty(conn));
            Assert.That(conn.Contains("Master", StringComparison.InvariantCultureIgnoreCase));

            Assert.That(!string.IsNullOrEmpty(dh.DatabaseName));
            Assert.That(File.Exists(backupFile));

            if (dh.DatabaseExists(databaseName))
            {
                dh.DropDatabase(databaseName);
            }

            dh.Dispose();
        }
    }
}