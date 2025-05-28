// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.EntityBackup
{

    /// <summary>
    /// Base class for <see cref="IEntityBackupDataService&lt;T&gt;"/> implementations
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public class BaseEntityBackupDataService<T> : IEntityBackupDataService<T> where T : class
    {
        /// <summary>
        /// Get the data for an entity by date
        /// </summary>
        /// <param name="from">Date from inclusive</param>
        /// <param name="to">Date until exclusive</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageIndex">Current page index</param>
        /// <returns>List with entities</returns>
        public virtual IList<T> GetData(DateTime from, DateTime to, int pageSize, int pageIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Remove the entities backuped already
        /// </summary>
        /// <param name="entities">Entities to remove</param>
        public virtual void RemoveData(IList<T> entities)
        {
            throw new NotSupportedException();
        }
    }
}