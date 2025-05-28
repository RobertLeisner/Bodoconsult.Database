// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Database.Interfaces;
using System;
using System.Data;
using Bodoconsult.Database.Test.Utilities.Helpers;

namespace Bodoconsult.Database.Test.DataTableBackup
{
    /// <summary>
    /// Demo implementation of <see cref="IDataTableBackupDataService"/> for testing purposes
    /// </summary>
    internal class DemoDataTableBackupDataService : IDataTableBackupDataService
    {

        public DemoDataTableBackupDataService()
        {
            Data = TestHelper.CreateEmptyDataTable();
        }

        public DataTable Data { get; set; }

        /// <summary>
        /// Get the data for an entity by date
        /// </summary>
        /// <param name="from">Date from inclusive</param>
        /// <param name="to">Date until exclusive</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Current page index</param>
        /// <returns>List with entities</returns>
        public DataTable GetData(DateTime from, DateTime to, int pageSize, int pageIndex)
        {
            return Data;
        }

        /// <summary>
        /// Remove the entities backuped already
        /// </summary>
        /// <param name="entities">Entities to remove</param>
        public void RemoveData(DataTable entities)
        {
            // Do nothing
        }
    }
}
