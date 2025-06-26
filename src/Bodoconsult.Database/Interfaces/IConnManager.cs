// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Bodoconsult.Database.Interfaces
{
    /// <summary>
    /// Get status messages from database stored procs
    /// </summary>
    /// <param name="message">Message received from database</param>
    public delegate void SqlStatus(string message);

    /// <summary>
    /// Handler method for notifiying a progress to user interface
    /// </summary>
    /// <param name="currentRowNumber">Current row number</param>
    public delegate void DatabaseNotifyProgressHandler(int currentRowNumber);


    /// <summary>
    /// Represents a database connection and important tasks for it
    /// </summary>
    public interface IConnManager
    {
        /// <summary>
        /// Get status messages from database stored procs
        /// </summary>
        SqlStatus SendStatus { get; set; }

        /// <summary>
        /// Notify a progress (row number) from a long lasting batch job
        /// </summary>
        DatabaseNotifyProgressHandler NotifyProgress { get; set; }


        /// <summary>
        /// Defines the step width a notifying progress handler is fired after
        /// </summary>
        int NotifyProgressSteps { get; set; }




        /// <summary>
        /// Run SQL statement directly against database
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <param name="async">Run async?</param>
        void Exec(string sql, bool async = false);

        /// <summary>
        /// Run SQL statement directly against database
        /// </summary>
        /// <param name="cmd">SQL statement to run</param>      
        void Exec(DbCommand cmd);

        /// <summary>
        /// Run a lot of commands on one connection in order of the list
        /// </summary>
        /// <param name="commands">List of commands</param>
        /// <returns>0 if there was no error, command's index if there was an error</returns>
        int ExecMultiple(IList<DbCommand> commands);

        /// <summary>
        /// Exec a SQL statement and return a scalar value as string
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>Scalar value as string</returns>
        string ExecWithResult(string sql);

        /// <summary>
        /// Exec a SQL command and return a scalar value as string
        /// </summary>
        /// <param name="cmd">SQL command to run</param>
        /// <returns>Scalar value as string</returns>
        string ExecWithResult(DbCommand cmd);

        /// <summary>
        /// Get a data table from an SQL command
        /// </summary>
        /// <param name="cmd">SQL command to run</param>
        /// <returns>A <see cref="DataTable"/> object with data</returns>
        DataTable GetDataTable(DbCommand cmd);

        /// <summary>
        /// Get a command object that implements <see cref="DbCommand"/> for the current database
        /// </summary>
        /// <returns></returns>
        DbCommand GetCommand();


        /// <summary>
        /// Get a parameter for the provided command
        /// </summary>
        /// <param name="cmd">Current command type</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="dataType">General database data typeof the parameter</param>
        /// <returns>Parameter object to set value for</returns>
        DbParameter GetParameter(DbCommand cmd, string parameterName, GeneralDbType dataType);



        /// <summary>
        /// Get a data table from an SQL command
        /// </summary>
        /// <param name="cmd">SQL command to run</param>
        /// <returns>Open <see cref="DataTable"/> object</returns>
        DbDataReader GetDataReader(DbCommand cmd);

        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>Open <see cref="DataTable"/> object</returns>
        DbDataReader GetDataReader(string sql);

        /// <summary>
        /// Get a <see cref="DataRow"/> object from a SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>A <see cref="DataRow"/> object with data</returns>
        object[] GetDataRow(string sql);

        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>A <see cref="DataTable"/> object with data</returns>
        DataTable GetDataTable(string sql);


        /// <summary>
        /// Get a <see cref="DataAdapter"/> from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>A <see cref="DataAdapter"/> object with data</returns>   
        DataAdapter GetDataAdapter(string sql);


        /// <summary>
        /// Get a <see cref="DataAdapter"/> from an <see cref="DbCommand"/>
        /// </summary>
        /// <param name="cmd">SQL statement</param>
        /// <returns>A <see cref="DataAdapter"/> object with data</returns>   
        DataAdapter GetDataAdapter(DbCommand cmd);

        /// <summary>
        /// Test the connection
        /// </summary>
        /// <returns>true on success else false</returns>
        bool TestConnection();

        /// <summary>
        /// Set the command timeout for the connection in seconds
        /// </summary>
        /// <param name="seconds">Timeout in seconds</param>
        void SetCommandTimeout(int seconds);

        /// <summary>
        /// Run command async
        /// </summary>
        /// <param name="cmd">Command to run</param>
        void ExecAsync(DbCommand cmd);


        /// <summary>
        /// Calls <see cref="NotifyProgress"/> (for testing purposes only)
        /// </summary>
        void TestNotifying();

        /// <summary>
        /// Bulk insert data to the database
        /// </summary>
        /// <param name="entities">Insert data in a table by bulkcopying it</param>
        /// <param name="tableName">Table name to insert the data in</param>
        void BulkInsertAll<TEntity>(IEnumerable<TEntity> entities, string tableName);


    }
}