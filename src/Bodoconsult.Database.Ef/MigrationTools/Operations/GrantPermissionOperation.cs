// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Grant permissions on tables, views, stored procedures or functions
    /// </summary>
    public class GrantPermissionOperation : MigrationOperation
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="databaseObject">Table, view, stored procedure or function name including schema</param>
        /// <param name="userOrRole">User or role name</param>
        /// <param name="permission">Permission to set</param>
        public GrantPermissionOperation(string databaseObject, string userOrRole, DatabasePermission permission)
        {
            DatabaseObject = databaseObject;
            UserOrRole = userOrRole;
            Permission = permission;
        }

        public string DatabaseObject { get; }
        public string UserOrRole { get; }
        public DatabasePermission Permission { get; }

        public override bool IsDestructiveChange => false;
    }
}