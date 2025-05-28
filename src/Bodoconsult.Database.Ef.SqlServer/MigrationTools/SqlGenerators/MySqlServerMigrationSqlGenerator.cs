// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.Database.Ef.MigrationTools.Operations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace Bodoconsult.Database.Ef.SqlServer.MigrationTools.SqlGenerators
{
    /// <summary>
    /// Create an enhanced SQL generator based on MS default generator
    /// </summary>
    /// <remarks>Based on https://romiller.com/2013/02/27/ef6-writing-your-own-code-first-migration-operations/ </remarks>
    public sealed class MySqlServerMigrationSqlGenerator : SqlServerMigrationsSqlGenerator
    {

        public MySqlServerMigrationSqlGenerator(MigrationsSqlGeneratorDependencies dependencies,
                ICommandBatchPreparer migrationsAnnotations) : base(dependencies, migrationsAnnotations)
        {
            sqlHelper = Dependencies.SqlGenerationHelper;
        }


        private readonly ISqlGenerationHelper sqlHelper;

        /// <summary>
        /// Generate statements if there is not default statement generated for the operation
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="model"></param>
        /// <param name="builder"></param>
        protected override void Generate(
            MigrationOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {

            if (builder==null) {
                throw new ArgumentNullException(nameof(builder));}

            if (operation is GrantPermissionOperation gpo)
            {
                PerformGrantPermissionOperation(gpo, builder);
                return;
            }

            if (operation is DenyPermissionOperation dpo)
            {
                PerformDenyPermissionOperation(dpo, builder);
                return;
            }

            if (operation is AddDatabaseRoleOperation adro)
            {
                PerformAddDatabaseRoleOperation(adro, builder);
                return;
            }


            if (operation is DropDatabaseRoleOperation ddro)
            {
                PerformDropDatabaseRoleOperation(ddro, builder);
                return;
            }


            if (operation is ChangeIdentityOperation cio)
            {
                PerformChangeIdentityOperation(cio, builder);
                return;

            }

            if (operation is CreateCheckConstraintOperation cco)
            {
                PerformCreateCheckConstraint(cco, builder);
                return;
            }

            if (operation is CreateViewOperation cvo)
            {
                PerformCreateView(cvo, builder);
                return;
            }

            if (operation is AlterViewOperation avo)
            {
                PerformAlterView(avo, builder);
                return;
            }

            base.Generate(operation, model, builder);
        }

        private void PerformAlterView(AlterViewOperation operation, MigrationCommandListBuilder builder)
        {

            var comment = string.IsNullOrEmpty(operation.Comment) ? "" : $"\r\n/*\r\n{operation.Comment}\r\n*/\r\n";

            builder.Append($"ALTER VIEW {operation.ViewName}{comment} AS\r\n\r\n{operation.ViewBody}\r\n\r\n")
                .AppendLine(sqlHelper.StatementTerminator)
                .EndCommand();

        }

        private void PerformCreateView(CreateViewOperation operation, MigrationCommandListBuilder builder)
        {
            var comment = string.IsNullOrEmpty(operation.Comment) ? "" : $"\r\n/*\r\n{operation.Comment}\r\n*/\r\n";

            builder.Append($"CREATE VIEW {operation.ViewName}{comment} AS\r\n\r\n{operation.ViewBody}\r\n\r\n")
                .AppendLine(sqlHelper.StatementTerminator)
                .EndCommand();
        }

        /// <summary>
        /// Perform a <see cref="DropDatabaseRoleOperation"/> to drop a database role
        /// </summary>
        /// <param name="operation"></param>
        private void PerformDropDatabaseRoleOperation(DropDatabaseRoleOperation operation, MigrationCommandListBuilder builder)
        {

            var sql = "DECLARE @RoleName sysname \r\n" + $"set @RoleName = N'{operation.RoleName}' \r\n" +
                      "IF @RoleName<> N'public' and(select is_fixed_role from sys.database_principals where name = @RoleName) = 0 \r\n" +
                      "BEGIN\r\n" + "\tDECLARE @RoleMemberName sysname\r\n" + "\tDECLARE Member_Cursor CURSOR FOR\r\n" +
                      "\tselect[name]\r\n" + "\tfrom sys.database_principals\r\n" + "\twhere principal_id in (\r\n" +
                      "\tselect member_principal_id\r\n" + "\tfrom sys.database_role_members\r\n" +
                      "\twhere role_principal_id in (\r\n" + "\tselect principal_id\r\n" +
                      "\tFROM sys.database_principals where[name] = @RoleName AND type = 'R'))\r\n" +
                      "\tOPEN Member_Cursor;\r\n" + "\tFETCH NEXT FROM Member_Cursor\r\n" +
                      "\tinto @RoleMemberName\r\n" + "\tDECLARE @SQL NVARCHAR(4000) \r\n" +
                      "\tWHILE @@FETCH_STATUS = 0 \r\n" + "\tBEGIN \r\n" +
                      "\t\tSET @SQL = 'ALTER ROLE ' + QUOTENAME(@RoleName, '[') + ' DROP MEMBER ' + QUOTENAME(@RoleMemberName, '[')\r\n" +
                      "\t\tEXEC(@SQL) \r\n" + "\t\tFETCH NEXT FROM Member_Cursor \r\n" +
                      "\t\tinto @RoleMemberName \r\n" + "\tEND;\r\n" + "\tCLOSE Member_Cursor;\r\n" +
                      "\tDEALLOCATE Member_Cursor;\r\n" + "END\r\n";

            // Drop role members
            builder.Append(sql);

            builder.AppendLine("");

            // Drop the role itself
            sql = $"IF EXISTS (SELECT * FROM sys.database_principals WHERE name = N'{operation.RoleName}' AND type = 'R') DROP ROLE [{operation.RoleName}]";


            builder.Append(sql)
                .AppendLine(sqlHelper.StatementTerminator)
                .EndCommand();
        }



        /// <summary>
        /// Perform a <see cref="ChangeIdentityOperation"/> to reset seed value for Idenity columns
        /// </summary>
        /// <remarks>Based on https://romiller.com/2013/04/30/ef6-switching-identity-onoff-with-a-custom-migration-operation/</remarks>
        /// <param name="operation"></param>

        private void PerformChangeIdentityOperation(ChangeIdentityOperation operation, MigrationCommandListBuilder builder)
        {
            throw new NotImplementedException();

            ////var tempPrincipalColumnName = "old_" + operation.PrincipalColumn;

            ////// 1. Drop all foreign key constraints that point to the primary key we are changing
            ////foreach (var item in operation.DependentColumns)
            ////{
            ////    Generate(new DropForeignKeyOperation
            ////    {
            ////        DependentTable = item.DependentTable,
            ////        PrincipalTable = operation.PrincipalTable,
            ////        DependentColumns = { item.ForeignKeyColumn }
            ////    });
            ////}

            ////// 2. Drop the primary key constraint
            ////Generate(new DropPrimaryKeyOperation { Table = operation.PrincipalTable });

            ////// 3. Rename the existing column (so that we can re-create the foreign key relationships later)
            ////Generate(new RenameColumnOperation(
            ////    operation.PrincipalTable,
            ////    operation.PrincipalColumn,
            ////    tempPrincipalColumnName));

            ////// 4. Add the new primary key column with the new identity setting
            ////Generate(new AddColumnOperation(
            ////    operation.PrincipalTable,
            ////    new ColumnBuilder().Int(
            ////        name: operation.PrincipalColumn,
            ////        nullable: false,
            ////        identity: operation.Change == ChangeIdentityOperation.IdentityChange.SwitchIdentityOn)));

            ////// 5. Update existing data so that previous foreign key relationships remain
            ////if (operation.Change == ChangeIdentityOperation.IdentityChange.SwitchIdentityOn)
            ////{
            ////    // If the new column is an identity column we need to update all
            ////    // foreign key columns with the new values
            ////    foreach (var item in operation.DependentColumns)
            ////    {
            ////        Generate(new SqlOperation(
            ////            "UPDATE " + item.DependentTable +
            ////            " SET " + item.ForeignKeyColumn +
            ////                " = (SELECT TOP 1 " + operation.PrincipalColumn +
            ////                " FROM " + operation.PrincipalTable +
            ////                " WHERE " + tempPrincipalColumnName + " = " + item.DependentTable + "." + item.ForeignKeyColumn + ")"));
            ////    }
            ////}
            ////else
            ////{
            ////    // If the new column doesn’t have identity on then we can copy the old
            ////    // values from the previous identity column
            ////    Generate(new SqlOperation(
            ////        "UPDATE " + operation.PrincipalTable +
            ////        " SET " + operation.PrincipalColumn + " = " + tempPrincipalColumnName + ";"));
            ////}

            ////// 6. Drop old primary key column
            ////Generate(new DropColumnOperation(
            ////    operation.PrincipalTable,
            ////    tempPrincipalColumnName));

            ////// 7. Add primary key constraint
            ////Generate(new AddPrimaryKeyOperation
            ////{
            ////    Table = operation.PrincipalTable,
            ////    Columns = { operation.PrincipalColumn }
            ////});

            ////// 8. Add back foreign key constraints
            ////foreach (var item in operation.DependentColumns)
            ////{
            ////    Generate(new AddForeignKeyOperation
            ////    {
            ////        DependentTable = item.DependentTable,
            ////        DependentColumns = { item.ForeignKeyColumn },
            ////        PrincipalTable = operation.PrincipalTable,
            ////        PrincipalColumns = { operation.PrincipalColumn }
            ////    });
            ////}
        }

        private void PerformAddDatabaseRoleOperation(AddDatabaseRoleOperation operation, MigrationCommandListBuilder builder)
        {
            //var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

            builder.Append($"IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'{operation.RoleName}' AND type = 'R') CREATE ROLE [{operation.RoleName}] AUTHORIZATION [dbo]")
                .AppendLine(sqlHelper.StatementTerminator)
                .EndCommand();
        }

        private void PerformGrantPermissionOperation(GrantPermissionOperation operation, MigrationCommandListBuilder builder)
        {

            //var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

            builder.Append($"GRANT {operation.Permission.ToString().ToUpperInvariant()} ON { operation.DatabaseObject} TO {operation.UserOrRole}")
                .Append(sqlHelper.StatementTerminator)
                .EndCommand();

        }



        private void PerformDenyPermissionOperation(DenyPermissionOperation operation, MigrationCommandListBuilder builder)
        {

            //var stringMapping = Dependencies.TypeMappingSource.FindMapping(typeof(string));

            builder.Append($"DENY {operation.Permission.ToString().ToUpperInvariant()} ON {operation.DatabaseObject} TO {operation.UserOrRole}")
                .Append(sqlHelper.StatementTerminator)
                .EndCommand();

        }


        private void PerformCreateCheckConstraint(CreateCheckConstraintOperation operation, MigrationCommandListBuilder builder)
        {

            if (operation != null)
            {
                if (operation.CheckConstraintName == null)
                {
                    operation.CheckConstraintName = operation.BuildDefaultName();
                }

                var tableName = sqlHelper.DelimitIdentifier(operation.Table);
                var constraintName = sqlHelper.DelimitIdentifier(operation.CheckConstraintName);


                builder.Append($"ALTER TABLE {tableName} ADD CONSTRAINT {constraintName} CHECK ({operation.CheckConstraint})")
                    .Append(sqlHelper.StatementTerminator)
                    .EndCommand();


            }

        }
    }
}