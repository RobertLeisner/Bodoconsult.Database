// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Dbase.Interfaces;

namespace Bodoconsult.Database.Dbase.FileImport;

public abstract class FileImportBaseTask<TData>: IFileImportTask<TData> where TData : class, new()
{
    #region Public Properties

    /// <summary>
    /// Delegate to map a record to an entity
    /// </summary>
    public MapToEntityDelegate<TData> MapToEntityDelegate { get; }

    /// <summary>
    /// Current logger
    /// </summary>
    public IAppLoggerProxy Log { get; }


    /// <summary>
    /// Store the read data from file into target. This method should be called batchwise by <see cref="IFileImportTask{TData}.Execute"/> 
    /// </summary>
    public virtual void StoreData(List<TData> data)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Does the file to import exist
    /// </summary>
    /// <returns>True if the file to import exists else false</returns>
    public bool FileExists { get; }


    /// <summary>
    /// Name of the file to import
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Current folder path the file to import is in
    /// </summary>
    public string FolderPath { get; }

    /// <summary>
    /// The full path to file to import
    /// </summary>
    public string FullPath { get; }

    /// <summary>
    /// Weight of the task
    /// 0 = task runs in front of all others
    /// 1 = low data load
    /// 2 = medium data load
    /// 3 = high data load
    /// </summary>
    public int Weight { get; set; } = 1;

    /// <summary>
    /// Use SQL bulk insert instead of EF. Default: false
    /// </summary>
    public bool UseBulkInsert { get; set; } = false;


    #endregion Public Properties

    #region Construction

    /// <summary>
    ///     Full constructor.
    /// </summary>
    /// <param name="name">Name of the file to import</param>
    /// <param name="folderPath">Current folder path the file is in</param>
    /// <param name="log">Current logger instance</param>
    /// <param name="mapToEntityDelegate">Delegate to map a record to an entity</param>
    protected FileImportBaseTask(string name, string folderPath, IAppLoggerProxy log, MapToEntityDelegate<TData> mapToEntityDelegate)
    {
        Name = name;
        FolderPath = folderPath ?? throw new ArgumentNullException(nameof(folderPath));
        Log = log;

        FullPath = Path.Combine(folderPath, name);
        FileExists = File.Exists(FullPath);
        MapToEntityDelegate = mapToEntityDelegate;
    }

    #endregion Construction

    public abstract void Execute();

    /// <summary>
    /// Count errors
    /// </summary>
    public int Errors { get; set; }

    /// <summary>
    /// Count warnings
    /// </summary>
    public int Warnings { get; set; }


}