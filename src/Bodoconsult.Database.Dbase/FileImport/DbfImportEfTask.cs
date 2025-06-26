// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Dbase.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Dbase.FileImport
{
    /// <summary>
    /// Import a DBF file to a SQL database using Bodoconsult.Database.Ef
    /// </summary>
    /// <typeparam name="TData">Target entity type</typeparam>
    public class DbfImportEfTask<TData> : DbfImportBaseTask<TData> where TData : class, IEntityRequirements, new()
    {

        private readonly IUnitOfWork _unitOfWork;

        protected IRepository<TData> Repository;

        /// <summary>
        /// Default ctor
        /// </summary>
        public DbfImportEfTask(string name, string dbBasePath, IAppLoggerProxy log, MapToEntityDelegate<TData> mapToEntityDelegate, IUnitOfWork unitOfWork) : base(name, dbBasePath, log, mapToEntityDelegate)
        {
            _unitOfWork = unitOfWork;
            Repository = _unitOfWork.GetRepository<TData>();
        }

        #region Public Methods

        public override void StoreData(List<TData> data)
        {
            using (var scope = _unitOfWork.GetContextScope(false, false, IsolationLevel.Serializable))
            {
                if (UseBulkInsert)
                {
                    Repository.BulkInsertAll(data);
                }
                else
                {
                    Repository.Add(data);
                    scope.SaveChanges();
                }
            }

            data.Clear();
        }

        #endregion

    }

}
