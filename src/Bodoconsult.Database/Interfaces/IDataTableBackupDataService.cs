// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using System.Data;

namespace Bodoconsult.Database.Interfaces;

/// <summary>
/// Data service for datatable based backup service providing, formatting and removing entities
/// </summary>
public interface IDataTableBackupDataService
{
    /// <summary>
    /// Get the data for an entity by date
    /// </summary>
    /// <param name="from">Date from inclusive</param>
    /// <param name="to">Date until exclusive</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageIndex">Current page index</param>
    /// <returns>List with entities</returns>
    DataTable GetData(DateTime from, DateTime to, int pageSize, int pageIndex);

    /// <summary>
    /// Remove the entities backuped already
    /// </summary>
    /// <param name="entities">Entities to remove</param>
    void RemoveData(DataTable entities);

}