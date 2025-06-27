Bodoconsult.Database.Dbase
================

# What does the library

Bodoconsult.Database.Dbase allows reading data from DBase or Clipper DBF database files.

# How to use the library

This sample is taken from the implementation Bodoconsult.Database.SqlClient.

The source code contain a NUnit test classes, the following source code is extracted from. The samples below show the most helpful use cases for the library.

# How to use the Bodoconsult.Database.Dbase

## Reading DBF file into list of entities

``` csharp
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
```

## Reading DBF file in an environment using Bodoconsult.Database.Ef and Entity Framework

``` csharp
[Test]
public void Execute_ValidSetupEf_PropertiesSetCorrectly()
{
    // Arrange 
    ClearArticles();

    var task = new DbfImportEfTask<Article>(DbfFileName, _folderPath, _logger, MapToEntityDelegate, _unitOfWork)
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
```

RowVersion fields and integer primary key fields cannot be transferred using DbfImportEfTask<TData\> by default. Inserting existing integer primary keys is possible by bringing the database in a mode allowing this and turning of this mode later by SqlServer specific SQL commands. Fields to not transfer can set by addig their names to Repository<TData\>.FieldsNotAllowedForBulkCopy or RepositoryGuid<TData\>.FieldsNotAllowedForBulkCopy list. By default the fields RowVersion and Id are excluded.

## Reading DBF file in an environment using Bodoconsult.Database and plain System.Data

To store the read DBF file data to another database using Bodoconsult.Database you have to implement a super class derived from DbfImportBaseTask\<TData\> overriding the method StoreData(List\<TData\> data) with the code required to save the data to your data table by using plain SQL or ADO.NET.

# About us

Bodoconsult (<http://www.bodoconsult.de>) is a Munich based software development company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.

