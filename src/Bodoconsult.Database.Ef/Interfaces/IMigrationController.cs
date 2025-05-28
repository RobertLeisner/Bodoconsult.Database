// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.Interfaces
{
    public interface IMigrationController
    {
        /// <summary>
        /// Current unit of work
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Current database model data converter handler
        /// </summary>

        IModelDataConvertersHandler ModelDataConvertersHandler { get; }

        /// <summary>
        /// Run a string containing SQL statements separated with \r\nGO\r\n before migrating an existing database
        /// </summary>
        string MigrationRunBeforeExistingDb { get; set; }


        /// <summary>
        /// Is Database a new database? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
        /// </summary>
        bool IsNewDatabase { get; set; }


        /// <summary>
        /// Are migrations pending? True if yes. Must be set before calling methods of <see cref="IMigrationController"/>
        /// </summary>
        bool MigrationsPending { get; set; }

        /// <summary>
        /// Load the current <see cref="IUnitOfWork"/> instance
        /// </summary>
        /// <param name="unitOfWork">Current <see cref="IUnitOfWork"/> instance</param>
        void LoadUnitOfWork(IUnitOfWork unitOfWork);

        /// <summary>
        /// Take a backup of the database before migrations
        /// </summary>
        void SaveDatabase();

        /// <summary>
        /// Take a backup of the database before migrations
        /// </summary>
        /// <param name="fileName">Filename to save the backup to</param>
        void SaveDatabase(string fileName);

        /// <summary>
        /// Migrate database
        /// </summary>
        void MigrateDatabase();

        /// <summary>
        /// Update database app content to current version (if necessary). Use only for app content not user content!
        /// </summary>
        void ApplyDatabaseUpdates();
    }
}