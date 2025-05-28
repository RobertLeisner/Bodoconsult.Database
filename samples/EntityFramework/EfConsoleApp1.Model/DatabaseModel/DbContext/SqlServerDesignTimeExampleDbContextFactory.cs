// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EfConsoleApp1.Model.DatabaseModel.DbContext;

public class SqlServerDesignTimeExampleDbContextFactory : IDesignTimeDbContextFactory<SqlServerExampleDbContext>
{
    /// <summary>Creates a new instance of a derived context.</summary>
    /// <param name="args">Arguments provided by the design-time service.</param>
    /// <returns>An instance of <typeparamref name="TContext" />.</returns>
    public SqlServerExampleDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();


        var loggerfactory = new FakeLoggerFactory();

        var conn = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(conn))
        {
            throw new ArgumentException($"{nameof(conn)} may not be null");
        }

        var builder = new DbContextOptionsBuilder<SqlServerExampleDbContext>();

        builder
            .UseLoggerFactory(loggerfactory)
            .UseSqlServer(conn, x => x
                .MigrationsAssembly(typeof(SqlServerExampleDbContext).Assembly.FullName)
                .EnableRetryOnFailure()
                .CommandTimeout((int)TimeSpan.FromSeconds(60).TotalSeconds));

        var dbContext = new SqlServerExampleDbContext(builder.Options);
        dbContext.Database.SetCommandTimeout(60);
        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;

        return dbContext;
    }
}