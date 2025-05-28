// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Database.SqlClient.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test.HelperTests
{
    [TestFixture]
    internal class UnitTestResourceHelperSql
    {

        [Test]
        public void TestGetTextResource()
        {
            // Arrange 

            // Act  
            var result = ResourceHelper.GetTextResource("test");

            // Assert
            Assert.That(!string.IsNullOrEmpty(result));

        }

        [Test]
        public void TestGetSqlResource()
        {
            // Arrange 

            // Act  
            var result = ResourceHelper.GetSqlResource("GetTraceData");

            // Assert
            Assert.That(!string.IsNullOrEmpty(result));

        }

    }
}