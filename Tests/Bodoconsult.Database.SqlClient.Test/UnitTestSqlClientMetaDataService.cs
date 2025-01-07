// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Linq;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.SqlClient.MetaData;
using Bodoconsult.Database.SqlClient.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test
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
    public class UnitTestSqlClientMetaDataService
    {

        private IMetaDataService _service;

        private string _conn;

        private const string Sql = "SELECT * FROM [Customer];";

        private const string EntityName = "Customer";

        private const string PrimaryKeyField = "CustomerId";

        private const string TargetPath = @"D:\temp";

        [SetUp]
        public void Setup()
        {
            _conn = TestHelper.SqlServerConnectionString;

            _service = new SqlClientMetaDataService();
        }

        [Test]
        public void TestGetMetaData()
        {
            // Assert
            Assert.That(_service.Table, Is.Not.Null);

            // Act
            _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            // Assert
            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo( EntityName) );

            Assert.That(_service.Table.Fields.Any());
        }


        //[Test]
        //public void TestGetMetaDataMoreTypes()
        //{
        //    // Assert
        //    const string sql = "SELECT * FROM [Employee];";

        //    const string entityName = "Employee";

        //    Assert.IsNull(_service.Table);

        //    // Act
        //    _service.GetMetaData(_conn, entityName, sql);

        //    // Assert
        //    Assert.That(_service.Table);

        //    Assert.AreEqual(entityName, _service.Table.Name);

        //    Assert.That(_service.Table.Fields.Any());
        //}

        [Test]
        public void TestCreateEntityClass()
        {
            // Assert
            Assert.That(_service.Table,Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateEntityClass();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateNewEntity()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateNewEntity();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateMappingFromDbToEntityForDataReader()
        {
            // Assert
            Assert.That(_service.Table, Is.Not.Null);

            _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateMappingFromDbToEntityForDataReader();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateNewEntityCommand()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateNewEntityCommand();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateUpdateEntityCommand()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateUpdateEntityCommand();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateDeleteEntityCommand()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateDeleteEntityCommand();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateEntityServiceClass()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateEntityServiceClass();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateGetAllEntitiesCommand()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateGetAllEntitiesCommand();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateCountCommand()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateCountCommand();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateGetByIdCommand()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateGetByIdCommand();

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.EqualTo(false));

            Debug.Print(result);
        }

        [Test]
        public void TestExport()
        {
            // Assert
            Assert.That(_service.Table, Is.Null);

            _service.GetMetaData(_conn, EntityName, Sql, PrimaryKeyField);

            Assert.That(_service.Table, Is.Not.Null);

            Assert.That(_service.Table.Name, Is.EqualTo(EntityName));

            Assert.That(_service.Table.Fields.Any());

            // Act
            var result = _service.ExportAll(TargetPath);

            // Assert
            Assert.That(result.Any());

            foreach (var fileName in result)
            {
                Debug.Print(fileName);
            }
        }
    }
}