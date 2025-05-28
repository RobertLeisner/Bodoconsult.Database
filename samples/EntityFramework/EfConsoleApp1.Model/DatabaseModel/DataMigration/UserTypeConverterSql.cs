// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;
using EfConsoleApp1.Model.DatabaseModel.Entities;
using Bodoconsult.Database.Ef.Interfaces;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

public class UserTypeConverterSql : BaseModelDataConverter
{
    private readonly IRepository<UserType> _userTypeRepo;

    public UserTypeConverterSql(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
    {
        RequiredAppVersion = new Version(1, 0, 0);
        UnitOfWork = unitOfWork;
        _userTypeRepo = UnitOfWork.GetRepository<UserType>();
    }

    /// <summary>
    /// Check if the premises are fulfilled to run the converter
    /// </summary>
    public override bool CheckPremisesToRunConverter()
    {
        using (UnitOfWork.GetReadOnlyContextScope())
        {
            return !_userTypeRepo.Any();
        }
    }

    /// <summary>
    /// Run the converter
    /// </summary>
    public override void Run()
    {
        const string sql = "IF NOT EXISTS(SELECT * FROM [UserType]) BEGIN INSERT INTO [UserType]([UserTypeName]) SELECT [UserType] FROM [Users] GROUP BY [UserType] END";
        UnitOfWork.RunSql(sql);
    }
}