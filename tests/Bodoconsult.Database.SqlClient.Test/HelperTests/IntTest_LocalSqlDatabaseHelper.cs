// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.IO;
using Bodoconsult.Database.SqlClient.DatabaseTools;
using Bodoconsult.Database.Test.Utilities.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test.HelperTests
{
    [TestFixture]
    [NonParallelizable]
    [SingleThreaded]
    // ReSharper disable once InconsistentNaming
    public class IntTest_LocalSqlDatabaseHelper
    {

        private readonly string _tempPath = FileHelper.GetTempPath();

        private readonly string _backupPath = FileHelper.BackupPath;

        private readonly string _input = TestHelper.LocalDbConnectionString;

        private readonly string _output = TestHelper.LocalDbConnectionString.Replace(TestHelper.DbName, "Master", StringComparison.CurrentCultureIgnoreCase);

        private LocalSqlDatabaseHelper _dh;


        //[SetUp]
        //public void Setup()
        //{

        //}

        [TearDown]
        public void TestCleanup()
        {
            try
            {
                if (_dh.DatabaseExists())
                {
                    _dh.DropDatabase();
                }
            }
            catch
            {
                // Ignore
            }


            _dh.Dispose();
            _dh = null;
        }


        [Test]
        public void TestMasterconnectionString()
        {


            // arrange
            _dh = new LocalSqlDatabaseHelper(_input)
            {
                DatabasePath = FileHelper.DataPath
            };

            // Act
            var result = _dh.MasterConnectionString;



            // assert
            Assert.That(!string.IsNullOrEmpty(result));

            Assert.That(result == _output);

            Assert.That(_dh.DatabaseName == TestHelper.DbName);


        }

        [Test]
        public void TestMasterconnectionStringDefault()
        {

            // arrange
            _dh = new LocalSqlDatabaseHelper(_input)
            {
                DatabasePath = FileHelper.DataPath
            };

            // Act
            var result = _dh.MasterConnectionString;

            // assert
            Assert.That(!string.IsNullOrEmpty(result));
            Assert.That(result.Contains("Master", StringComparison.InvariantCultureIgnoreCase));

            Assert.That(!string.IsNullOrEmpty(_dh.DatabaseName));

        }


        [Test]
        public void TestDatabaseExists()
        {

            // arrange
            _dh = new LocalSqlDatabaseHelper(_input);
            var conn = _dh.MasterConnectionString;

            // Act
            var result = _dh.DatabaseExists("Master");

            var result1 = _dh.DatabaseExists("MasterXXX");

            // assert
            Assert.That(result);
            Assert.That(!result1);
            Assert.That(!string.IsNullOrEmpty(conn));
            Assert.That(conn.Contains("Master", StringComparison.InvariantCultureIgnoreCase));

            Assert.That(!string.IsNullOrEmpty(_dh.DatabaseName));

        }


        [Test]
        public void TestAny()
        {

            // arrange
            _dh = new LocalSqlDatabaseHelper(_output); // Use output (master db) here !!!!
            var conn = _dh.MasterConnectionString;

            Assert.That(!string.IsNullOrEmpty(conn));

            // Act
            var result = _dh.Any("[spt_monitor]");

            // assert
            Assert.That(result);

        }

        [Test]
        public void TestCount()
        {

            // arrange
            _dh = new LocalSqlDatabaseHelper(_output); // Use output (master db) here !!!!
            var conn = _dh.MasterConnectionString;
            Assert.That(!string.IsNullOrEmpty(conn));

            // Act
            var result = _dh.Count("[spt_monitor]");

            // assert
            Assert.That(result > 0);

        }


        [Test]
        public void TestTableExists()
        {

            // arrange
            _dh = new LocalSqlDatabaseHelper(_output); // Use output (master db) here !!!!
            var conn = _dh.MasterConnectionString;
            Assert.That(!string.IsNullOrEmpty(conn));

            // Act
            var result = _dh.TableExists("dbo", "spt_monitor");

            // assert
            Assert.That(result);

            // Act
            result = _dh.TableExists("dbo", "spt_monitorXXXX");

            // assert
            Assert.That(!result);

        }




        [Test]
        public void TestBackupDatabase()
        {

            var backupFile = Path.Combine(_tempPath, "backup.bak");

            if (File.Exists(backupFile)) File.Delete(backupFile);

            // arrange
            _dh = new LocalSqlDatabaseHelper(_input)
            {
                DatabasePath = FileHelper.DataPath
            };

            var conn = _dh.MasterConnectionString;

            // Act
            var result = _dh.DatabaseExists("Master");

            var result1 = _dh.BackupDatabase(backupFile, "Master");

            // assert
            Assert.That(result);
            Assert.That(result1);
            Assert.That(!string.IsNullOrEmpty(conn));
            Assert.That(conn.Contains("Master", StringComparison.OrdinalIgnoreCase));

            Assert.That(!string.IsNullOrEmpty(_dh.DatabaseName));
            Assert.That(File.Exists(backupFile));

        }


        [Test]
        public void TestRestoreDatabase()
        {

            var backupFile = Path.Combine(_backupPath, "StSysDatabase_V1_02.bak");

            // arrange
            _dh = new LocalSqlDatabaseHelper(_input)
            {
                DatabasePath = FileHelper.DataPath
            };
            var conn = _dh.MasterConnectionString;


            if (_dh.DatabaseExists(TestHelper.DbName))
            {
                _dh.DropDatabase(TestHelper.DbName);
            }

            // Act
            var result = _dh.RestoreDatabase(backupFile, TestHelper.DbName);

            // assert
            Assert.That(result, Is.True);
            Assert.That(!string.IsNullOrEmpty(conn));
            Assert.That(conn.Contains("Master", StringComparison.InvariantCultureIgnoreCase));

            Assert.That(!string.IsNullOrEmpty(_dh.DatabaseName));
            Assert.That(File.Exists(backupFile));

            // special cleanup
            if (_dh.DatabaseExists(TestHelper.DbName))
            {
                _dh.DropDatabase(TestHelper.DbName);
            }

        }

        [Test]
        public void TestCheckHasData()
        {

            // arrange
            _dh = new LocalSqlDatabaseHelper(_output); // Use output (master db) here !!!!
            var conn = _dh.MasterConnectionString;
            Assert.That(!string.IsNullOrEmpty(conn));

            // Act
            var result = _dh.CheckHasData("SELECT * FROM sys.tables");

            // assert
            Assert.That(result);

            // Act
            result = _dh.CheckHasData("SELECT TOP 0 * FROM sys.tables"); ;

            // assert
            Assert.That(!result);

        }

    }
}
