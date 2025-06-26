// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Database.Ef.Test.Infrastructure;
using Bodoconsult.Database.Test.Utilities.App;
using EfConsoleApp1.Model.DatabaseModel.DbContext;

namespace Bodoconsult.Database.Ef.Test;

public class SqlServerDesignTimeExampleDbContextFactoryTests: BaseDatabaseTests
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

        var factory = new SqlServerDesignTimeExampleDbContextFactory();

        // Act  
        var instance = factory.CreateDbContext([]);

        // Assert
        Assert.That(instance, Is.Not.Null);

    }

}