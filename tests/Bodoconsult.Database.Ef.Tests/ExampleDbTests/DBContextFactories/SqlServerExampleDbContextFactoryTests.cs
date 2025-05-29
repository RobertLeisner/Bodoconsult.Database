// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Database.Test.Utilities.App;
using EfConsoleApp1.Model.DatabaseModel.DbContext;

namespace Bodoconsult.Database.Ef.Tests.ExampleDbTests.DBContextFactories;

[TestFixture]
internal class SqlServerExampleDbContextFactoryTests
{

    [Test]
    public void CreateDbContext_SingleCall_DbContextCreated()
    {
        // Arrange 
        var factory = new SqlServerExampleDbContextFactory(Globals.Instance, Globals.Instance.Logger);

        // Act  
        var instance = factory.CreateDbContext();

        // Assert
        Assert.That(instance, Is.Not.Null);

    }

    [Test]
    public void CreateDbContext_MultipleCalls_MultipleDbContextsCreated()
    {
        // Arrange 
        var factory = new SqlServerExampleDbContextFactory(Globals.Instance, Globals.Instance.Logger);

        // Act  
        var instance1 = factory.CreateDbContext();
        var instance2 = factory.CreateDbContext();
        var instance3 = factory.CreateDbContext();

        // Assert
        Assert.That(instance1, Is.Not.SameAs(instance2));
        Assert.That(instance1, Is.Not.SameAs(instance3));
        Assert.That(instance2, Is.Not.SameAs(instance3));
    }

}