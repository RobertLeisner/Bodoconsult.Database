// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.App.DependencyInjection;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Infrastructure;
using EfConsoleApp1.Model.DatabaseModel.DbContext;
using EfConsoleApp1.Model.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EfConsoleApp1.Model.DiContainerProvider;

/// <summary>
/// Provider adding unit of work services for SqlServer
/// </summary>
public class SqlServerExampleDbDiContainerProvider: IDiContainerServiceProvider
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public SqlServerExampleDbDiContainerProvider(IContextConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Current configuration
    /// </summary>
    public IContextConfig Config { get; }

    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        diContainer.AddSingletonInstance(Config);
        diContainer.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();
        diContainer.AddSingleton<IDbContextFactory<SqlServerExampleDbContext>, SqlServerExampleDbContextFactory>();
        diContainer.AddSingleton<IContextScopeFactory<SqlServerExampleDbContext>, DbContextScopeFactory<SqlServerExampleDbContext>>();
        diContainer.AddSingleton<IBackupEngine, SqlServerBackupEngine<SqlServerExampleDbContext>>();
        diContainer.AddSingleton<IUnitOfWork, SqlServerExampleDbUnitOfWork> ();
    }

    /// <summary>
    /// Late bind DI container references to avoid circular DI references
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void LateBindObjects(DiContainer diContainer)
    {
        //Do nothing
    }
}