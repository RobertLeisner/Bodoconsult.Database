﻿// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

public class SqlServerExampleDbSqlModelDataConvertersHandlerFactory : IModelDataConvertersHandlerFactory
{
    private readonly IAppLoggerProxy _logger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SqlServerExampleDbSqlModelDataConvertersHandlerFactory(IAppLoggerProxy logger)
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

        // Load converters now in the order you need!
        h.AddConverter<AppSettingsConverterSql>();
        h.AddConverter<UserTypeConverterSql>(); // must be done before user migration to hace the UserType.ID available
        h.AddConverter<UsersConverterSql>();

        return h;

    }
}