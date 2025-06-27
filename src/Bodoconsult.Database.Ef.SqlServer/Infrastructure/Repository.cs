// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.SqlServer.Infrastructure
{

#pragma warning disable CA1724

    /// <summary>
    /// Generic repository for all POCO types
    /// </summary>
    /// <typeparam name="TEntity">POCO type of the repository</typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class Repository<TEntity, TContext> : BaseRepository<TEntity, TContext> where TEntity : class, IEntityRequirements, new() where TContext : DbContext
    {
        /// <summary>
        /// Fields not allowed to copy by BulkCopy operations
        /// </summary>
        public static List<string> FieldsNotAllowedForBulkCopy => ["rowversion", "id"];

        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="contextLocator">Current database context locator</param>
        /// <param name="logger">Current logger</param>
        public Repository(IAmbientDbContextLocator contextLocator, IAppLoggerProxy logger) : base(contextLocator, logger)
        { }

        
        /// <summary>
        /// Bulk insert data to the database
        /// </summary>
        /// <param name="entities"></param>
        public override void BulkInsertAll(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var db = _contextLocator.GetContext<TContext>();
            using (var conn = new SqlConnection(db.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();

                var t = typeof(TEntity);

                var options = SqlBulkCopyOptions.Default;

                using (var bulkCopy = new SqlBulkCopy(conn, options, null)
                {
                    DestinationTableName = GetTableName()
                })
                {
                    using (var table = new DataTable())
                    {
                        var properties = t.GetProperties()
                                                   .Where(p => !FieldsNotAllowedForBulkCopy.Contains(p.Name.ToLowerInvariant()) &&
                                                               (p.PropertyType.IsValueType || p.PropertyType == typeof(string)))
                                                   .ToList();

                        foreach (var property in properties)
                        {
                            var propertyType = property.PropertyType;
                            if (propertyType.IsGenericType &&
                                propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                propertyType = Nullable.GetUnderlyingType(propertyType);
                            }

                            if (propertyType == null)
                            {
                                continue;
                            }

                            table.Columns.Add(new DataColumn(property.Name, propertyType));

                            var mapName = new SqlBulkCopyColumnMapping(property.Name, property.Name);
                            bulkCopy.ColumnMappings.Add(mapName);
                        }

                        foreach (var entity in entities)
                        {
                            table.Rows.Add(properties.Select(property => property.GetValue(entity, null) ?? DBNull.Value).ToArray());
                        }

                        table.AcceptChanges();

                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.WriteToServer(table);
                    }
                }
            }
        }
    }

#pragma warning restore CA1724
}