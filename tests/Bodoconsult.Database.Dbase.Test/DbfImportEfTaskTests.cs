// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.Database.Dbase.DbReader;
using Bodoconsult.Database.Dbase.FileImport;
using Bodoconsult.Database.Dbase.Helpers;
using Bodoconsult.Database.Dbase.Test.Infrastructure;
using Bodoconsult.Database.Dbase.Test.TestData;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Infrastructure;
using Bodoconsult.Database.Ef.SqlServer.MigrationTools;
using Bodoconsult.Database.Test.Utilities.App;
using EfConsoleApp1.Model.DatabaseModel.DataMigration;
using EfConsoleApp1.Model.DatabaseModel.DbContext;
using EfConsoleApp1.Model.DatabaseModel.Entities;
using EfConsoleApp1.Model.Infrastructure;

namespace Bodoconsult.Database.Dbase.Test;

public class DbfImportEfTaskTests : BaseDatabaseTests
{
    private static string DbFile => "Article.DBF";

    private readonly string _folderPath;

    private readonly IAppLoggerProxy _logger = new AppLoggerProxy(new FakeLoggerFactory(), new LogDataFactory(), "Blubb");

    private readonly SqlServerExampleDbUnitOfWork _uow;

    private readonly IRepository<Article> _articleRepository;

    public DbfImportEfTaskTests()
    {

        var ass = typeof(DbfImportEfTaskTests).Assembly;

        if (ass == null)
        {
            throw new ArgumentException("Assembly may not be null");
        }

        var dir = new DirectoryInfo(ass.Location).Parent;

        if (dir == null)
        {
            throw new ArgumentException("Assembly parent directory may not be null");
        }

        if (dir.Parent == null)
        {
            throw new ArgumentException("Assembly parent directories may not be null");
        }

        if (dir.Parent.Parent == null)
        {
            throw new ArgumentException("Assembly parent directories may not be null");
        }

        var root = dir.Parent.Parent.Parent;

        if (root == null)
        {
            throw new ArgumentException("Assembly parent directories may not be null");
        }

        _folderPath = Path.Combine(root.FullName, "TestData");

        PrepareServer();

        IAmbientDbContextLocator ambientDbContextLocator = new AmbientDbContextLocator();

        var dbContextFactory = new SqlServerExampleDbContextFactory(Globals.Instance, _logger);
        IContextScopeFactory<SqlServerExampleDbContext> dbContextScopeFactory = new DbContextScopeFactory<SqlServerExampleDbContext>(dbContextFactory);
        IBackupEngine backupEngine = new SqlServerBackupEngine<SqlServerExampleDbContext>(ambientDbContextLocator);

        // Load EF version
        IModelDataConvertersHandlerFactory modelDataConvertersHandlerFactory = new SqlServerExampleDbEfModelDataConvertersHandlerFactory(_logger);

        IMigrationController migrationController = new SqlServerMigrationController<SqlServerExampleDbContext>(modelDataConvertersHandlerFactory, _logger);

        // Act  
        _uow = new SqlServerExampleDbUnitOfWork(dbContextScopeFactory, _logger, ambientDbContextLocator, backupEngine, migrationController);
        _articleRepository = _uow.GetRepository<Article>();

        //// Test
        //var article = new Article
        //{
        //    ArticleName = "Blubb"
        //};

        //using (var scope = _uow.GetContextScope())
        //{
        //    _articleRepository.Add(article);
        //    scope.SaveChanges();
        //}

    }


    //[SetUp]
    //public void Setup()
    //{
    //}

    [OneTimeTearDown]
    public void Clear()
    {
        _logger.Dispose();
        _uow.Dispose();
    }

    [Test]
    public void Ctor_ValidSetup_PropertiesSetCorrectly()
    {

        // Arrange 

        // Act  
        var task = new DbfImportEfTask<Article>(DbFile, _folderPath, _logger, MapToEntityDelegate, _uow);

        // Assert
        Assert.That(task.FolderPath, Is.EqualTo(_folderPath));
        Assert.That(task.Name, Is.EqualTo(DbFile));
        Assert.That(task.FullPath, Is.EqualTo(Path.Combine(_folderPath, DbFile)));
    }

    //[Test]
    //public void Execute_ValidSetupBulkCopy_PropertiesSetCorrectly()
    //{

    //    // Arrange 
    //    ClearArticles();

    //    var task = new DbfImportEfTask<Article>(DbFile, _folderPath, _logger, MapToEntityDelegate, _uow)
    //    {
    //        UseBulkInsert = true,
    //        BatchSize = 50
    //    };
    //    Assert.That(Count(), Is.EqualTo(0));

    //    // Act  
    //    task.Execute();

    //    // Assert
    //    Assert.That(Count(), Is.Not.EqualTo(0));
    //}


    [Test]
    public void Execute_ValidSetupEf_PropertiesSetCorrectly()
    {

        // Arrange 
        ClearArticles();

        var task = new DbfImportEfTask<Article>(DbFile, _folderPath, _logger, MapToEntityDelegate, _uow)
        {
            UseBulkInsert = false,
            BatchSize = 50
        };
        Assert.That(Count(), Is.EqualTo(0));

        // Act  
        task.Execute();

        // Assert
        Assert.That(Count(), Is.Not.EqualTo(0));
    }


    private int Count()
    {
        using (_uow.GetReadOnlyContextScope())
        {
            return _articleRepository.Count();
        }
    }

    private void ClearArticles()
    {
        _uow.RunSql("DELETE FROM Articles;");
    }

    private Article MapToEntityDelegate(DbfResult dbf, List<string> record)
    {
        var result = new Article
        {
            // Do net set ID as it is generated by SqlServer
            //ID = ParseHelper.ParseInt(dbf.ColumnValue(record, "ID")) ?? 0,
            ArticleName = ParseHelper.ProcessString(dbf.ColumnValue(record, "ARTICLE"))
        };

        return result;
    }
}