// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.Database.DataTableBackup.Formatters;
using Bodoconsult.Database.Test.Utilities.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.Test.DataTableBackup.Formatters
{
    [TestFixture]
    internal class DefaultCsvDataTableBackupFormatterTests
    {


        [Test]
        public void LoadData_ValidDataTable_DataTableLoaded()
        {
            // Arrange 
            var f = new DefaultCsvDataTableBackupFormatter();

            var data = TestHelper.GetDataTableForTests();

            // Act  
            f.LoadData(data);

            // Assert
            Assert.That(f.Data, Is.SameAs(data));

        }


        [Test]
        public void GetResult_ValidDataTable_ReturnsString()
        {
            // Arrange 
            var f = new DefaultCsvDataTableBackupFormatter();

            var data = TestHelper.GetDataTableForTests();

            f.LoadData(data);

            // Act  
            var result = f.GetResult();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.Not.EqualTo(0));

            Debug.Print(result.ToString());
        }
        
    }

    [TestFixture]
    internal class DefaultXmlDataTableBackupFormatterTests
    {


        [Test]
        public void LoadData_ValidDataTable_DataTableLoaded()
        {
            // Arrange 
            var f = new DefaultXmlDataTableBackupFormatter();

            var data = TestHelper.GetDataTableForTests();

            // Act  
            f.LoadData(data);

            // Assert
            Assert.That(f.Data, Is.SameAs(data));

        }


        [Test]
        public void GetResult_ValidDataTable_ReturnsString()
        {
            // Arrange 
            var f = new DefaultXmlDataTableBackupFormatter();

            var data = TestHelper.GetDataTableForTests();

            f.LoadData(data);

            // Act  
            var result = f.GetResult();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.Not.EqualTo(0));

            Debug.Print(result.ToString());
        }

    }
}
