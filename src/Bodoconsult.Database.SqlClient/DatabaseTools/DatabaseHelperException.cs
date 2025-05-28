// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;

namespace Bodoconsult.Database.SqlClient.DatabaseTools
{
    /// <summary>
    /// Database helper related exception
    /// </summary>
    public class DatabaseHelperException : Exception
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public DatabaseHelperException(): base("DatebaseHelper exception")
        {

        }

        /// <summary>
        /// Ctor with a message
        /// </summary>
        /// <param name="message">Exception message</param>
        public DatabaseHelperException(string message) : base($"DatebaseHelper exception: {message}")
        {

        }

        /// <summary>
        /// Ctor with a message and a inner exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public DatabaseHelperException(string message, Exception innerException) : base($"DatebaseHelper exception{message}: ", innerException)
        {

        }
    }
}