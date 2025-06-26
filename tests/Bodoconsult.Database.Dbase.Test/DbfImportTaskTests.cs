// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.Database.Dbase.DbReader;
using Bodoconsult.Database.Dbase.FileImport;
using Bodoconsult.Database.Dbase.Helpers;
using Bodoconsult.Database.Dbase.Test.TestData;
using EfConsoleApp1.Model.DatabaseModel.Entities;

namespace Bodoconsult.Database.Dbase.Test
{
    public class DbfImportTaskTests
    {
        private static string DbFile => "Article.DBF";

        private readonly string _folderPath;

        private readonly IAppLoggerProxy _logger = new AppLoggerProxy(new FakeLoggerFactory(), new LogDataFactory(), "Blubb");

        public DbfImportTaskTests()
        {
            var ass = typeof(DbfImportTaskTests).Assembly;

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

            // Act  
            var task = new DbfImportTask<Article>(DbFile, _folderPath, _logger, MapToEntityDelegate);

            // Assert
            Assert.That(task.FolderPath, Is.EqualTo(_folderPath));
            Assert.That(task.Name, Is.EqualTo(DbFile));
            Assert.That(task.FullPath, Is.EqualTo(Path.Combine(_folderPath, DbFile)));
        }

        [Test]
        public void Execute_ValidSetup_PropertiesSetCorrectly()
        {

            // Arrange 
            var task = new DbfImportTask<Article>(DbFile, _folderPath, _logger, MapToEntityDelegate);
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
                ID = ParseHelper.ParseInt(dbf.ColumnValue(record, "ID")) ?? 0,
                ArticleName = ParseHelper.ProcessString(dbf.ColumnValue(record, "ARTICLE"))
            };

            return result;
        }
    }
}