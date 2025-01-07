﻿using System.Linq;
using Bodoconsult.Database.SqlClient.Test.Helpers;
using Bodoconsult.Database.SqlClient.Test.MetaDataSample;
using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test
{
    [TestFixture]
    public class UnitTestMetaDataSample
    {

        private CustomerService _db;

        [SetUp]
        public void Setup()
        {
            var conn = TestHelper.SqlServerConnectionString;

            _db = new CustomerService(conn);

        }

        [Test]
        public void TestGetAll()
        {

            // Act
            var result = _db.GetAll();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any());

        }

        [Test]
        public void TestGetById()
        {

            const int id = 28;

            // Act
            var result = _db.GetById(id);

            // Assert
            Assert.That(result, Is.Not.Null);

            Assert.That(result.CustomerId, Is.EqualTo(id));
        }

        [Test]
        public void TestCount()
        {

            // Act
            var result = _db.Count();

            // Assert
            Assert.That(result > 0);

        }


        [Test]
        public void TestUpdate()
        {

            // Assert
            const int id = 28;

            var customer = _db.GetById(id);

            var newName = customer.LastName == "Barnett" ? "Roberts" : "Barnett";

            customer.LastName = newName;

            // Act
            _db.Update(customer);

            // Assert
            var result = _db.GetById(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(newName, Is.EqualTo(result.LastName));

        }


        [Test]
        public void TestAddNew()
        {
            var id = _db.Count() + 1;

            var customer = TestDataHelper.NewCustomer();
            customer.CustomerId = id;

            // Act
            _db.AddNew(customer);

            // Assert
            var result = _db.GetById(id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.LastName, Is.EqualTo(customer.LastName));

        }


        [Test]
        public void TestDelete()
        {
            // Assert
            const int id = -99;
            // Act
            _db.Delete(id);

            // Assert
            Assert.That(true);

        }
    }
}