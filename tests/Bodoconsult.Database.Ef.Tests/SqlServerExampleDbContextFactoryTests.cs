// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using EfConsoleApp1.Model.DatabaseModel.DbContext;
using Bodoconsult.Database.Test.Utilities.App;
using Bodoconsult.Database.Ef.Tests.Infrastructure;

namespace Bodoconsult.Database.Ef.Tests
{
    public class SqlServerExampleDbContextFactoryTests : BaseDatabaseTests
    {
        [SetUp]
        public void Setup()
        {
            PrepareServer();
        }

        [Test]
        public void CreateDbContext_ValidSetup_DbContextCreated()
        {
            // Arrange 
            var config = Globals.Instance.ContextConfig;

            // ToDo: bug in Bodoconsult.App remove with 1.0.4
            config.ConnectionString = Globals.Instance.AppStartParameter.DefaultConnectionString;

            var factory = new SqlServerExampleDbContextFactory(Globals.Instance, Globals.Instance.Logger);

            // Act  
            var instance = factory.CreateDbContext();

            // Assert
            Assert.That(instance, Is.Not.Null);

        }

    }
}
