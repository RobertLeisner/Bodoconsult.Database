// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace EfConsoleApp1.Model.DatabaseModel.DbContext
{
    /// <summary>
    /// Implement SQLServer specific features for the DbContext
    /// </summary>
    public class SqlServerExampleDbContext : ExampleDbContext
    {
        /// <summary>
        /// Ctor with context options
        /// </summary>
        /// <param name="options">Current DB options</param>
        public SqlServerExampleDbContext(DbContextOptions options) : base(options)
        { }

        ///// <summary>
        ///// Get default options for the DbContext
        ///// </summary>
        ///// <returns>DbContext options</returns>
        //private static DbContextOptions GetDefaultOptions()
        //{
        //    var builder = new DbContextOptionsBuilder<ExampleDbContext>();

        //    builder.UseSqlServer(Globals.Instance.AppStartParameter.DefaultConnectionString, x => x
        //        .MigrationsAssembly("EfConsoleApp1.Model")
        //        .EnableRetryOnFailure()
        //        .CommandTimeout((int)TimeSpan.FromSeconds(600).TotalSeconds));
        //    return builder.Options;
        //}

    }
}
