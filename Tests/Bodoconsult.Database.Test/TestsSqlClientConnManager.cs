//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Data.SqlClient;
//using NUnit.Framework;

//namespace Bodoconsult.Core.Database.Test
//{
//    [TestFixture]
//    public class TestsSqlClientConnManager
//    {

//        private const string ConnectionString =
//            @"SERVER=.\sqlexpress;DATABASE=MediaDb;Trusted_Connection=True;MultipleActiveResultSets=true";


//        private AdapterConnManager _db;

//        [SetUp]
//        public void Setup()
//        {
//            _db = SqlClientConnManager.GetConnManager(ConnectionString);

//        }

//        /// <summary>
//        /// Test the connection
//        /// </summary>
//        [Test]
//        public void TestTestConnection()
//        {

//            var erg = _db.TestConnection();


//            Assert.IsTrue(erg);
//        }


//        /// <summary>
//        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataTableFromSql()
//        {

//            const string sql = "SELECT * FROM dbo.settings";

//            var erg = _db.GetDataTable(sql);


//            Assert.IsTrue(erg.Rows.Count>0);
//        }



//        /// <summary>
//        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataTableFromCommand()
//        {

//            const string sql = "SELECT * FROM dbo.settings";


//            var cmd = new SqlCommand
//            {
//                CommandText = sql
//            };

//            // Add parameters here if required

//            var erg = _db.GetDataTable(cmd);


//            Assert.IsTrue(erg.Rows.Count > 0);
//        }


//        /// <summary>
//        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataTableFromCommandWidthGetCommand()
//        {
//            const string sql = "SELECT * FROM dbo.settings";

//            var cmd = _db.GetCommand();
//            cmd.CommandText = sql;

//            // Add parameters here if required
//            var erg = _db.GetDataTable(cmd);


//            Assert.IsTrue(erg.Rows.Count > 0);
//        }


//        /// <summary>
//        /// Get a datatable from the database from a plain SQL string (avoid this option due to SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataReaderFromSql()
//        {

//            const string sql = "SELECT * FROM dbo.settings";

//            var erg = _db.GetDataReader(sql);

//            Assert.IsTrue(erg.FieldCount > 0);
//        }



//        /// <summary>
//        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataReaderFromCommand()
//        {

//            const string sql = "SELECT * FROM dbo.settings";


//            var cmd = new SqlCommand
//            {
//                CommandText = sql
//            };

//            // Add parameters here if required

//            var erg = _db.GetDataReader(cmd);

//            Assert.IsTrue(erg.FieldCount > 0);
//        }


//        /// <summary>
//        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataReaderFromCommandWidthGetCommand()
//        {
//            const string sql = "SELECT * FROM dbo.settings";

//            var cmd = _db.GetCommand();
//            cmd.CommandText = sql;

//            // Add parameters here if required
//            var erg = _db.GetDataReader(cmd);

//            Assert.IsTrue(erg.FieldCount>0);

//        }


//        /// <summary>
//        /// Get a datatable from the database from a (parameterized) SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestGetDataReaderFromCommandWidthGetCommandAndParameter()
//        {
//            const string sql = "SELECT * FROM dbo.settings WHERE S_ID=@ID;";

//            var cmd = _db.GetCommand();
//            cmd.CommandText = sql;

//            var p = _db.GetParameter(cmd, "@ID", GeneralDbType.UniqueIdentifier);
//            p.Value = new Guid("c3f41d97-26bf-4bf2-b428-756f97a17079");

//            // Add parameters here if required
//            var erg = _db.GetDataReader(cmd);

//            Assert.IsTrue(erg.FieldCount > 0);
//            Assert.IsTrue(erg.HasRows);

//        }

//        /// <summary>
//        /// Execute a plain SQL string (avoid this option due to SQL injection)
//        /// </summary>
//        [Test]
//        public void TestExecFromSql()
//        {

//            const string sql = "DELETE FROM dbo.settings WHERE skey='XXX'";

//            Assert.DoesNotThrow(() => _db.Exec(sql));
//        }


//        /// <summary>
//        /// Execute a SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestExecFromCommand()
//        {

//            const string sql = "DELETE FROM dbo.settings WHERE skey=@Key";

//            // Create command
//            var cmd = new SqlCommand
//            {
//                CommandText = sql
//            };

//            // Add a parameter to the command
//            var para = cmd.Parameters.Add("@Key", SqlDbType.VarChar);
//            para.Value = "XXX";

//            Assert.DoesNotThrow(() => _db.Exec(cmd));
//        }


//        /// <summary>
//        /// Get a scalar value from database from a plain SQL string (avoid this option due to SQL injection)
//        /// </summary>
//        [Test]
//        public void TestExecWithResultFromSql()
//        {

//            const string sql = "SELECT [Value] FROM dbo.settings WHERE skey='Company'";

//            var result = _db.ExecWithResult(sql);

//            Assert.IsNotNull(result);
//            Assert.IsFalse(string.IsNullOrEmpty(result));
//        }


//        /// <summary>
//        /// Get a scalar value from database from SqlCommand object (choose this option to avoid SQL injection)
//        /// </summary>
//        [Test]
//        public void TestExecWithResultFromCommand()
//        {

//            const string sql = "SELECT [Value] FROM dbo.settings WHERE skey=@Key";

//            // Create command
//            var cmd = new SqlCommand
//            {
//                CommandText = sql
//            };

//            // Add a parameter to the command
//            var para = cmd.Parameters.Add("@Key", SqlDbType.VarChar);
//            para.Value = "Company";

//            var result = _db.ExecWithResult(cmd);

//            Assert.IsNotNull(result);
//            Assert.IsFalse(string.IsNullOrEmpty(result));
//        }


//        [Test]
//        public void TestExecMultiple()
//        {
//            const string sql = "DELETE FROM [dbo].[DUMMY_Order] WHERE CTR_Type='D'";

//            var commands = new List<DbCommand>();

//            var cmd = new SqlCommand(sql);
//            commands.Add(cmd);

//            cmd = new SqlCommand(sql);
//            commands.Add(cmd);

//            cmd = new SqlCommand(sql);
//            commands.Add(cmd);

//            var result = _db.ExecMultiple(commands);

//            Assert.IsTrue(result == 0);
//        }


//        [TestCase(GeneralDbType.BigInt, SqlDbType.BigInt)]
//        [TestCase(GeneralDbType.Int, SqlDbType.Int)]
//        [TestCase(GeneralDbType.DateTime, SqlDbType.DateTime)]
//        [TestCase(GeneralDbType.DateTime2, SqlDbType.DateTime2)]
//        [TestCase(GeneralDbType.SmallInt, SqlDbType.SmallInt)]
//        [TestCase(GeneralDbType.Binary, SqlDbType.Binary)]
//        [TestCase(GeneralDbType.Char, SqlDbType.Char)]
//        [TestCase(GeneralDbType.Bit, SqlDbType.Bit)]
//        [TestCase(GeneralDbType.Decimal, SqlDbType.Decimal)]
//        [TestCase(GeneralDbType.Float, SqlDbType.Float)]
//        public void Test(GeneralDbType inpuType, SqlDbType expectedType)
//        {
//            // Arrange

//            // Act
//            var result = SqlClientConnManager.MapGeneralDbTypeToSqlDbType(inpuType);

//            // Assert
//            Assert.AreEqual(expectedType, result);
//        }

        
//    }
//}