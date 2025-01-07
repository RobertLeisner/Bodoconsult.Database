// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using System.IO;
using System.Text;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.EntityBackup;

/// <summary>
/// Base class for <see cref="IEntityBackupService&lt;T&gt;"/> implementations
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseEntityBackupService<T> : IEntityBackupService<T> where T : class
{
    /// <summary>
    ///  Default ctor
    /// </summary>
    /// <param name="formatter"></param>
    protected BaseEntityBackupService(IEntityBackupFormatter<T> formatter)
    {
        Formatter=formatter;
    }

    /// <summary>
    ///  Describes the manner how the data to backup are written in backup files. Daily means the data for one day is written in one file. Default: daily
    /// </summary>
    public BackupTypeEnum BackupType { get; set; }

    /// <summary>
    /// Current page size for saving data
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Target folder to store the backup files in
    /// </summary>
    public string TargetFolder { get; protected set; }

    /// <summary>
    /// File name for the backup files. Start and end date will be added to the filename
    /// </summary>
    public string FileName { get; protected set; }

    /// <summary>
    /// The number of files written during the backup process
    /// </summary>
    public int FilesWritten { get; protected set; }

    /// <summary>
    /// The current data service to provide the data for the backup
    /// </summary>
    public IEntityBackupDataService<T> DataService { get; protected set; }

    /// <summary>
    /// Current formatter to use
    /// </summary>
    public IEntityBackupFormatter<T> Formatter { get; }


    /// <summary>
    /// Main method to start the backup process
    /// </summary>
    /// <param name="from">Start date inclusive</param>
    /// <param name="to">End date exclusive</param>
    public virtual void Backup(DateTime from, DateTime to)
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
    /// <param name="fileName">Filename the result should be appended to</param>
    /// <returns>Numbe rof rows affected</returns>
    public virtual int Backup(DateTime from, DateTime to, int pageIndex, string fileName)
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

    /// <summary>
    /// Get the corrected first start date for the next interval for the given <see cref="BackupType"/>
    /// </summary>
    /// <param name="lastStartDate">Last start date</param>
    /// <returns>The next valid start date or null</returns>
    public virtual DateTime? GetFirstStartDate(DateTime lastStartDate)
    {

        var nextStartDate = new DateTime(lastStartDate.Year, lastStartDate.Month, lastStartDate.Day);

        switch (BackupType)
        {
            case BackupTypeEnum.Daily:
                return nextStartDate;
            case BackupTypeEnum.Weekly:
                var day = nextStartDate.DayOfWeek;
                return nextStartDate.AddDays(-(int)day);
            case BackupTypeEnum.Monthly:
                return new DateTime(nextStartDate.Year, lastStartDate.Month, 1);
            case BackupTypeEnum.Yearly:
                return new DateTime(nextStartDate.Year, 1, 1);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Get the start date for the next interval for the given <see cref="BackupType"/>
    /// </summary>
    /// <param name="lastStartDate">Last start date</param>
    /// <param name="to">End date (start date has to be smaller then end date)</param>
    /// <returns>The next valid start date or null</returns>
    public virtual DateTime? GetNextStartDate(DateTime? lastStartDate, DateTime to)
    {
        if (lastStartDate == null)
        {
            return null;
        }

        var nextStartDate = new DateTime(lastStartDate.Value.Year, lastStartDate.Value.Month, lastStartDate.Value.Day);

        switch (BackupType)
        {
            case BackupTypeEnum.Daily:
                nextStartDate = nextStartDate.AddDays(1);
                break;
            case BackupTypeEnum.Weekly:
                nextStartDate = nextStartDate.AddDays(7 - (int)nextStartDate.DayOfWeek);
                break;
            case BackupTypeEnum.Monthly:
                nextStartDate = new DateTime(nextStartDate.Year, nextStartDate.Month + 1, 1);
                break;
            case BackupTypeEnum.Yearly:
                nextStartDate = new DateTime(nextStartDate.Year + 1, 1, 1);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return nextStartDate > to ? null : nextStartDate;
    }

    protected string GetFileName(DateTime from, DateTime to)
    {
        return Path.Combine(TargetFolder, BackupType == BackupTypeEnum.Daily ?
            $"{FileName}_{from:yyyyMMdd}.csv" :
            $"{FileName}_{from:yyyyMMdd}-{to:yyyyMMdd}.csv");
    }

}