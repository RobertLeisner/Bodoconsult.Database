// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.EntityBackup
{
    /// <summary>
    /// Describes the manner how the data to backup are written in backup files. Daily means the data for one day is written in one file.
    /// </summary>
    public enum BackupTypeEnum
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}

