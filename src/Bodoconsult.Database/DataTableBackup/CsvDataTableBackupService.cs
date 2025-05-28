// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Database.DataTableBackup.Formatters;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.DataTableBackup;

/// <summary>
/// Backup data to CSV files
/// </summary>
public class CsvDataTableBackupService : BaseDataTableBackupService
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public CsvDataTableBackupService(IDataTableBackupDataService dataService, string targetFolder, string fileName) :
        base(new DefaultCsvDataTableBackupFormatter())
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }

    /// <summary>
    /// Ctor with individual formatter
    /// </summary>
    public CsvDataTableBackupService(IDataTableBackupDataService dataService, string targetFolder, string fileName, IDataTableBackupFormatter formatter) :
        base(formatter)
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }
}