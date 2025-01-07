// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bodoconsult.Database.EntityBackup;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.Test.EntityBackup;

/// <summary>
/// Example for a implementation of <see cref="IEntityBackupDataService{T}"/>  based on <see cref="DemoEntity"/>
/// </summary>
public class DemoEntityEntityBackupDataService : BaseEntityBackupDataService<DemoEntity>
{

    /// <summary>
    /// Demo entities for testing
    /// </summary>
    public List<DemoEntity> DemoEntities { get; } = new();

    /// <summary>
    /// Get the data for an entity by date
    /// </summary>
    /// <param name="from">Date from inclusive</param>
    /// <param name="to">Date until exclusive</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="pageIndex">Current page index</param>
    /// <returns>List with entities</returns>
    public override IList<DemoEntity> GetData(DateTime from, DateTime to, int pageSize, int pageIndex)
    {
        return DemoEntities.Where(x => x.Date >= from && x.Date < to).ToList();
    }

    /// <summary>
    /// Remove the entities backuped already
    /// </summary>
    /// <param name="entities">Entities to remove</param>
    public override void RemoveData(IList<DemoEntity> entities)
    {

        foreach (var entity in entities)
        {
            DemoEntities.Remove(entity);
        }

    }

}