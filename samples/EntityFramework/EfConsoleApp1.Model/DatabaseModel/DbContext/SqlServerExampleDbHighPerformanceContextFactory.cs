// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EfConsoleApp1.Model.DatabaseModel.DbContext;

/// <summary>
/// High performance factory for <see cref="SqlServerExampleDbContext"/> instances
/// </summary>
public class SqlServerExampleDbHighPerformanceContextFactory : IDbContextWithConfigFactory<SqlServerExampleDbContext>
{

    private DbContextOptionsBuilder<SqlServerExampleDbContext> _builder;

    /// <summary>
    /// Current DB context configuration
    /// </summary>

    /// <summary>
    /// Current app logger instance
    /// </summary>
    public IAppLoggerProxy Logger { get; }

    /// <summary>
    /// 
    /// </summary>
    public SqlServerExampleDbHighPerformanceContextFactory(IAppGlobalsWithDatabase appGlobals, IAppLoggerProxy logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        AppGlobals = appGlobals ?? throw new ArgumentNullException(nameof(appGlobals));
    }

    /// <summary>
    /// Current app globals with database settings
    /// </summary>
    public IAppGlobalsWithDatabase AppGlobals { get; }


    public SqlServerExampleDbContext CreateDbContext()
    {
        if (string.IsNullOrEmpty(AppGlobals.ContextConfig.ConnectionString))
        {
            throw new ArgumentNullException(nameof(AppGlobals.ContextConfig.ConnectionString));
        }

        // if options builder is not loaded already load it now
        if (_builder == null)
        {
            _builder = new DbContextOptionsBuilder<SqlServerExampleDbContext>();

            _builder
                .UseLoggerFactory(Logger.LoggerFactory)
                .UseSqlServer(AppGlobals.ContextConfig.ConnectionString, x => x
                    .MigrationsAssembly("EfConsoleApp1.Model")
                    .EnableRetryOnFailure()
                    .CommandTimeout((int)TimeSpan.FromSeconds(AppGlobals.ContextConfig.CommandTimeout).TotalSeconds));
        }

        // Create the DB context now
        var dbContext = new SqlServerExampleDbContext(_builder.Options);
        dbContext.Database.SetCommandTimeout(AppGlobals.ContextConfig.CommandTimeout);
        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

        return dbContext;
    }
}