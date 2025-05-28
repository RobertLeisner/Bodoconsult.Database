// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Linq;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Sqlite.MetaData;
using Bodoconsult.Database.Test.Utilities.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.Sqlite.Test
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
    public class UnitTestSqliteMetaDataService
    {

        private IMetaDataService _service;

        private string _conn;


        private const string Sql = "SELECT * FROM \"Customer\";";

        private const string EntityName = "Customer";

        private const string PrimaryKeyField = "CustomerId";


        [SetUp]
        public void Setup()
        {
            _conn = TestHelper.ConnectionString;

            _service = new SqliteMetaDataService();
        }

        [Test]
        public void TestGetMetaData()
        {
            // Assert

            // Act
            var table = _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            // Assert
            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());
        }


        //[Test]
        //public void TestGetMetaDataMoreTypes()
        //{
        //    // Assert
        //    const string sql = "SELECT * FROM \"Employee\";";

        //    const string entityName = "Employee";

        //    Assert.IsNull(table);

        //    // Act
        //    _service.GetMetaData(_conn, entityName, sql);

        //    // Assert
        //    Assert.That(table);

        //    Assert.AreEqual(entityName, table.Name);

        //    Assert.That(table.Fields.Any());
        //}

        [Test]
        public void TestCreateEntityClass()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateEntityClass(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }


        [Test]
        public void TestCreateNewEntity()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateNewEntity(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }


        [Test]
        public void TestCreateMappingFromDbToEntityForDataReader()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateMappingFromDbToEntityForDataReader(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestCreateNewEntityCommand()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateNewEntityCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }


        [Test]
        public void TestCreateUpdateEntityCommand()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateUpdateEntityCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestCreateDeleteEntityCommand()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateDeleteEntityCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestCreateEntityServiceClass()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateEntityServiceClass(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestCreateGetAllEntitiesCommand()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateGetAllEntitiesCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestCreateCountCommand()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateCountCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestCreateGetByIdCommand()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateGetByIdCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }

        [Test]
        public void TestExport()
        {
            // Assert
            var table = _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo(EntityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.ExportAll(table, FileHelper.BackupPath);

            // Assert
            Assert.That(result.Any());

            foreach (var fileName in result)
            {
                Debug.Print(fileName);
            }
        }
    }
}