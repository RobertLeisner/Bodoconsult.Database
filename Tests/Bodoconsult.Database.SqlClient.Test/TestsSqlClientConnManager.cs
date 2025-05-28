// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Test.Utilities.Helpers;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test
{
    [TestFixture]
    public class TestsSqlClientConnManager
    {

        private readonly string _connectionString = TestHelper.LocalDbConnectionString;


        private IConnManager _db;

        [SetUp]
        public void Setup()
        {
            _db = SqlClientConnManager.GetConnManager(_connectionString);

        }

        /// <summary>
        /// Test the connection
        /// </summary>
        [Test]
        public void TestTestConnection()
        {

            var erg = _db.TestConnection();


            Assert.That(erg, Is.EqualTo(true));
        }


        /// <summary>
        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromSql()
        {

            const string sql = "SELECT * FROM dbo.Customer";

            var erg = _db.GetDataTable(sql);

            Assert.That(erg.Rows.Count, Is.GreaterThan(0));
        }



        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromCommand()
        {

            const string sql = "SELECT * FROM dbo.Customer";


            var cmd = new SqlCommand
            {
                CommandText = sql
            };

            // Add parameters here if required

            var erg = _db.GetDataTable(cmd);


            Assert.That(erg.Rows.Count, Is.GreaterThan(0));
        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromCommandWidthGetCommandWithParameter()
        {
            const string sql = "SELECT * FROM dbo.Customer WHERE CustomerId=@ID";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            var p = _db.GetParameter(cmd, "@ID", GeneralDbType.Int);
            p.Value = 1;

            // Add parameters here if required
            var erg = _db.GetDataTable(cmd);


            Assert.That(erg.Rows.Count, Is.GreaterThan(0));
            Assert.That(erg.Rows.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromCommandWidthGetCommand()
        {
            const string sql = "SELECT * FROM dbo.Customer";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataTable(cmd);


            Assert.That(erg.Rows.Count, Is.GreaterThan(0));
        }


        /// <summary>
        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromSql()
        {

            const string sql = "SELECT * FROM dbo.Customer";

            var erg = _db.GetDataReader(sql);

            Assert.That(erg.FieldCount, Is.GreaterThan(0));
        }



        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommand()
        {

            const string sql = "SELECT * FROM dbo.Customer";


            var cmd = new SqlCommand
            {
                CommandText = sql
            };

            // Add parameters here if required

            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount, Is.GreaterThan(0));
        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommandWidthGetCommand()
        {
            const string sql = "SELECT * FROM dbo.Customer";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount, Is.GreaterThan(0));

        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommandWidthGetCommandAndParameter()
        {
            const string sql = "SELECT * FROM dbo.Customer WHERE [CustomerId]=@ID;";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            var p = _db.GetParameter(cmd, "@ID", GeneralDbType.Int);
            p.Value = 1;

            // Add parameters here if required
            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount, Is.GreaterThan(0));
            Assert.That(erg.HasRows, Is.EqualTo(true));

        }

        /// <summary>
        /// Execute a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestExecFromSql()
        {

            const string sql = "DELETE FROM dbo.Customer WHERE CustomerId=-99";

            Assert.DoesNotThrow(() => _db.Exec(sql));
        }


        /// <summary>
        /// Execute a SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecFromCommand()
        {

            const string sql = "DELETE FROM dbo.Customer WHERE CustomerId=@Key";

            // Create command
            var cmd = new SqlCommand
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", SqlDbType.Int);
            para.Value = -99;

            Assert.DoesNotThrow(() => _db.Exec(cmd));
        }


        /// <summary>
        /// Get a scalar value from database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromSql()
        {

            const string sql = "SELECT CustomerId FROM dbo.Customer WHERE CustomerId=1";

            var result = _db.ExecWithResult(sql);

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));
        }


        /// <summary>
        /// Get a scalar value from database from SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromCommand()
        {

            const string sql = "SELECT CustomerId FROM dbo.Customer WHERE CustomerId=@Key";

            // Create command
            var cmd = new SqlCommand
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", SqlDbType.Int);
            para.Value = 1;

            var result = _db.ExecWithResult(cmd);

            Assert.That(result, Is.Not.EqualTo(null));
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));
        }


        [Test]
        public void TestExecMultiple()
        {
            const string sql = "DELETE FROM Customer WHERE CustomerId=-99";

            var commands = new List<DbCommand>();

            var cmd = new SqlCommand(sql);
            commands.Add(cmd);

            cmd = new SqlCommand(sql);
            commands.Add(cmd);

            cmd = new SqlCommand(sql);
            commands.Add(cmd);

            var result = _db.ExecMultiple(commands);

            Assert.That(result, Is.EqualTo(0));
        }


        [TestCase(GeneralDbType.BigInt, SqlDbType.BigInt)]
        [TestCase(GeneralDbType.Int, SqlDbType.Int)]
        [TestCase(GeneralDbType.DateTime, SqlDbType.DateTime)]
        [TestCase(GeneralDbType.DateTime2, SqlDbType.DateTime2)]
        [TestCase(GeneralDbType.SmallInt, SqlDbType.SmallInt)]
        [TestCase(GeneralDbType.Binary, SqlDbType.Binary)]
        [TestCase(GeneralDbType.Char, SqlDbType.Char)]
        [TestCase(GeneralDbType.Bit, SqlDbType.Bit)]
        [TestCase(GeneralDbType.Decimal, SqlDbType.Decimal)]
        [TestCase(GeneralDbType.Float, SqlDbType.Float)]
        public void Test(GeneralDbType inpuType, SqlDbType expectedType)
        {
            // Arrange

            // Act
            var result = SqlClientConnManager.MapGeneralDbTypeToSqlDbType(inpuType);

            // Assert
            Assert.That(result, Is.EqualTo(expectedType));
        }


        

    }
}