// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using System.Data.Common;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Test.Utilities.Helpers;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;

namespace Bodoconsult.Database.Postgres.Test
{

    /// <summary>
    /// Install postgres version of Chinook database before testing.
    /// See https://github.com/lerocha/chinook-database/tree/master/ChinookDatabase/DataSources for details.
    ///
    /// Pay attention field names and table names are normally lower case words in PostgreSQL.
    /// If you want to use upper case or a mixture of upper and lower case, please set the names in
    /// quotation marks.
    /// </summary>
    [TestFixture]
    public class UnitTestPostgresConnManager
    {

        private IConnManager _db;

        [SetUp]
        public void Setup()
        {
            var conn = TestHelper.ConnectionString;

            _db = PostgresConnManager.GetConnManager(conn);

        }

        [Test]
        public void TestGetDataTableSql()
        {
            // Assert
            var sql = "SELECT * FROM \"Customer\";";

            // Act
            var result = _db.GetDataTable(sql);

            // Assert
            Assert.That(result, Is.Not.Null);

        }


        [Test]
        public void TestGetDataTableCommand()
        {
            // Act
            var sql =
                "SELECT * FROM \"Customer\"; ";

            var cmd = new NpgsqlCommand(sql);

            // Act
            var result = _db.GetDataTable(cmd);

            // Assert
            Assert.That(result, Is.Not.Null);

        }


        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataTableFromCommandWithGetCommand()
        {
            const string sql = "SELECT * FROM \"Customer\"";

            var cmd = _db.GetCommand();
            cmd.CommandText = sql;

            // Add parameters here if required
            var erg = _db.GetDataTable(cmd);


            Assert.That(erg.Rows.Count > 0);
        }


        /// <summary>
        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromSql()
        {

            const string sql = "SELECT * FROM \"Customer\"";

            var erg = _db.GetDataReader(sql);

            Assert.That(erg.FieldCount > 0);
        }



        /// <summary>
        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestGetDataReaderFromCommand()
        {

            const string sql = "SELECT * FROM \"Customer\"";


            var cmd = new NpgsqlCommand
            {
                CommandText = sql
            };

            // Add parameters here if required

            var erg = _db.GetDataReader(cmd);

            Assert.That(erg.FieldCount > 0);
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

            Assert.That(erg.FieldCount > 0);

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

            Assert.That(erg.FieldCount > 0);
            Assert.That(erg.HasRows);

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
            var cmd = new NpgsqlCommand()
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", NpgsqlDbType.Integer);
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

            Assert.That(result, Is.Not.Null);
            Assert.That(string.IsNullOrEmpty(result), Is.False);
        }


        /// <summary>
        /// Get a scalar value from database from SqlCommand object (choose this option to avoid SQL injection)
        /// </summary>
        [Test]
        public void TestExecWithResultFromCommand()
        {

            const string sql = "SELECT \"CustomerId\" FROM \"Customer\" WHERE \"CustomerId\"=@Key";

            // Create command
            var cmd = new NpgsqlCommand
            {
                CommandText = sql
            };

            // Add a parameter to the command
            var para = cmd.Parameters.Add("@Key", NpgsqlDbType.Integer);
            para.Value = 8;

            var result = _db.ExecWithResult(cmd);

            Assert.That(result, Is.Not.Null);
            Assert.That(string.IsNullOrEmpty(result), Is.False);
        }


        [Test]
        public void TestExecMultiple()
        {
            const string sql = "DELETE FROM \"Customer\" WHERE \"CustomerId\"=-99";

            var commands = new List<DbCommand>();

            var cmd = new NpgsqlCommand(sql);
            commands.Add(cmd);

            cmd = new NpgsqlCommand(sql);
            commands.Add(cmd);

            cmd = new NpgsqlCommand(sql);
            commands.Add(cmd);

            var result = _db.ExecMultiple(commands);

            Assert.That(result == 0);
        }

        
        [TestCase(GeneralDbType.Int, NpgsqlDbType.Integer)]
        [TestCase(GeneralDbType.DateTime, NpgsqlDbType.Date)]
        [TestCase(GeneralDbType.DateTime2, NpgsqlDbType.Date)]
        [TestCase(GeneralDbType.SmallInt, NpgsqlDbType.Smallint)]
        [TestCase(GeneralDbType.Char, NpgsqlDbType.Char)]
        [TestCase(GeneralDbType.Bit, NpgsqlDbType.Bit)]
        [TestCase(GeneralDbType.Decimal, NpgsqlDbType.Numeric)]
        [TestCase(GeneralDbType.Float, NpgsqlDbType.Double)]
        public void Test(GeneralDbType inpuType, NpgsqlDbType expectedType)
        {
            // Arrange

            // Act
            var result = PostgresConnManager.MapGeneralDbTypeToNpgsqlDbType(inpuType);

            // Assert
            Assert.That(result, Is.SameAs(expectedType));
        }



        [Test]
        public void TestTestConnection()
        {
            // Assert

            // Act
            var result = _db.TestConnection();

            // Assert
            Assert.That(result);

        }



    }
}