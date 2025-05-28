// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Permissions to set on tables, views, stroed procs or functions
    /// </summary>
    public enum DatabasePermission
    {
        Select,
        Update,
        Delete,
        Insert,
        Execute
    }
}