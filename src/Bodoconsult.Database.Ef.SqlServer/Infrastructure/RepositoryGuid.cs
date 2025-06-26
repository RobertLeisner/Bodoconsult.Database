// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.SqlServer.Infrastructure
{
    /// <summary>
    /// Generic repository for all POCO types
    /// </summary>
    /// <typeparam name="TEntity">POCO type of the repository</typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class RepositoryGuid<TEntity, TContext> : BaseRepositoryGuid<TEntity, TContext> where TEntity : class, IEntityRequirementsGuid, new() where TContext : DbContext
    {

        /// <summary>
        /// default ctor
        /// </summary>
        /// <param name="contextLocator">Current database context locator</param>
        /// <param name="logger">Current logger</param>
        public RepositoryGuid(IAmbientDbContextLocator contextLocator, IAppLoggerProxy logger) : base(contextLocator, logger)
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

                using (var bulkCopy = new SqlBulkCopy(conn)
                {
                    DestinationTableName = GetTableName()
                })
                {
                    using (var table = new DataTable())
                    {
                        var properties = t.GetProperties()
                            .Where(p => p.PropertyType.IsValueType ||
                                        p.PropertyType == typeof(string)).ToList();

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
                        }

                        foreach (var entity in entities)
                        {
                            table.Rows.Add(properties.Select(property => property.GetValue(entity, null) ?? DBNull.Value).ToArray());
                        }

                        bulkCopy.BulkCopyTimeout = 0;
                        bulkCopy.WriteToServer(table);
                    }
                }
            }
        }
    }
}