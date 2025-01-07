// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Sqlite.Test.Helpers;
using Bodoconsult.Database.Sqlite.Test.MetaDataSample;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Bodoconsult.Database.Sqlite.Test
{


    [TestFixture]
    public class UnitTestSqliteConnManager
    {

        private IConnManager _db;

        [SetUp]
        public void Setup()
        {
            var conn = TestHelper.SqliteConnectionString;

            _db = SqliteConnManager.GetConnManager(conn);

        }

        [Test]
        public void TestTestConnection()
        {
            // Assert

            // Act
            var result = _db.TestConnection();

            // Assert
            Assert.IsTrue(result);

        }



        [Test]
        public void TestTestConnectionDatabaseBuiltFromConnectionString()
        {
            // Assert

            var fileName = @"D:\temp\test.sqlite";

            var conn = @$"Data Source={fileName}";

            if (File.Exists(fileName)) File.Delete(fileName);

            _db = SqliteConnManager.GetConnManager(conn);

            // Act
            var result = _db.TestConnection();

            // Assert
            Assert.IsTrue(result);

        }


        //[Test]
        //public void TestTestConnectionCreateChinookDatabase()
        //{
        //    // Assert

        //    var fileName = @"D:\temp\chinook.sqlite";

        //    var conn = @$"Data Source={fileName}";

        //    if (File.Exists(fileName)) File.Delete(fileName);

        //    _db = SqliteConnManager.GetConnManager(conn);

        //    var script = TestHelper.GetTextResource("Bodoconsult.Core.Database.Sqlite.Test.Resources.Chinook_Sqlite.sql");

        //    // Act
        //    var result = _db.TestConnection();

        //    _db.Exec(script);


        //    // Assert
        //    Assert.IsTrue(result);

        //}



        [Test]
        public void TestGetDataTableSql()
        {
            // Assert
            var sql = "SELECT * FROM \"Customer\";";

            // Act
            var result = _db.GetDataTable(sql);

            // Assert
            Assert.IsNotNull(result);

        }


        [Test]
        public void TestGetDataTableCommand()
        {
            // Act
            var sql =
                "SELECT * FROM \"Customer\"; ";

            var cmd = new SqliteCommand(sql);

            // Act
            var result = _db.GetDataTable(cmd);

            // Assert
            Assert.IsNotNull(result);

        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromCommandWithGetCommand()
        {
            const string sql = "SELECT * FROM Customer";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataTable(cmd);


            Assert.IsTrue(erg.Rows.Count > 0);
        }


        /// <summary>
        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromSql()
        {

            const string sql = "SELECT * FROM \"Customer\"";

            var erg = _db.GetDataReader(sql);

            Assert.IsTrue(erg.FieldCount > 0);
        }



        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommand()
        {

            const string sql = "SELECT * FROM \"Customer\"";


            var cmd = new SqliteCommand
            {
                CommandText = sql
            };

            // Add parameters here if required

            var erg = _db.GetDataReader(cmd);

            Assert.IsTrue(erg.FieldCount > 0);
        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommandWidthGetCommand()
        {
            const string sql = "SELECT * FROM \"Customer\"";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataReader(cmd);

            Assert.IsTrue(erg.FieldCount > 0);

        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommandWidthGetCommandAndParameter()
        {
            const string sql = "SELECT * FROM \"Customer\" WHERE \"CustomerId\"=@ID;";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            var p = _db.GetParameter(cmd, "@ID", GeneralDbType.Int);
            p.Value = 8;

            // Add parameters here if required
            var erg = _db.GetDataReader(cmd);

            Assert.IsTrue(erg.FieldCount > 0);
            Assert.IsTrue(erg.HasRows);

        }

        /// <summary>
        /// Execute a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestExecFromSql()
        {

            const string sql = "DELETE FROM \"Customer\" WHERE \"CustomerId\"=-99";

            Assert.DoesNotThrow(() => _db.Exec(sql));
        }


        /// <summary>
        /// Execute a SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecFromCommand()
        {

            const string sql = "DELETE FROM \"Customer\" WHERE \"CustomerId\"=@Key";

            // Create command
            var cmd = new SqliteCommand()
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", SqliteType.Integer);
            para.Value = -99;

            Assert.DoesNotThrow(() => _db.Exec(cmd));
        }


        /// <summary>
        /// Get a scalar value from database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromSql()
        {

            const string sql = "SELECT \"CustomerId\" FROM \"Customer\" WHERE \"CustomerId\"=8;";

            var result = _db.ExecWithResult(sql);

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }


        /// <summary>
        /// Get a scalar value from database from SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromCommand()
        {

            const string sql = "SELECT \"CustomerId\" FROM \"Customer\" WHERE \"CustomerId\"=@Key";

            // Create command
            var cmd = new SqliteCommand
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", SqliteType.Integer);
            para.Value = 8;

            var result = _db.ExecWithResult(cmd);

            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }


        [Test]
        public void TestExecMultiple()
        {
            const string sql = "DELETE FROM \"Customer\" WHERE \"CustomerId\"=-99";

            var commands = new List<DbCommand>();

            var cmd = new SqliteCommand(sql);
            commands.Add(cmd);

            cmd = new SqliteCommand(sql);
            commands.Add(cmd);

            cmd = new SqliteCommand(sql);
            commands.Add(cmd);

            var result = _db.ExecMultiple(commands);

            Assert.IsTrue(result == 0);
        }

        [TestCase(1278)]
        [TestCase(220)]
        [TestCase(5)]
        public void TestExecMultipleWithNewRow(int runs)
        {
            const string sql = "SELECT COUNT(*) FROM \"Customer\"";

            var count1 = Convert.ToInt32(_db.ExecWithResult(sql));

            var commands = new List<DbCommand>();

            var i = 0;
            for (var index = 1; index < runs; index++)
            {
                commands.Add(TestDataHelper.AddNewCommand(count1+index));
                i++;
            }


            var result = _db.ExecMultiple(commands);

            Assert.IsTrue(result == 0);

            var count2 = Convert.ToInt32(_db.ExecWithResult(sql));
            Assert.IsTrue(count2==count1+i);
            
            _db.Exec($"DELETE FROM \"Customer\" WHERE \"CustomerId\">{count1}");

        }


        [TestCase(GeneralDbType.Int, SqliteType.Integer)]

        [TestCase(GeneralDbType.Decimal, SqliteType.Real)]
        [TestCase(GeneralDbType.Float, SqliteType.Real)]
        [TestCase(GeneralDbType.Text, SqliteType.Text)]
        public void Test(GeneralDbType inpuType, SqliteType expectedType)
        {
            // Arrange

            // Act
            var result = SqliteConnManager.MapGeneralDbTypeToSqliteType(inpuType);

            // Assert
            Assert.AreEqual(expectedType, result);
        }
    }
}