// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfConsoleApp1.Model.DatabaseModel.Migrations
{
    public partial class V1_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            //// If necessary run a SQL script here to bring existing database to a defined state
            //var sql = ResourceHelper.GetSqlResource("MigrationV100_RunBefore");
            //migrationBuilder.Sql(sql);

            // Create the table
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 255, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.ID);
                });

            // Create the table Users now
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(maxLength: 30, nullable: false),
                    Realname = table.Column<string>(maxLength: 40, nullable: true),
                    Password = table.Column<string>(maxLength: 15, nullable: true),
                    Rights = table.Column<string>(maxLength: 10, nullable: true),
                    UserType = table.Column<string>(maxLength:30, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });



            // ***************************
            // The following section must be the end of the migration!
            // ***************************

            #region Give permissions to a database role (may be needed in more sophisticated security scenarios)

            // Add XXX role manually.
            const string roleName = "YourRoleName";

            var sql = "DECLARE @RoleName sysname \r\n" +
                      "set @RoleName = N'DummyRole' \r\n" +
                      "IF(select count(*) from sys.database_principals where name = @RoleName) = 0 \r\n" +
                      "BEGIN \r\n" +
                      "   CREATE ROLE ["+roleName+"] \r\n" +
                      "END\r\n";

            migrationBuilder.Sql(sql);

            // Give permissions on the tables to StSysUser role
            sql = "GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[__EFMigrationsHistory] To ["+roleName+"];";
            migrationBuilder.Sql(sql);

            sql = "GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[AppSettings] To ["+roleName+"]";
            migrationBuilder.Sql(sql);

            sql = "GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[Users] To [" + roleName + "]";
            migrationBuilder.Sql(sql);

            #endregion

            // ***************************
            // The section before must be the end of the migration!
            // Do not add code behind this point
            // ***************************
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore doesn't make sense for StSysServer app
        }
    }
}
