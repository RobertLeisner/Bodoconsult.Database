// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

/// <summary>
/// This converter checks at every app start if all expected settings (LastUpdate and Company) are available. If not they are created with default values
/// </summary>
public class AppSettingsConverterSql : BaseModelDataConverter
{
    public AppSettingsConverterSql(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
    {
        RequiredAppVersion = new Version(1, 0, 0);
        UnitOfWork = unitOfWork;
    }

    /// <summary>
    /// Check if the premises are fulfilled to run the converter
    /// </summary>
    public override bool CheckPremisesToRunConverter()
    {
        // Run it always
        return true;
    }


    /// <summary>
    /// Run the converter
    /// </summary>
    public override void Run()
    {

        // Check setting LastUpdate
        var sql = "INSERT INTO [dbo].[AppSettings] ([Key],[Value]) SELECT 'LastUpdate', null WHERE NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [key]='LastUpdate')";
        UnitOfWork.RunSql(sql);

        // Check setting Company
        sql = "INSERT INTO [dbo].[AppSettings] ([Key] ,[Value]) SELECT 'Company', 'TestCompany Ltd' WHERE NOT EXISTS (SELECT * FROM [dbo].[AppSettings] WHERE [key]='Company')";
        UnitOfWork.RunSql(sql);
    }
}