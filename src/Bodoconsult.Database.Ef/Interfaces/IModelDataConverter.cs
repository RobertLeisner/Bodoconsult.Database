// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;

namespace Bodoconsult.Database.Ef.Interfaces
{
    /// <summary>
    /// Converts model data
    /// </summary>
    public interface IModelDataConverter
    {

        /// <summary>
        /// The required app version for running the converter
        /// </summary>
        Version RequiredAppVersion { get; }

        /// <summary>
        /// Messages delivered by the converter
        /// </summary>
        IList<string> Messages { get;  }

        /// <summary>
        /// Does the current app version stored in <see cref="IAppStartParameter.SoftwareRevision"/> require the converter to run
        /// </summary>
        /// <returns>true if the converter should run otherwise false</returns>
        bool DoesAppVersionRequireToRunConverter();

        /// <summary>
        /// Check if the premises are fulfilled to run the converter
        /// </summary>
        bool CheckPremisesToRunConverter();


        /// <summary>
        /// Run the converter always when app version higher then <see cref="RequiredAppVersion"/>. Default: false
        /// </summary>
        bool RunAlwaysOnHigherVersions { get; set; }

        /// <summary>
        /// Run the converter
        /// </summary>
        void Run();

    }
}