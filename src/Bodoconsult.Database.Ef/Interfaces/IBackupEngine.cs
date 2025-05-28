// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.Interfaces
{
    /// <summary>
    /// Interface for backup engines for saving a database online
    /// </summary>
    public interface IBackupEngine
    {

        /// <summary>
        /// Run a online backup from the database
        /// </summary>
        /// <param name="backupFileName">backup file name</param>
        void RunBackup(string backupFileName);


        /// <summary>
        /// Restore a database from a backup file
        /// </summary>
        /// <param name="backupFileName">Full local file path for the backup</param>
        bool RestoreDatabase(string backupFileName);

    }
}
