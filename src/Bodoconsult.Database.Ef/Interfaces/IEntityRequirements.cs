// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.Interfaces;

/// <summary>
/// Defines basic requirements for POCO entity properties with an integer ID column as primary key field 
/// </summary>
/// <remarks>May be used to force creation of DateCreated, DateChanged, UserCreated, userChanged properties for an entity.
/// If all your POCO entities implement this interface or a derivative of it, it may be used to collect repositories automatically via reflection.</remarks>
public interface IEntityRequirements
{
    /// <summary>
    /// Primary key column with unique values
    /// </summary>
    int ID { get; set; }


    /// <summary>
    /// Row version to solve concurrency issues
    /// </summary>
    byte[] RowVersion { get; set; }

}