// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.DependencyInjection;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Infrastructure;
using Bodoconsult.Database.Ef.SqlServer.MigrationTools;
using Bodoconsult.Database.Ef.Tests.Infrastructure;
using Bodoconsult.Database.Test.Utilities.App;
using Bodoconsult.Database.Test.Utilities.Helpers;
using EfConsoleApp1.Model.DatabaseModel.DataMigration;
using EfConsoleApp1.Model.DatabaseModel.DbContext;
using EfConsoleApp1.Model.DatabaseModel.Entities;
using EfConsoleApp1.Model.Infrastructure;

namespace Bodoconsult.Database.Ef.Tests.MigrationTools;

public class MigrationControllerTests: BaseDatabaseTests
{
    private readonly string _backupPath;

    private readonly IAppLoggerProxy _logger = Globals.Instance.Logger;

    public MigrationControllerTests()
    {
        var dir = new DirectoryInfo(Globals.Instance.AppStartParameter.AppPath).Parent.Parent.Parent;

        _backupPath = Path.Combine(dir.FullName, "Testdata\\Databases\\ExampleDbTests_V100.bak");
        DataPath = FileHelper.DataPath;
    }




    [SetUp]
    public void Setup()
    {
        Globals.Instance.AppStartParameter.BackupPath = FileHelper.BackupPath;
        RestoreDatabase(_backupPath);
    }

    [OneTimeTearDown]
    public void OneTimeCleanup()
    {
        PrepareServer();
    }

    [Test]
    public void StartMigrationWorkFlow_ManualSetupEfConverters_MigrationSuccessful()
    {
        // Arrange 
        IAmbientDbContextLocator ambientDbContextLocator = new AmbientDbContextLocator();

        var dbContextFactory = new SqlServerExampleDbContextFactory(Globals.Instance, _logger);
        IContextScopeFactory<SqlServerExampleDbContext> dbContextScopeFactory = new DbContextScopeFactory<SqlServerExampleDbContext>(dbContextFactory);
        IBackupEngine backupEngine = new SqlServerBackupEngine<SqlServerExampleDbContext>(ambientDbContextLocator);

        // Load EF version
        IModelDataConvertersHandlerFactory modelDataConvertersHandlerFactory = new SqlServerExampleDbEfModelDataConvertersHandlerFactory(_logger);

        IMigrationController migrationController = new SqlServerMigrationController<SqlServerExampleDbContext>(modelDataConvertersHandlerFactory, _logger);

        // Act  
        var uow = new SqlServerExampleDbUnitOfWork(dbContextScopeFactory, _logger, ambientDbContextLocator, backupEngine, migrationController);

        // Assert
        using (uow.GetReadOnlyContextScope())
        {
            var repo = uow.GetRepository<Users>();
            var result = repo.Any(x => x.UserTypeId > 0);
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void StartMigrationWorkFlow_ManualSetupSqlConverters_MigrationSuccessful()
    {
        // Arrange 
        IAmbientDbContextLocator ambientDbContextLocator = new AmbientDbContextLocator();

        var dbContextFactory = new SqlServerExampleDbContextFactory(Globals.Instance, _logger);
        IContextScopeFactory<SqlServerExampleDbContext> dbContextScopeFactory = new DbContextScopeFactory<SqlServerExampleDbContext>(dbContextFactory);
        IBackupEngine backupEngine = new SqlServerBackupEngine<SqlServerExampleDbContext>(ambientDbContextLocator);

        // Load SQL version
        IModelDataConvertersHandlerFactory modelDataConvertersHandlerFactory = new SqlServerExampleDbSqlModelDataConvertersHandlerFactory(_logger);

        IMigrationController migrationController = new SqlServerMigrationController<SqlServerExampleDbContext>(modelDataConvertersHandlerFactory, _logger);

        // Act  

        var uow = new SqlServerExampleDbUnitOfWork(dbContextScopeFactory, _logger, ambientDbContextLocator, backupEngine,
            migrationController);

        // Assert
        using (uow.GetReadOnlyContextScope())
        {
            var repo = uow.GetRepository<Users>();
            var result = repo.Any(x => x.UserTypeId > 0);
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void StartMigrationWorkFlow_DiSetupEfConverters_MigrationSuccessful()
    {
        // Arrange 
        var di = new DiContainer();
        di.AddSingleton(Globals.Instance.Logger);
        di.AddSingleton((IAppGlobals)Globals.Instance);
        di.AddSingleton((IAppGlobalsWithDatabase)Globals.Instance);
        di.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();
        di.AddSingleton<IDbContextWithConfigFactory<SqlServerExampleDbContext>, SqlServerExampleDbContextFactory>();
        di.AddSingleton<IContextScopeFactory<SqlServerExampleDbContext>, DbContextScopeFactory<SqlServerExampleDbContext>>();
        di.AddSingleton<IBackupEngine, SqlServerBackupEngine<SqlServerExampleDbContext>>();
        di.AddSingleton<IModelDataConvertersHandlerFactory, SqlServerExampleDbEfModelDataConvertersHandlerFactory>();
        di.AddSingleton<IMigrationController, SqlServerMigrationController<SqlServerExampleDbContext>>();
        di.AddSingleton<IUnitOfWork, SqlServerExampleDbUnitOfWork>();

        di.BuildServiceProvider();

        // Act  
        var uow = di.Get<IUnitOfWork>();

        // Assert
        using (uow.GetReadOnlyContextScope())
        {
            var repo = uow.GetRepository<Users>();
            var result = repo.Any(x => x.UserTypeId > 0);
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void StartMigrationWorkFlow_DiSetupSqlConverters_MigrationSuccessful()
    {
        // Arrange 
        var di = new DiContainer();
        di.AddSingleton(Globals.Instance.Logger);
        di.AddSingleton((IAppGlobals)Globals.Instance);
        di.AddSingleton((IAppGlobalsWithDatabase)Globals.Instance);
        di.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();
        di.AddSingleton<IDbContextWithConfigFactory<SqlServerExampleDbContext>, SqlServerExampleDbContextFactory>();
        di.AddSingleton<IContextScopeFactory<SqlServerExampleDbContext>, DbContextScopeFactory<SqlServerExampleDbContext>>();
        di.AddSingleton<IBackupEngine, SqlServerBackupEngine<SqlServerExampleDbContext>>();
        di.AddSingleton<IModelDataConvertersHandlerFactory, SqlServerExampleDbSqlModelDataConvertersHandlerFactory>();
        di.AddSingleton<IMigrationController, SqlServerMigrationController<SqlServerExampleDbContext>>();
        di.AddSingleton<IUnitOfWork, SqlServerExampleDbUnitOfWork>();

        di.BuildServiceProvider();

        // Act  
        var uow = di.Get<IUnitOfWork>();

        // Assert
        using (uow.GetReadOnlyContextScope())
        {
            var repo = uow.GetRepository<Users>();
            var result = repo.Any(x => x.UserTypeId > 0);
            Assert.That(result, Is.True);
        }
    }

}