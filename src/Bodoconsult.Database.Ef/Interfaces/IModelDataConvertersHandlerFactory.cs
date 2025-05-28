// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.Database.Ef.Interfaces;

/// <summary>
/// Interface for factories for creating <see cref="IModelDataConvertersHandler"/> instances with all loaded converters for data migration
/// </summary>
public interface IModelDataConvertersHandlerFactory
{

    /// <summary>
    /// Create the instance of <see cref="IModelDataConvertersHandler"/> with all loaded converters for data migration
    /// </summary>
    /// <returns></returns>
    IModelDataConvertersHandler CreateInstance();
}