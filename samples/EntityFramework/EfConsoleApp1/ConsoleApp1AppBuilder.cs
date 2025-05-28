// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Interfaces;
using EfConsoleApp1.Model.DiContainerProvider;

namespace EfConsoleApp1;

public class ConsoleApp1AppBuilder: BaseAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public ConsoleApp1AppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    {

    }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var package = new SqlServerExampleDbDiContainerServiceProviderPackage(AppGlobals);
        DiContainerServiceProviderPackage = package;
    }
}