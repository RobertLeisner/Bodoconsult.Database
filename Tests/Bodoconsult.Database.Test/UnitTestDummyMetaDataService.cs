using System.Diagnostics;
using System.Linq;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Test.MetaData;
using NUnit.Framework;

namespace Bodoconsult.Database.Test
{

    [TestFixture]
    class UnitTestDummyMetaDataService
    {


        private IMetaDataService _service;

        private const string Conn = "ConnectionString";

        [SetUp]
        public void Setup()
        {
            _service = new DummyMetaDataService();

        }

        [Test]
        public void TestGetMetaData()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            Assert.IsNull(_service.Table);

            // Act
            _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            // Assert
            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());
        }

        [Test]
        public void TestCreateEntityClass()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateEntityClass();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateNewEntity()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateNewEntity();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateMappingFromDbToEntityForDataReader()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateMappingFromDbToEntityForDataReader();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateNewEntityCommand()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateNewEntityCommand();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }


        [Test]
        public void TestCreateUpdateEntityCommand()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateUpdateEntityCommand();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }



        [Test]
        public void TestCreateEntityServiceClass()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateEntityServiceClass();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateGetAllEntitiesCommand()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateGetAllEntitiesCommand();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateCountCommand()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateCountCommand();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }

        [Test]
        public void TestCreateGetByIdCommand()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";


            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.CreateGetByIdCommand();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));

            Debug.Print(result);
        }

        [Test]
        public void TestExport()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            const string targetPath = @"D:\temp";


            Assert.IsNull(_service.Table);

            _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.IsNotNull(_service.Table);

            Assert.AreEqual(entityName, _service.Table.Name);

            Assert.IsTrue(_service.Table.Fields.Any());

            // Act
            var result = _service.ExportAll(targetPath);

            // Assert
            Assert.IsTrue(result.Any());

            foreach (var fileName in result)
            {
                Debug.Print(fileName);
            }
        }
    }

}

