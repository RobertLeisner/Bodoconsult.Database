// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Drop a database role
    /// </summary>
    public class DropDatabaseRoleOperation : MigrationOperation
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="roleName">Role name</param>
        public DropDatabaseRoleOperation(string roleName)
        {
            RoleName = roleName;
        }

        public string RoleName { get; }

        public override bool IsDestructiveChange => false;
    }
}