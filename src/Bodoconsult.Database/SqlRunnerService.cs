// Copyright (c) Royotech. All rights reserved.


using System.Text;
using System.Threading;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database
{
    /// <summary>
    /// Current implementation of <see cref="ISqlRunnerService"/>
    /// </summary>
    public class SqlRunnerService : ISqlRunnerService
    {

        private ProducerConsumerQueue<string> _queue;

        private bool _isRunning;


        /// <summary>
        /// Current <see cref="IConnManager"/> instance
        /// </summary>
        public IConnManager CurrentConnManager { get; private set; }

        /// <summary>
        /// Load current <see cref="IConnManager"/> instance
        /// </summary>
        /// <param name="connManager">Current unit of work to use</param>
        public void LoadCurrentConnManager(IConnManager connManager)
        {
            CurrentConnManager = connManager;
        }

        /// <summary>
        /// Is the watchdog already started
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Add a SQL statement to the queue
        /// </summary>
        /// <param name="sql">Current SQL statement to add to the queue</param>
        public void AddSqlStatement(StringBuilder sql)
        {
            _queue.Enqueue(sql.ToString());

            sql.Clear();
        }

        /// <summary>
        /// Add a SQL statement to the queue
        /// </summary>
        /// <param name="sql">Current SQL statement to add to the queue</param>
        public void AddSqlStatement(string sql)
        {
            _queue.Enqueue(sql);
        }


        

        /// <summary>
        /// Start the internal SQL watchdog
        /// </summary>
        public void StartWatchDog()
        {
            if (IsStarted)
            {

                return;
            }

            IsStarted = true;
            _queue = new ProducerConsumerQueue<string>
            {
                ConsumerTaskDelegate = ConsumerTaskDelegate
            };
            _queue.StartConsumer();
        }

        private void ConsumerTaskDelegate(string sql)
        {
            _isRunning = true;
            CurrentConnManager.Exec(sql);
            _isRunning = false;
        }

        /// <summary>
        /// Stop the internal SQL watchdog
        /// </summary>
        public void StopWatchDog()
        {
            _queue.StopConsumer();
            IsStarted = false;
        }

        /// <summary>
        /// Wait until the SQL queue is empty
        /// </summary>
        public void Wait()
        {
            while (_queue.InternalQueue.Count > 0 || _isRunning)
            {
                Thread.Sleep(10);
            }
        }
    }
}
