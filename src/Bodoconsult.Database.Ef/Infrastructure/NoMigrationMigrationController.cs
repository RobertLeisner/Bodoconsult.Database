// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.Infrastructure;

/// <summary>
/// An <see cref="IMigrationController"/> implementation doing nothing
/// </summary>
public class NoMigrationMigrationController : IMigrationController
{
    /// <summary>
    /// Current unit of work
    /// </summary>
    public IUnitOfWork UnitOfWork { get; private set; }

    /// <summary>
    /// Current database model data converter handler
    /// </summary>
    public IModelDataConvertersHandler ModelDataConvertersHandler { get; set; }

    /// <summary>
    /// Run a string containing SQL statements separated with \r\nGO\r\n before migrating an existing database
    /// </summary>
    public string MigrationRunBeforeExistingDb { get; set; }

    /// <summary>
    /// Is Database a new database? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
    /// </summary>
    public bool IsNewDatabase { get; set; }

    /// <summary>
    /// Are migrations pending? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
    /// </summary>
    public bool MigrationsPending { get; set; }

    public void LoadUnitOfWork(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// Take a backup of the database before migrations
    /// </summary>
    public void SaveDatabase()
    {
        // Do nothing
    }

    /// <summary>
    /// Take a backup of the database before migrations
    /// </summary>
    /// <param name="fileName">Filename to save the backup to</param>
    public void SaveDatabase(string fileName)
    {
        // Do nothing
    }

    /// <summary>
    /// Migrate database
    /// </summary>
    public void MigrateDatabase()
    {
        // Do nothing
    }

    public void ApplyDatabaseUpdates()
    {
        // Do nothing
    }
}