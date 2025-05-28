// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Delegates;
using Bodoconsult.App.DependencyInjection;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.Database.Ef.Infrastructure;

namespace Bodoconsult.Database.Test.Utilities.App
{
    /// <summary>
    /// Current app globals for tests
    /// </summary>
    public class Globals: IAppGlobalsWithDatabase
    {

        #region Singleton factory

        // Thread-safe implementation of singleton pattern
        private static Lazy<Globals> _instance;

        /// <summary>
        /// Get a singleton instance of 
        /// </summary>
        /// <returns></returns>
        public static Globals Instance
        {
            get
            {
                try
                {
                    _instance ??= new Lazy<Globals>(() => new Globals());
                    return _instance.Value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
        }

        #endregion

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public EventWaitHandle EventWaitHandle { get; set; }
        public IAppStartParameter AppStartParameter { get; set; } = new AppStartParameter();
        public ILogDataFactory LogDataFactory { get; set; }
        public LoggingConfig LoggingConfig { get; set; } = new();
        public IAppLoggerProxy Logger { get; set; }
        public DiContainer DiContainer { get; set; }
        public HandleFatalExceptionDelegate HandleFatalExceptionDelegate { get; set; }
        public IAppStorageConnectionCheck AppStorageConnectionCheck { get; set; }
        public StatusMessageDelegate StatusMessageDelegate { get; set; }
        public LicenseMissingDelegate LicenseMissingDelegate { get; set; }
        public IContextConfig ContextConfig { get; set; } = new ContextConfig();
    }
}
