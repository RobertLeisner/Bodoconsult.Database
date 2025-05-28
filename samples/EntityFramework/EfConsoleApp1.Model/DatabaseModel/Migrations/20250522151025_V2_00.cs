// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EfConsoleApp1.Model.DatabaseModel.Migrations
{
    public partial class V1_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            if (migrationBuilder == null)
            {
                throw new ArgumentNullException(nameof(migrationBuilder));
            }

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AppSettings",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Users",
                rowVersion: true,
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "UserTypeId",
                table: "Users",
                nullable: false,
                defaultValueSql: "0");

            // Create the table UserType now
            migrationBuilder.CreateTable(
                name: "UserType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", maxLength: 30, rowVersion: true, nullable: false),
                    UserTypeName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserType", x => x.ID);
                });

        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {

            // Restore doesn't make sense for StSysServer app

        }
    }
}
