// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;

namespace Bodoconsult.Database.Interfaces
{

    /// <summary>
    /// Data service for entity backup service providing, formatting and removing entities
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public interface IEntityBackupDataService<T> where T : class
    {
        /// <summary>
        /// Get the data for an entity by date
        /// </summary>
        /// <param name="from">Date from inclusive</param>
        /// <param name="to">Date until exclusive</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Current page index</param>
        /// <returns>List with entities</returns>
        IList<T> GetData(DateTime from, DateTime to, int pageSize, int pageIndex);

        /// <summary>
        /// Remove the entities backuped already
        /// </summary>
        /// <param name="entities">Entities to remove</param>
        void RemoveData(IList<T> entities);

    }
}
