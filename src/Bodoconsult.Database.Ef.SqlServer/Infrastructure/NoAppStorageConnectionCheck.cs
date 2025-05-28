// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.Database.Ef.SqlServer.Infrastructure
{
    /// <summary>
    /// Current impl of <see cref="IAppStorageConnectionCheck"/> to use with MS SqlServer, MS SqlServer Express or MS LocalDb depending on the connection string provided
    /// </summary>
    public class NoAppStorageConnectionCheck : IAppStorageConnectionCheck
    {
        private readonly string _connectionString;

        /// <summary>
        /// Default ctor
        /// </summary>
        public NoAppStorageConnectionCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Check if the connection to the app data storage is available. Returns true if the data storage is accessible else false
        /// </summary>
        public bool IsConnected => true;

        /// <summary>
        /// Helpful information for the user if the check fails i.e. conenction etc.
        /// </summary>
        public string HelpfulInformation => $"Current connection string: {_connectionString}";
    }
}