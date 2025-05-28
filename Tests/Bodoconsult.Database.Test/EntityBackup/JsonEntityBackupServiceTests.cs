// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using Bodoconsult.Database.EntityBackup;
using Bodoconsult.Database.Test.Utilities.Helpers;
using Bodoconsult.Database.Test.Utilities.TestData;
using NUnit.Framework;

namespace Bodoconsult.Database.Test.EntityBackup;

[TestFixture]
internal class JsonEntityBackupServiceTests
{


    [Test]
    public void Ctor_ValidSetup_PropersLoadedCorrectly()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        // Act  
        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup");

        // Assert
        Assert.That(backupService, Is.Not.Null);
        Assert.That(backupService.PageSize, Is.Not.EqualTo(0));
        Assert.That(backupService.BackupType, Is.EqualTo(BackupTypeEnum.Daily));
    }

    [Test]
    public void GetFirstStartDate_DailyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Daily
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday

        var expectedResult = new DateTime(2023, 7, 19);

        // Act  
        var result = backupService.GetFirstStartDate(startDate);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetFirstStartDate_YearlyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Yearly
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday

        var expectedResult = new DateTime(2023, 1, 1);

        // Act  
        var result = backupService.GetFirstStartDate(startDate);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetFirstStartDate_MonthlyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Monthly
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday

        var expectedResult = new DateTime(2023, 7, 1);

        // Act  
        var result = backupService.GetFirstStartDate(startDate);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetFirstStartDate_WeeklyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Weekly
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday
        var expectedResult = new DateTime(2023, 7, 16);

        // Act  
        var result = backupService.GetFirstStartDate(startDate);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetNextStartDate_DailyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Daily
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday
        var to = DateTime.Now.AddDays(100);
        var expectedResult = new DateTime(2023, 7, 20);

        // Act  
        var result = backupService.GetNextStartDate(startDate, to);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetNextStartDate_YearlyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Yearly
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday
        var to = DateTime.Now.AddDays(500);
        var expectedResult = new DateTime(2024, 1, 1);

        // Act  
        var result = backupService.GetNextStartDate(startDate, to);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GetNextStartDate_MonthlyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Monthly
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday
        var to = DateTime.Now.AddDays(100);
        var expectedResult = new DateTime(2023, 8, 1);

        // Act  
        var result = backupService.GetNextStartDate(startDate, to);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }



    [Test]
    public void GetNextStartDate_WeeklyWednesday_ReturnsDate()
    {
        // Arrange 
        var dataService = new DemoEntityEntityBackupDataService();

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup")
        {
            BackupType = BackupTypeEnum.Weekly
        };

        var startDate = new DateTime(2023, 7, 19, 19, 0, 0); // Wednesday
        var to = DateTime.Now.AddDays(100);
        var expectedResult = new DateTime(2023, 7, 23);

        // Act  
        var result = backupService.GetNextStartDate(startDate, to);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void Backup_ValidSetup_FileCreated()
    {
        // Arrange 
        var from = new DateTime(2023, 7, 19);
        var to = new DateTime(2023, 7, 22);


        var dataService = new DemoEntityEntityBackupDataService();
        TestHelper.GetDataForService(dataService.DemoEntities, from, to);

        var backupService = new JsonEntityBackupService<DemoEntity>(dataService, FileHelper.EntityBackupPath, "TestBackup");

        // Act  
        backupService.Backup(from, to);

        // Assert
        Assert.That(backupService.FilesWritten, Is.EqualTo(2));
    }


}