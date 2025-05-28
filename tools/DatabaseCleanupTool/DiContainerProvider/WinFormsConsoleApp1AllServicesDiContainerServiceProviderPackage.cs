// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Delegates;
using Bodoconsult.App.DependencyInjection;
using Bodoconsult.App.Interfaces;

namespace DatabaseCleanupTool.DiContainerProvider
{
    /// <summary>
    /// Load all the complete package of DatabaseCleanupTool services based on GRPC to DI container. Intended mainly for production
    /// </summary>
    public class DatabaseCleanupToolAllServicesDiContainerServiceProviderPackage : BaseDiContainerServiceProviderPackage
    {

        public DatabaseCleanupToolAllServicesDiContainerServiceProviderPackage(IAppGlobals appGlobals,
            StatusMessageDelegate statusMessageDelegate, LicenseMissingDelegate licenseMissingDelegate) : base(appGlobals)
        {

            // Performance measurement
            IDiContainerServiceProvider  provider = new ApmDiContainerServiceProvider(appGlobals.AppStartParameter, statusMessageDelegate);
            ServiceProviders.Add(provider);

            // App default logging
            provider = new DefaultAppLoggerDiContainerServiceProvider(appGlobals.LoggingConfig, appGlobals.Logger);
            ServiceProviders.Add(provider);

            // SDatabaseCleanupTool specific services
            provider = new DatabaseCleanupToolAllServicesContainerServiceProvider(appGlobals.AppStartParameter, licenseMissingDelegate);
            ServiceProviders.Add(provider);
        }

    }
}
