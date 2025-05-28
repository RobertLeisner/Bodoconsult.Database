// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.MigrationTools;

/// <summary>
/// Do NOT seed the database or migrate any existing data
/// </summary>
public class DoNothingDataModelConvertersHandlerFactory : IModelDataConvertersHandlerFactory
{
    private readonly IAppLoggerProxy _logger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public DoNothingDataModelConvertersHandlerFactory(IAppLoggerProxy logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create the instance of <see cref="IModelDataConvertersHandler"/> with all loaded converters for data migration
    /// </summary>
    /// <returns></returns>
    public IModelDataConvertersHandler CreateInstance()
    {

        var h = new ModelDataConvertersHandler(_logger);
        // Do NOT load converters here
        return h;

    }
}