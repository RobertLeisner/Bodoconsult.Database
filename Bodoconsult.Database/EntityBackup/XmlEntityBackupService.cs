// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.Database.EntityBackup.Formatters;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.EntityBackup;

/// <summary>
/// Backup data to XML files
/// </summary>
/// <typeparam name="T">Type to backup</typeparam>
public class XmlEntityBackupService<T> : BaseEntityBackupService<T> where T : class
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public XmlEntityBackupService(IEntityBackupDataService<T> dataService, string targetFolder, string fileName) :
        base(new DefaultXmlEntityBackupFormatter<T>())
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }

    /// <summary>
    /// Ctor with individual formatter
    /// </summary>
    public XmlEntityBackupService(IEntityBackupDataService<T> dataService, string targetFolder, string fileName, IEntityBackupFormatter<T> formatter) :
        base(formatter)
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }
}