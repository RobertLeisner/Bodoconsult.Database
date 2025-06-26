// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Dbase.Interfaces;

namespace Bodoconsult.Database.Dbase.FileImport
{
    /// <summary>
    /// Simple import task for DBF file to Result list of entities
    /// </summary>
    /// <typeparam name="TData">Target entity type</typeparam>
    public class DbfImportTask<TData> : DbfImportBaseTask<TData> where TData : class, new()
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public DbfImportTask(string name, string dbBasePath, IAppLoggerProxy log, MapToEntityDelegate<TData> mapToEntityDelegate) : base(name, dbBasePath, log, mapToEntityDelegate)
        { }

        #region Public properties

        /// <summary>
        /// The result of the task
        /// </summary>
        public List<TData> Result { get; } = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// Store the read data from file into target. This method should be called batchwise by <see cref="IFileImportTask{TData}.Execute"/> 
        /// </summary>
        public override void StoreData(List<TData> data)
        {
            Result.AddRange(data);
        }

        #endregion

    }
}
