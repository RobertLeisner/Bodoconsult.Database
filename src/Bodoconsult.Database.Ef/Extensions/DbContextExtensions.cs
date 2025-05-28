// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Bodoconsult.Database.Ef.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="DbContext"/> instances
    /// </summary>
    public static class DbContextExtensions
    {

        /// <summary>
        /// Commit changes to database
        /// </summary>
        /// <param name="context">Current context</param>
        public static void Commit(this DbContext context)
        {
            context.SaveChanges();
        }

        /// <summary>
        /// Run SQL statement against database and return a result of type T
        /// </summary>
        /// <typeparam name="T">Return data type</typeparam>
        /// <param name="context">Current context</param>
        /// <param name="sql">SQL statement</param>
        /// <returns>Return data type T</returns>
        public static T ExecuteSqlWithResult<T>(this DbContext  context, string sql)
        {

            T result;
            DbConnection conn;
            ConnectionState initialState;

            try
            {
                conn = context.Database.GetDbConnection(); 
                initialState = conn.State;
                if (initialState != ConnectionState.Open)
                {
                    conn.Open(); // open connection if not already open
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(sql, e);
            }

            try
            {

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    result = (T)cmd.ExecuteScalar();
                }

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error running SQL: {sql}", ex);
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Close(); // only close connection if not initially open
                }
            }

            return result;
        }


        /// <summary>
        /// Run a SQL statement
        /// </summary>
        /// <param name="context">Current context</param>
        /// <param name="sql"></param>
        public static void ExecuteSql(this DbContext context, string sql)
        {

            if (string.IsNullOrEmpty(sql))
            {
                return;
            }

            //The Backup TSQL must run on a non-transactional connection. There context.Database.ExecuteSqlCommand(sql) maxy not work.
            // Use the underlying ADO.NET connection of the DbContext
            var conn = context.Database.GetDbConnection();  
            var initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Open(); // open connection if not already open
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error running SQL: {sql}", ex);
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                {
                    conn.Close(); // only close connection if not initially open
                }
            }
        }
    }
}