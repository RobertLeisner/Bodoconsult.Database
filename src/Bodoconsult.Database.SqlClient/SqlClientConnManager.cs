// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Bodoconsult.Database.Interfaces;
using Microsoft.Data.SqlClient;

namespace Bodoconsult.Database.SqlClient
{
    public class SqlClientConnManager : AdapterConnManager
    {
        private SqlConnection _connection;


        public static IConnManager GetConnManager(string connectionString)
        {
            return new SqlClientConnManager(connectionString);
        }

        public SqlClientConnManager(string connectionString)
        {
            
            ConnectionString = connectionString;

            if (!ConnectionString.ToLower().Contains("connect timeout"))
            {
                ConnectionString += "; Connect timeout=3000";
            }
        }


        /// <summary>
        /// Tests the connectionstring.
        /// </summary>
        /// <returns>True= connection could be established; False connection could not be established</returns>
        public override bool TestConnection()
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }
                    conn.Open();
                    conn.Close();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void ConnOnInfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            SendStatus(e.Message);
        }

        ///// <summary>
        ///// Get a DbCommand Object, that matches the Provider
        ///// </summary>
        ///// <returns>DBCommand matching the Provider</returns>
        //public override DbCommand GetDbCommand()
        //{
        //    var cmd = new SqlCommand();
        //    return cmd;
        //}

        private int _commandTimeOut = -1;


        /// <summary>
        /// Set the command timeout for the connection in seconds
        /// </summary>
        /// <param name="seconds">Timeout in seconds</param>
        public override void SetCommandTimeout(int seconds)
        {
            _commandTimeOut = seconds;
        }


        /// <summary>
        /// Get a <see cref="DataAdapter"/> from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>A <see cref="DataAdapter"/> object with data</returns>   
        public override DataAdapter GetDataAdapter(string sql)
        {
            var conn = new SqlConnection(ConnectionString);
            if (SendStatus != null)
            {
                conn.InfoMessage += ConnOnInfoMessage;
            }
            return new SqlDataAdapter(sql, conn);
        }


        /// <summary>
        /// Get a <see cref="DataAdapter"/> from an <see cref="DbCommand"/>
        /// </summary>
        /// <param name="cmd">SQL statement</param>
        /// <returns>A <see cref="DataAdapter"/> object with data</returns>   
        public override DataAdapter GetDataAdapter(DbCommand cmd)
        {
            if (cmd is not SqlCommand command)
            {
                throw new ArgumentException("Command must by of type OleDbCommand!");
            }

            var conn = new SqlConnection(ConnectionString);
            var a = new SqlDataAdapter();
            cmd.Connection = conn;
            a.SelectCommand = command;

            return a;
        }


        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>Open <see cref="DataTable"/> object</returns>
        public override DbDataReader GetDataReader(string sql)
        {
            try
            {
                var conn = new SqlConnection(ConnectionString);
                if (SendStatus != null)
                {
                    conn.InfoMessage += ConnOnInfoMessage;
                }
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                if (_commandTimeOut != -1)
                {
                    cmd.CommandTimeout = _commandTimeOut;
                }
                var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetDataReader:{ConnectionString}:Sql:{sql}", ex);
            }
        }


        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>A <see cref="DataTable"/> object with data</returns>
        public override DataTable GetDataTable(string sql)
        {
            try
            {
                var conn = new SqlConnection(ConnectionString);
                if (SendStatus != null)
                {
                    conn.InfoMessage += ConnOnInfoMessage;
                }
                conn.Open();
                var cmd = new SqlCommand(sql, conn);
                if (_commandTimeOut != -1)
                {
                    cmd.CommandTimeout = _commandTimeOut;
                }
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                var dt = new DataTable();
                dt.BeginLoadData();
                dt.Load(reader);
                dt.EndLoadData();
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"GetDataTable:{ConnectionString}:Sql:{sql}", ex);
            }
        }


        /// <summary>
        /// Get a <see cref="DataRow"/> object from a SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>A <see cref="DataRow"/> object with data</returns>
        public override object[] GetDataRow(string sql)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }
                    var cmd = new SqlCommand(sql, conn);
                    if (_commandTimeOut != -1)
                    {
                        cmd.CommandTimeout = _commandTimeOut;
                    }
                    cmd.Connection.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        var values = new object[reader.FieldCount];
                        // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                        reader.GetValues(values);
                        // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                        return values;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"GetDataRow:{ConnectionString}:Sql:{sql}", ex);
            }
        }


        /// <summary>
        /// Exec a SQL statement and return a scalar value as string
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>Scalar value as string</returns>
        public override string ExecWithResult(string sql)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }
                    var cmd = new SqlCommand(sql, conn);
                    if (_commandTimeOut != -1)
                    {
                        cmd.CommandTimeout = _commandTimeOut;
                    }
                    cmd.Connection.Open();
                    var value = cmd.ExecuteScalar();
                    conn.Close();
                    var val = (value != null) ? value.ToString() : "";
                    return val;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ExecWithResult:{ConnectionString}:Sql:{sql}", ex);
            }


        }


        /// <summary>
        /// Exec a SQL command and return a scalar value as string
        /// </summary>
        /// <param name="cmd">SQL command to run</param>
        /// <returns>Scalar value as string</returns>
        public override string ExecWithResult(DbCommand cmd)
        {

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }
                    cmd.Connection = conn;
                    if (_commandTimeOut != -1)
                    {
                        cmd.CommandTimeout = _commandTimeOut;
                    }
                    cmd.Connection.Open();
                    var value = cmd.ExecuteScalar();
                    conn.Close();
                    var val = (value != null) ? value.ToString() : "";
                    return val;
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"ExecWithResult:{ConnectionString}:Sql:{cmd.CommandText}", ex);
            }
        }


        /// <summary>
        /// Get a data table from an SQL command
        /// </summary>
        /// <param name="cmd">SQL command to run</param>
        /// <returns>A <see cref="DataTable"/> object with data</returns>
        public override DataTable GetDataTable(DbCommand cmd)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }
                    cmd.Connection = conn;
                    if (_commandTimeOut != -1)
                    {
                        cmd.CommandTimeout = _commandTimeOut;
                    }
             
                    cmd.Connection.Open();

                    var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    var dt = new DataTable();
                    dt.BeginLoadData();
                    dt.Load(reader);
                    dt.EndLoadData();
                    return dt;
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"GetDataTable:{ConnectionString}:Sql:{cmd.CommandText}", ex);
            }


        }

        /// <summary>
        /// Get a command object that implements <see cref="DbCommand"/> for the current database
        /// </summary>
        /// <returns></returns>
        public override DbCommand GetCommand()
        {
            return new SqlCommand();
        }


        /// <summary>
        /// Returns a DataReader for a DbCommand
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public override DbDataReader GetDataReader(DbCommand cmd)
        {
            try
            {

                var p = new SqlParameter("Test", SqlDbType.BigInt);

                var conn = new SqlConnection(ConnectionString);
                if (SendStatus != null)
                {
                    conn.InfoMessage += ConnOnInfoMessage;
                }
                cmd.Connection = conn;
                if (_commandTimeOut != -1)
                {
                    cmd.CommandTimeout = _commandTimeOut;
                }
                cmd.Connection.Open();
                var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return dr;
            }
            catch (Exception ex)
            {

                throw new Exception($"GetDataReader:{ConnectionString}:Sql:{cmd.CommandText}", ex);
            }
        }


        /// <summary>
        /// Run SQL statement directly against database
        /// </summary>
        /// <param name="cmd">SQL statement to run</param>      
        public override void Exec(DbCommand cmd)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }
                    cmd.Connection = conn;
                    if (_commandTimeOut != -1)
                    {
                        cmd.CommandTimeout = _commandTimeOut;
                    }
                    cmd.Connection.Open();
                    //cmd.ExecuteNonQuery();
                    cmd.ExecuteScalar();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Exec:{ConnectionString}:Sql:{cmd.CommandText}", ex);
            }

        }


        /// <summary>
        /// Run command async
        /// </summary>
        /// <param name="cmd">Command to run</param>
        public override void ExecAsync(DbCommand cmd)
        {
            try
            {
                var cs = $"{ConnectionString};Asynchronous Processing=true;";
                using (_connection = new SqlConnection(cs))
                {
                    if (SendStatus != null)
                    {
                        _connection.InfoMessage += ConnOnInfoMessage;
                    }
                    cmd.Connection = _connection;
                    if (_commandTimeOut != -1)
                    {
                        cmd.CommandTimeout = _commandTimeOut;
                    }
                    cmd.Connection.Open();

                    //var callback = new AsyncCallback(HandleCallback);
                    //((SqlCommand) cmd).BeginExecuteNonQuery(callback, cmd);


                    var sqlcmd = ((SqlCommand)cmd);

                    sqlcmd.ExecuteScalar();

                    //var result = sqlcmd.BeginExecuteNonQuery();
                    //while (!result.IsCompleted)
                    //{
                    //}

                    //sqlcmd.EndExecuteNonQuery(result);


                    //((SqlCommand)cmd).BeginExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ExecAsync:{ConnectionString}:Sql:{cmd.CommandText}", ex);
            }
        }

        public override void BulkInsertAll<TEntity>(IEnumerable<TEntity> entities, string tableName)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var t = typeof(TEntity);

                using (var bulkCopy = new SqlBulkCopy(conn)
                       {
                           DestinationTableName = tableName
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


        /// <summary>
        /// Run SQL statement directly against database
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <param name="async">Run async?</param>
        public override void Exec(string sql, bool async=false)
        {
            try
            {
                if (async)
                {
                    var cs = $"{ConnectionString};Asynchronous Processing=true;";
                    using (_connection = new SqlConnection(cs))
                    {
                        if (SendStatus != null)
                        {
                            _connection.InfoMessage += ConnOnInfoMessage;
                        }
                        var cmd = new SqlCommand(sql, _connection);
                        if (_commandTimeOut != -1)
                        {
                            cmd.CommandTimeout = _commandTimeOut;
                        }
                        cmd.Connection.Open();

                        var callback = new AsyncCallback(HandleCallback);
                        cmd.BeginExecuteNonQuery(callback, cmd);

                    }

                }
                else
                {
                    using (var conn = new SqlConnection(ConnectionString))
                    {
                        if (SendStatus != null)
                        {
                            conn.InfoMessage += ConnOnInfoMessage;
                        }
                        var cmd = new SqlCommand(sql, conn);
                        if (_commandTimeOut != -1)
                        {
                            cmd.CommandTimeout = _commandTimeOut;
                        }
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Exec:{ConnectionString}:Sql:{sql}", ex);
            }


        }

        /// <summary>
        /// Run a lot of commands on one connection
        /// </summary>
        /// <param name="commands">List of commands</param>
        /// <returns>0 if there was no error, command's index if there was an error</returns>
        public override int ExecMultiple(IList<DbCommand> commands)
        {

            var index = 0;

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    if (SendStatus != null)
                    {
                        conn.InfoMessage += ConnOnInfoMessage;
                    }

                    conn.Open();

                    for (index = 0; index < commands.Count; index++)
                    {

                        if (index % NotifyProgressSteps == 0)
                        {
                            NotifyProgress?.Invoke(index);
                        }

                        var cmd = commands[index];
                        cmd.Connection = conn;
                        if (_commandTimeOut != -1)
                        {
                            cmd.CommandTimeout = _commandTimeOut;
                        }
                        //cmd.Connection.Open();
                        //cmd.ExecuteNonQuery();
                        cmd.ExecuteScalar();
                    }


                    conn.Close();

                    return 0;
                }
            }
            catch //(Exception e)
            {
                return index;
            }


        }



        private void HandleCallback(IAsyncResult result)
        {
            try
            {
                var command = (SqlCommand)result.AsyncState;
                command.EndExecuteNonQuery(result);
                if (_connection == null)
                {
                    return;
                }
                try
                {
                    _connection.Close();
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {
                    //nop
                }
            }
            catch (Exception ex)
            {

                throw new Exception("HandleCallback", ex);
            }


        }


        /// <summary>
        /// Get a parameter for the provided command
        /// </summary>
        /// <param name="cmd">Current command type</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="dataType">General database data typeof the parameter</param>
        /// <returns>Parameter object to set value for</returns>
        public override DbParameter GetParameter(DbCommand cmd, string parameterName, GeneralDbType dataType)
        {
            var sqlCmd = (SqlCommand) cmd;

            var sqlType = MapGeneralDbTypeToSqlDbType(dataType);

            var p = new SqlParameter(parameterName, sqlType);

            sqlCmd.Parameters.Add(p);

            return p;
        }


        /// <summary>
        /// Map the general database parameter data type to it's SQLClient relative
        /// </summary>
        /// <param name="dataType">Gernal database data type</param>
        /// <returns>SQql data type</returns>
        public static SqlDbType MapGeneralDbTypeToSqlDbType(GeneralDbType dataType)
        {

            var t = dataType.ToString().Replace("GeneralDbType", "SqlDbType");

            var sucess = Enum.TryParse(t, out SqlDbType sqlDataType);

            if (sucess)
            {
                return sqlDataType;
            }

            throw new ArgumentException($"Type not implemented: {dataType}");
        }
    }
}