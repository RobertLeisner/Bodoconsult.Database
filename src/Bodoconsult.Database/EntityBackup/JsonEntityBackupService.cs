// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.IO;
using System.Text;
using Bodoconsult.Database.EntityBackup.Formatters;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.EntityBackup;

/// <summary>
/// Backup data to XML files
/// </summary>
/// <typeparam name="T">Type to backup</typeparam>
public class JsonEntityBackupService<T> : BaseEntityBackupService<T> where T : class
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public JsonEntityBackupService(IEntityBackupDataService<T> dataService, string targetFolder, string fileName) :
        base(new DefaultJsonEntityBackupFormatter<T>())
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }

    /// <summary>
    /// Ctor with individual formatter
    /// </summary>
    public JsonEntityBackupService(IEntityBackupDataService<T> dataService, string targetFolder, string fileName, IEntityBackupFormatter<T> formatter) :
        base(formatter)
    {
        DataService = dataService;
        TargetFolder = targetFolder;
        FileName = fileName;
    }

    /// <summary>
    /// Main method to start the backup process
    /// </summary>
    /// <param name="from">Start date inclusive</param>
    /// <param name="to">End date exclusive</param>
    public override void Backup(DateTime from, DateTime to)
    {
        var pageIndex = 1;

        var startDate = GetFirstStartDate(from);

        var endDate = GetNextStartDate(startDate, to);

        while (true)
        {

            if (!startDate.HasValue)
            {
                break;
            }

            if (!endDate.HasValue)
            {
                break;
            }

            var fileName = GetFileName(startDate.Value, endDate.Value);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            while (true)
            {
                if (Backup(startDate.Value, endDate.Value, pageIndex, fileName) == 0)
                {
                    if (File.Exists(fileName))
                    {
                        FilesWritten++;
                    }
                    break;
                }

                pageIndex++;
            }

            startDate = endDate;
            endDate = GetNextStartDate(startDate, to);

        }
    }

    /// <summary>
    /// Main method to start the backup process to CSV files
    /// </summary>
    /// <remarks>Method public for unit testing. Do not use directly</remarks>
    /// <param name="from">Start date inclusive</param>
    /// <param name="to">End date exclusive</param>
    /// <param name="pageIndex">Current page index</param>
    /// <param name="fileName"></param>
    /// <returns>Number of rows affected</returns>
    public override int Backup(DateTime from, DateTime to, int pageIndex, string fileName)
    {

        var data = DataService.GetData(from, to, PageSize, pageIndex);

        

        var count = data.Count;

        if (count == 0)
        {
            return 0;
        }

        Formatter.LoadData(data);

        var sb = Formatter.GetResult();

        File.AppendAllText(fileName, sb.ToString(), Encoding.UTF8);

        DataService.RemoveData(data);



        return count;
    }
}