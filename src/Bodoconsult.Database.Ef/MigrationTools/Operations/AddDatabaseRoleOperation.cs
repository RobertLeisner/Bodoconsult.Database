// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Add a database role
    /// </summary>
    public class AddDatabaseRoleOperation : MigrationOperation
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="roleName">Role name</param>
        public AddDatabaseRoleOperation(string roleName)
        {
            RoleName = roleName;
        }

        public string RoleName { get; }

        public override bool IsDestructiveChange => false;
    }
}