// Copyright (c) Royotech. All rights reserved.

using System.Text;

namespace Bodoconsult.Database.Interfaces
{
    /// <summary>
    /// Interface for a service running SQL statements against the database
    /// </summary>
    public interface ISqlRunnerService
    {
        /// <summary>
        /// Current <see cref="IConnManager"/> instance
        /// </summary>
        IConnManager CurrentConnManager { get; }

        /// <summary>
        /// Load current <see cref="IConnManager"/> instance
        /// </summary>
        /// <param name="connManager">Current unit of work to use</param>
        void LoadCurrentConnManager(IConnManager connManager);

        /// <summary>
        /// Is the watchdog already started
        /// </summary>
        bool IsStarted { get; }


        /// <summary>
        /// Add a SQL statement to the queue
        /// </summary>
        /// <param name="sql">Current SQL statement to add to the queue</param>
        void AddSqlStatement(StringBuilder sql);

        /// <summary>
        /// Add a SQL statement to the queue
        /// </summary>
        /// <param name="sql">Current SQL statement to add to the queue</param>
        void AddSqlStatement(string sql);

        /// <summary>
        /// Start the internal SQL watchdog
        /// </summary>
        void StartWatchDog();

        /// <summary>
        /// Stop the internal SQL watchdog
        /// </summary>
        void StopWatchDog();

        /// <summary>
        /// Wait until the SQL queue is empty
        /// </summary>
        void Wait();
    }
}