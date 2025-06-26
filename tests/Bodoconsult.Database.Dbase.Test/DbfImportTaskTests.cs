// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.Database.Dbase.DbReader;
using Bodoconsult.Database.Dbase.FileImport;
using Bodoconsult.Database.Dbase.Helpers;
using Bodoconsult.Database.Dbase.Test.TestData;

namespace Bodoconsult.Database.Dbase.Test
{
    public class DbfImportTaskTests
    {

        string DbFile => "Article.DBF";

        private readonly string _folderPath;

        private readonly IAppLoggerProxy _logger = new AppLoggerProxy(new FakeLoggerFactory(), new LogDataFactory(), "Blubb");

        public DbfImportTaskTests()
        {
            var ass = typeof(DbfImportTaskTests).Assembly;

            var root = new DirectoryInfo(ass.Location).Parent.Parent.Parent.Parent;

            _folderPath = Path.Combine(root.FullName, "TestData");
        }


        //[SetUp]
        //public void Setup()
        //{
        //}

        [OneTimeTearDown]
        public void Clear()
        {
            _logger.Dispose();
        }

        [Test]
        public void Ctor_ValidSetup_PropertiesSetCorrectly()
        {

            // Arrange 
            var fileName = "Article.DBF";

            // Act  
            var task = new DbfImportTask<Article>( fileName, _folderPath, _logger, MapToEntityDelegate);

            // Assert
            Assert.That(task.FolderPath, Is.EqualTo(_folderPath));
            Assert.That(task.Name, Is.EqualTo(fileName));
            Assert.That(task.FullPath, Is.EqualTo(Path.Combine(_folderPath, fileName)));
        }

        [Test]
        public void Execute_ValidSetup_PropertiesSetCorrectly()
        {

            // Arrange 
            var fileName = "Article.DBF";
            var task = new DbfImportTask<Article>(fileName, _folderPath, _logger, MapToEntityDelegate);
            Assert.That(task.Result.Count, Is.EqualTo(0));

            // Act  
            task.Execute();

            // Assert
            Assert.That(task.Result.Count, Is.Not.EqualTo(0));
        }

        private Article MapToEntityDelegate(DbfResult dbf, List<string> record)
        {
            var result = new Article
            {
                Id = ParseHelper.ParseInt(dbf.ColumnValue(record, "ID")) ?? 0,
                ArticleName = ParseHelper.ProcessString(dbf.ColumnValue(record, "ARTICLE"))
            };

            return result;
        }
    }
}