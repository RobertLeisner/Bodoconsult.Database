// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Dbase.DbReader;
using Bodoconsult.Database.Dbase.Interfaces;

namespace Bodoconsult.Database.Dbase.FileImport;

/// <summary>
/// Base class for import tasks from DBF file
/// </summary>
/// <typeparam name="TData">Target entity type</typeparam>
public class DbfImportBaseTask<TData> : FileImportBaseTask<TData> where TData : class, new()
{

    #region Private Fields

    private readonly IProducerConsumerQueue<List<TData>> _queue;

    #endregion Private Fields

    #region Protected Constructors

    protected DbfImportBaseTask(string name, string dbBasePath, IAppLoggerProxy log, MapToEntityDelegate<TData> mapToEntityDelegate)
        : base(name, dbBasePath, log, mapToEntityDelegate)
    {
        _queue = new ProducerConsumerQueue<List<TData>>();
        _queue.ConsumerTaskDelegate = StoreData;
    }



    #endregion Protected Constructors

    #region Protected Properties

    /// <summary>
    /// The batch size used to send data to the target database
    /// </summary>
    protected int BatchSize { get; set; } = 50;

    #endregion Protected Properties

    #region Public Methods

    public sealed override void Execute()
    {
        try
        {
            var msg = $"{Name}: Run import from {FullPath}";

            Debug.Print(msg);
            Log.LogInformation(msg);

            if (!FileExists)
            {
                msg = $"{Name}: Could not import file {Name}! File does not exists: {FullPath}";
                Debug.Print(msg);
                Log.LogError(msg);
                return;
            }

            // Start the queue now
            _queue.StartConsumer();

            // Get the data and process it
            using (var dbf = new FastDbf(FullPath))
            {
                var count = dbf.RecordCount;

                msg = $"{Name}: DBF contains {count} rows";

                Debug.Print(msg);
                Log.LogInformation(msg);
                var index = 0;
                var entitylist = new List<TData>(BatchSize);

                var insertCount = 0;

                while (index < count)
                {

                    var batch = dbf.Read(index, BatchSize);

                    foreach (var record in batch.DbfRecords)
                    {
                        var entity = MapToEntityDelegate.Invoke(batch, record);

                        if (entity == null)
                        {
                            continue;
                        }

                        insertCount++;

                        entitylist.Add(entity);
                    }


                    var data = entitylist.ToList();
                    _queue.Enqueue(data);
                    batch.Dispose();
                    entitylist.Clear();

                    index += BatchSize;

                    msg = $"{Name}: Inserted {insertCount} rows";

                    Debug.Print(msg);
                    Log.LogInformation(msg);
                }

            }
        }
        catch (Exception e)
        {
            Log.LogError($"{Name}: failed", e);
            throw new Exception($"{Name}: failed", e);
        }


        _queue.StopConsumer();

        Log.LogInformation("{Name}: Import done");
    }

    #endregion Public Methods

}