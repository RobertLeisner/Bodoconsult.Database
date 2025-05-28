// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Migrations;

namespace Bodoconsult.Database.Ef.MigrationTools.Operations
{
    /// <summary>
    /// Provides extended migration operations for SqlServer
    /// </summary>
    public static class SqlServerExtensions
    {

        /// <summary>
        /// Create a database user
        /// </summary>
        /// <param name="migrationBuilder">current migrationbuilder</param>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>current migrationbuilder</returns>
        public static MigrationBuilder CreateUser(
            this MigrationBuilder migrationBuilder,
            string userName,
            string password)
        {

            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            var sql = "";

            switch (migrationBuilder.ActiveProvider)
            {
                case "Npgsql.EntityFrameworkCore.PostgreSQL":
                    sql = $"CREATE USER {userName} WITH PASSWORD '{password}';";
                    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    sql = $"CREATE USER {userName} WITH PASSWORD = '{password}';";
                    break;
                default:
                    throw new ArgumentException("Wrong provider");
            }

            if (!string.IsNullOrEmpty(sql))
            {
                migrationBuilder.Sql(sql);
            }


            return migrationBuilder;
        }


        /// <summary>
        /// Create a database role
        /// </summary>
        /// <param name="migrationBuilder">current migrationbuilder</param>
        /// <param name="roleName">Role name</param>
        /// <returns>current migrationbuilder</returns>
        public static MigrationBuilder CreateRole(
            this MigrationBuilder migrationBuilder,
            string roleName)
        {

            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            var sql = "";

            switch (migrationBuilder.ActiveProvider)
            {
                //case "Npgsql.EntityFrameworkCore.PostgreSQL":
                //    sql = $"CREATE USER {name} WITH PASSWORD '{password}';";
                //    break;
                case "Microsoft.EntityFrameworkCore.SqlServer":
                    sql = $"IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'{roleName}' AND type = 'R') CREATE ROLE [{roleName}] AUTHORIZATION [dbo]";
                    break;
                default:
                    throw new ArgumentException("Wrong provider");
            }

            if (!string.IsNullOrEmpty(sql))
            {
                migrationBuilder.Sql(sql);
            }


            return migrationBuilder;
        }

    }
}
