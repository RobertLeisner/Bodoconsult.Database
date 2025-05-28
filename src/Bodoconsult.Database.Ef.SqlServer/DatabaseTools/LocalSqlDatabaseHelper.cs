// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.SqlServer.DatabaseTools
{
    /// <summary>
    /// Tools for handling local databases (LocalDb, SqlServer Express and SqlServer)
    /// </summary>
    public class LocalSqlDatabaseHelper : BaseSqlDatabaseHelper
    {
        /// <summary>
        /// Default ctor: loads StSysEntities connection string
        /// </summary>
        public LocalSqlDatabaseHelper()
        {
            
        }

        /// <summary>
        /// Ctor with customized connection string
        /// </summary>
        /// <param name="nameOrConnectionString">connection string or name of a connection string</param>
        public LocalSqlDatabaseHelper(string nameOrConnectionString) : base(nameOrConnectionString)
        {

        }

    }
}