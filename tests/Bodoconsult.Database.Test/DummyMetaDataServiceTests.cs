using System.Diagnostics;
using System.Linq;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Test.MetaData;
using Bodoconsult.Database.Test.Utilities.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.Test
{

    [TestFixture]
    internal class DummyMetaDataServiceTests
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

            // Act
            var table  = _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            // Assert
            Assert.That(table.Name, Is.EqualTo(entityName));

            Assert.That(table.Fields.Any());
        }

        [Test]
        public void TestCreateEntityClass()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            var table = _service.GetMetaData(Conn, entityName, sql);

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            var table = _service.GetMetaData(Conn, entityName, sql);

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";


            var table = _service.GetMetaData(Conn, entityName, sql);

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";
    

            var table = _service.GetMetaData(Conn, entityName, sql);

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            var table = _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.That(table.Name, Is.EqualTo(entityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.CreateUpdateEntityCommand(table);

            // Assert
            Assert.That(string.IsNullOrEmpty(result), Is.False);

            Debug.Print(result);
        }



        [Test]
        public void TestCreateEntityServiceClass()
        {
            // Assert
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            var table = _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            var table = _service.GetMetaData(Conn, entityName, sql);

            

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            var table = _service.GetMetaData(Conn, entityName, sql);

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";


            var table = _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            

            Assert.That(table.Name, Is.EqualTo(entityName));

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
            const string sql = "SELECT * FROM \"Customer\";";

            const string entityName = "Customer";

            const string primaryKeyField = "CustomerId";

            var table = _service.GetMetaData(Conn, entityName, sql, primaryKeyField);

            Assert.That(table.Name, Is.EqualTo(entityName));

            Assert.That(table.Fields.Any());

            // Act
            var result = _service.ExportAll(table, FileHelper.EntityBackupPath);

            // Assert
            Assert.That(result.Any());

            foreach (var fileName in result)
            {
                Debug.Print(fileName);
            }
        }
    }

}

