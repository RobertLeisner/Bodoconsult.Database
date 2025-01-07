// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Bodoconsult.Database.Interfaces;
using Npgsql;
using NpgsqlTypes;

namespace Bodoconsult.Database.Postgres
{


    public class PostgresConnManager : AdapterConnManager
    {

        public PostgresConnManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public static IConnManager GetConnManager(string connectionString)
        {
            return new PostgresConnManager(connectionString);
        }

        /// <summary>
        /// Tests the connectionstring.
        /// </summary>
        /// <returns>True= connection could be established; False connection could not be established</returns>
        public override bool TestConnection()
        {
            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    conn.Open();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
            return true;
        }

        private int _commandTimeOut = -1;


        /// <summary>
        /// Get a command object that implements <see cref="DbCommand"/> for the current database
        /// </summary>
        /// <returns></returns>
        public override DbCommand GetCommand()
        {
            return new NpgsqlCommand();
        }


        /// <summary>
        /// Set the command timeout for the connection in seconds
        /// </summary>
        /// <param name="seconds">Timeout in seconds</param>
        public override void SetCommandTimeout(int seconds)
        {
            _commandTimeOut = seconds;
        }

        public override DataAdapter GetDataAdapter(string sql)
        {
            var conn = new NpgsqlConnection(ConnectionString);
            return new NpgsqlDataAdapter(sql, conn);
        }

        /// <summary>
        /// Get a <see cref="DataAdapter"/> from an <see cref="DbCommand"/>
        /// </summary>
        /// <param name="cmd">SQL statement</param>
        /// <returns>A <see cref="DataAdapter"/> object with data</returns>   
        public override DataAdapter GetDataAdapter(DbCommand cmd)
        {
            if (!(cmd is NpgsqlCommand))
            {
                throw new ArgumentException("Command must be of type NpgsqlCommand!");
            }

            var conn = new NpgsqlConnection(ConnectionString);
            var a = new NpgsqlDataAdapter();
            cmd.Connection = conn;
            a.SelectCommand = (NpgsqlCommand)cmd;

            return a;
        }

        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>A <see cref="DataTable"/> object with data</returns>
        public override DataTable GetDataTable(string sql)
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            var cmd = new NpgsqlCommand(sql, conn);
            if (_commandTimeOut != -1)
            {
                cmd.CommandTimeout = _commandTimeOut;
            }
            var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            //if (reader == null)
            //{
            //    return null;
            //}

            var dt = new DataTable();
            dt.Load(reader);

            return dt;
        }


        /// <summary>
        /// Get a data table from an SQL command
        /// </summary>
        /// <param name="cmd">SQL command to run</param>
        /// <returns>A <see cref="DataTable"/> object with data</returns>
        public override DataTable GetDataTable(DbCommand cmd)
        {

            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            cmd.Connection = conn;
            if (_commandTimeOut != -1)
            {
                cmd.CommandTimeout = _commandTimeOut;
            }
            var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            if (reader == null)
            {
                return null;
            }

            var dt = new DataTable();
            dt.Load(reader);

            return dt;
        }



        /// <summary>
        /// Get a <see cref="DataRow"/> object from a SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>A <see cref="DataRow"/> object with data</returns>
        public override object[] GetDataRow(string sql)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = new NpgsqlCommand(sql, conn);
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

        /// <summary>
        /// Run SQL statement directly against database
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <param name="async">Run async?</param>
        public override void Exec(string sql, bool async = false)
        {
            if (async)
            {
                throw new DbConnException("Async Access not possible with this provider. Use provider System.Data.SqlClient.", DbConnErrorCode.ERC_UNSUPPORTEDPROVIDER);
            }
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = new NpgsqlCommand(sql, conn);
                if (_commandTimeOut != -1)
                {
                    cmd.CommandTimeout = _commandTimeOut;
                }
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Exec a SQL statement and return a scalar value as string
        /// </summary>
        /// <param name="sql">SQL statement</param>
        /// <returns>Scalar value as string</returns>
        public override string ExecWithResult(string sql)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                var cmd = new NpgsqlCommand(sql, conn);
                if (_commandTimeOut != -1)
                {
                    cmd.CommandTimeout = _commandTimeOut;
                }
                cmd.Connection.Open();
                object value = cmd.ExecuteScalar();
                conn.Close();
                string val = (value != null) ? value.ToString() : "";
                return val;
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
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
                    //if (SendStatus != null) conn.InfoMessage += ConnOnInfoMessage;
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
        /// Run SQL statement directly against database
        /// </summary>
        /// <param name="cmd">SQL statement to run</param>      
        public override void Exec(DbCommand cmd)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                cmd.Connection = conn;
                if (_commandTimeOut != -1)
                {
                    cmd.CommandTimeout = _commandTimeOut;
                }
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Run a lot of commands on one connection in order of the list
        /// </summary>
        /// <param name="commands">List of commands</param>
        /// <returns>0 if there was no error, command's index if there was an error</returns>
        public override int ExecMultiple(IList<DbCommand> commands)
        {
            var index = 0;

            try
            {
                using (var conn = new NpgsqlConnection(ConnectionString))
                {
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
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                return 0;
            }
            catch
            {
                return index;
            }
        }


        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="sql">SQL statement to run</param>
        /// <returns>Open <see cref="DataTable"/> object</returns>
        public override DbDataReader GetDataReader(string sql)
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();
            var cmd = new NpgsqlCommand(sql, conn);
            if (_commandTimeOut != -1)
            {
                cmd.CommandTimeout = _commandTimeOut;
            }
            var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;
        }


        /// <summary>
        /// Get a data table from an SQL statement
        /// </summary>
        /// <param name="cmd">Current command to run</param>
        /// <returns>Open <see cref="DataTable"/> object</returns>
        public override DbDataReader GetDataReader(DbCommand cmd)
        {
            var conn = new NpgsqlConnection(ConnectionString);
            conn.Open();

            cmd.Connection = conn;

            if (_commandTimeOut != -1)
            {
                cmd.CommandTimeout = _commandTimeOut;
            }
            var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;
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
            var sqlCmd = (NpgsqlCommand)cmd;

            var sqlType = MapGeneralDbTypeToNpgsqlDbType(dataType);

            var p = new NpgsqlParameter(parameterName, sqlType);

            sqlCmd.Parameters.Add(p);

            return p;
        }


        /// <summary>
        /// Map the general database parameter data type to it's SQLClient relative
        /// </summary>
        /// <param name="dataType">Gernal database data type</param>
        /// <returns>Npgsql data type</returns>
        public static NpgsqlDbType MapGeneralDbTypeToNpgsqlDbType(GeneralDbType dataType)
        {

            switch (dataType)
            {
                case GeneralDbType.BigInt:
                    return NpgsqlDbType.Bigint;
                //case GeneralDbType.Binary:
                //    break;
                case GeneralDbType.Bit:
                    return NpgsqlDbType.Bit;
                case GeneralDbType.Char:
                    return NpgsqlDbType.Char;
                case GeneralDbType.DateTime:
                    return NpgsqlDbType.Date;
                case GeneralDbType.Decimal:
                    return NpgsqlDbType.Numeric;
                case GeneralDbType.Float:
                    return NpgsqlDbType.Double;
                //case GeneralDbType.Image:
                //    break;
                case GeneralDbType.Int:
                    return NpgsqlDbType.Integer;
                case GeneralDbType.Money:
                    return NpgsqlDbType.Money;
                case GeneralDbType.NChar:
                    return NpgsqlDbType.Char;
                case GeneralDbType.NText:
                    return NpgsqlDbType.Text;
                case GeneralDbType.NVarChar:
                    return NpgsqlDbType.Text;
                case GeneralDbType.Real:
                    return NpgsqlDbType.Double;
                case GeneralDbType.UniqueIdentifier:
                    return NpgsqlDbType.Uuid;
                case GeneralDbType.SmallDateTime:
                    return NpgsqlDbType.Date;
                case GeneralDbType.SmallInt:
                    return NpgsqlDbType.Smallint;
                case GeneralDbType.SmallMoney:
                    return NpgsqlDbType.Money;
                case GeneralDbType.Text:
                    return NpgsqlDbType.Text;
                case GeneralDbType.Timestamp:
                    return NpgsqlDbType.Timestamp;
                case GeneralDbType.TinyInt:
                    return NpgsqlDbType.Smallint;
                //case GeneralDbType.VarBinary:
                //    break;
                case GeneralDbType.VarChar:
                    return NpgsqlDbType.Varchar;
                //case GeneralDbType.Variant:
                //    break;
                case GeneralDbType.Xml:
                    return NpgsqlDbType.Text;
                //case GeneralDbType.Udt:
                //    break;
                //case GeneralDbType.Structured:
                //    break;
                case GeneralDbType.Date:
                    return NpgsqlDbType.Date;
                case GeneralDbType.Time:
                    return NpgsqlDbType.Date;
                case GeneralDbType.DateTime2:
                    return NpgsqlDbType.Date;
                case GeneralDbType.DateTimeOffset:
                    return NpgsqlDbType.TimeTz;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
            }
        }
    }
}