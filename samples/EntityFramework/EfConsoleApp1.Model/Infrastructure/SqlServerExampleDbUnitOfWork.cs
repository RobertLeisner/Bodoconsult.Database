// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.SqlServer.Infrastructure;
using EfConsoleApp1.Model.DatabaseModel.DbContext;

namespace EfConsoleApp1.Model.Infrastructure
{
    /// <summary>
    /// Current unit of work for ExampleDb running on SqlServer
    /// </summary>
    public class SqlServerExampleDbUnitOfWork: SqlServerUnitOfWork<SqlServerExampleDbContext>
    {
        public SqlServerExampleDbUnitOfWork(IContextScopeFactory<SqlServerExampleDbContext> dbContextScopeFactory, IAppLoggerProxy logger, IAmbientDbContextLocator ambientDbContextLocator, IBackupEngine backupEngine, IMigrationController migrationController) : base(dbContextScopeFactory, logger, ambientDbContextLocator, backupEngine, migrationController)
        {
        }

        // Add additional functionality for your unit of work here if needed
    }
}
