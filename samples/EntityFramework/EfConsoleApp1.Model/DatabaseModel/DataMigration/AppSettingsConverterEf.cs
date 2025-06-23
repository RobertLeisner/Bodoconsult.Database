// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;
using EfConsoleApp1.Model.DatabaseModel.Entities;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

/// <summary>
/// This converter checks at every app start if all expected settings (LastUpdate and Company) are available. If not they are created with default values
/// </summary>
public class AppSettingsConverterEf: BaseModelDataConverter
{
    private readonly IRepository<AppSettings> _repo;

    public AppSettingsConverterEf(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
    {
        RequiredAppVersion = new Version(1, 0, 0);
        UnitOfWork = unitOfWork;
        _repo = UnitOfWork.GetRepository<AppSettings>();
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
        CheckLastUpdateSetting();

        // Check setting Company
        CheckCompanySetting();
    }

    public void CheckLastUpdateSetting()
    {
        using (var scope = UnitOfWork.GetContextScope())
        {
            if (_repo.Any(x => x.Key == "LastUpdate"))
            {
                return;
            }

            var setting = new AppSettings
            {
                Key = "LastUpdate"
            };

            _repo.Add(setting);

            scope.SaveChanges();
        }
    }

    public void CheckCompanySetting()
    {
        using (var scope = UnitOfWork.GetContextScope())
        {
            if (_repo.Any(x => x.Key == "Company"))
            {
                return;
            }

            var setting = new AppSettings
            {
                Key = "Company",
                Value = "TestCompany Ltd"
            };

            _repo.Add(setting);

            scope.SaveChanges();
        }
    }
}