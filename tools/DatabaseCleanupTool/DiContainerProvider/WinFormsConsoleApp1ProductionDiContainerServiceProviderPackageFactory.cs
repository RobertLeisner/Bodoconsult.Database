// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Delegates;
using Bodoconsult.App.Interfaces;

namespace DatabaseCleanupTool.DiContainerProvider
{
    /// <summary>
    /// The current DI container used for production 
    /// </summary>
    public class DatabaseCleanupToolProductionDiContainerServiceProviderPackageFactory : IDiContainerServiceProviderPackageFactory
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public DatabaseCleanupToolProductionDiContainerServiceProviderPackageFactory(IAppGlobals appGlobals)
        {
            AppGlobals = appGlobals;
        }

        /// <summary>
        /// App globals
        /// </summary>
        public IAppGlobals AppGlobals { get; }

        /// <summary>
        /// Current status message delegate
        /// </summary>
        public StatusMessageDelegate StatusMessageDelegate { get; set; }

        /// <summary>
        /// Current license management delegate
        /// </summary>
        public LicenseMissingDelegate LicenseMissingDelegate { get; set; }

        /// <summary>
        /// Create an instance of <see cref="IDiContainerServiceProviderPackage"/>. Should be a singleton instance
        /// </summary>
        /// <returns>Singleton instance of <see cref="IDiContainerServiceProviderPackage"/></returns>
        public IDiContainerServiceProviderPackage CreateInstance()
        {
            
            return new DatabaseCleanupToolAllServicesDiContainerServiceProviderPackage(AppGlobals, StatusMessageDelegate, LicenseMissingDelegate);
        }
    }
}