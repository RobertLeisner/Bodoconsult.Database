// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Extensions;
using Bodoconsult.Database.Ef.Interfaces;

namespace Bodoconsult.Database.Ef.MigrationTools;

public class BaseModelDataConverter : IModelDataConverter
{
    protected IUnitOfWork UnitOfWork;

    public readonly IAppLoggerProxy AppLogger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public BaseModelDataConverter(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger)
    {
        AppLogger = appLogger;
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// The required app version for running the converter
    /// </summary>
    public Version RequiredAppVersion { get; protected set; }

    /// <summary>
    /// Run the converter always when app version higher then <see cref="IModelDataConverter.RequiredAppVersion"/>. Default: false
    /// </summary>
    public bool RunAlwaysOnHigherVersions { get; set; }

    /// <summary>
    /// Messages delivered by the converter
    /// </summary>
    public IList<string> Messages { get; } = new List<string>();

    /// <summary>
    /// Does the current app version stored in <see cref="IAppStartParameter.SoftwareRevision"/> require the converter to run
    /// </summary>
    /// <returns>true if the converter should run otherwise false</returns>
    public bool DoesAppVersionRequireToRunConverter()
    {
        if (UnitOfWork == null)
        {
            return false;
        }

        if (UnitOfWork.AppGlobals?.AppStartParameter?.SoftwareRevision == null)
        {
            throw new ArgumentNullException("UnitOfWork.AppGlobals.AppStartParameter.SoftwareRevision may NOT be null");
        }

        return UnitOfWork.AppGlobals.AppStartParameter.SoftwareRevision.IsEqualOrGreater(RequiredAppVersion);
    }

    /// <summary>
    /// Check if the premises are fulfilled to run the converter
    /// </summary>
    public virtual bool CheckPremisesToRunConverter()
    {
        throw new NotSupportedException("Implement an override for CheckPremisesToRunConverter()");
    }

    /// <summary>
    /// Run the converter
    /// </summary>
    public virtual void Run()
    {
        throw new NotSupportedException("Implement an override for Run()");
    }
}