using System.Linq;
using Bodoconsult.Database.Postgres.Test.Helpers;
using Bodoconsult.Database.Postgres.Test.MetaDataSample;
using NUnit.Framework;

namespace Bodoconsult.Database.Postgres.Test
{
    [TestFixture]
    public class UnitTestMetaDataSample
    {

        private CustomerService _db;

        [SetUp]
        public void Setup()
        {
            var conn = TestHelper.PostgresConnectionString;

            _db = new CustomerService(conn);

        }

        [Test]
        public void TestGetAll()
        {

            // Act
            var result = _db.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());

        }

        [Test]
        public void TestGetById()
        {

            const int id = 28;

            // Act
            var result = _db.GetById(id);

            // Assert
            Assert.IsNotNull(result);

            Assert.AreEqual(id, result.CustomerId);
        }

        [Test]
        public void TestCount()
        {

            // Act
            var result = _db.Count();

            // Assert
            Assert.IsTrue(result>0);

        }


        [Test]
        public void TestUpdate()
        {
            const int id = 28;

            var customer = _db.GetById(id);

            var newName = customer.LastName=="Barnett" ? "Roberts" : "Barnett";

            customer.LastName = newName;

            // Act
            _db.Update(customer);

            // Assert
            var result = _db.GetById(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(newName, result.LastName);

        }


        [Test]
        public void TestAddNew()
        {
            int id = _db.Count()+1;

            var customer = TestDataHelper.NewCustomer();
            customer.CustomerId = id;

            // Act
            _db.AddNew(customer);

            // Assert
            var result = _db.GetById(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(customer.LastName, result.LastName);

        }

        [Test]
        public void TestDelete()
        {
            // Assert
            const int id = -99;
            // Act
            _db.Delete(id);

            // Assert
            Assert.IsTrue(true);

        }
    }
}