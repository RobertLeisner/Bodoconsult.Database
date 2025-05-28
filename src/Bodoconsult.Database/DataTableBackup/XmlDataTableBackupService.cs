// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Database.DataTableBackup.Formatters;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.DataTableBackup;

/// <summary>
/// Backup data to XML files
/// </summary>
public class XmlDataTableBackupService : BaseDataTableBackupService
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public XmlDataTableBackupService(IDataTableBackupDataService dataService, string targetFolder, string fileName) :
        base(new DefaultXmlDataTableBackupFormatter())
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }

    /// <summary>
    /// Ctor with individual formatter
    /// </summary>
    public XmlDataTableBackupService(IDataTableBackupDataService dataService, string targetFolder, string fileName, IDataTableBackupFormatter formatter) :
        base(formatter)
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }
}