// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.App.WinForms.App;
using DatabaseCleanupTool.App;
using DatabaseCleanupTool.DiContainerProvider;

namespace DatabaseCleanupTool;

public class DatabaseCleanupToolAppBuilder : BaseWinFormsAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public DatabaseCleanupToolAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    {

    }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var factory = new DatabaseCleanupToolProductionDiContainerServiceProviderPackageFactory(Globals.Instance);
        DiContainerServiceProviderPackage = factory.CreateInstance();
    }
}