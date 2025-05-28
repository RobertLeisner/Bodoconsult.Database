// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;
using EfConsoleApp1.Model.DatabaseModel.Entities;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

public class UsersConverterSql : BaseModelDataConverter
{

    private readonly IRepository<Users> _usersRepo;

    public UsersConverterSql(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
    {
        RequiredAppVersion = new Version(1, 0, 0);
        UnitOfWork = unitOfWork;
        _usersRepo = UnitOfWork.GetRepository<Users>();
    }

    /// <summary>
    /// Check if the premises are fulfilled to run the converter
    /// </summary>
    public override bool CheckPremisesToRunConverter()
    {
        using (UnitOfWork.GetReadOnlyContextScope())
        {
            return _usersRepo.Any(x => x.UserTypeId == 0);
        }
    }

    /// <summary>
    /// Run the converter
    /// </summary>
    public override void Run()
    {
        const string sql = "UPDATE dbo.Users SET UserTypeId=ut.ID, UserType='old' FROM dbo.Users [u] INNER JOIN dbo.UserType [ut] ON  u.UserType=ut.UserTypeName";
        UnitOfWork.RunSql(sql);
    }
}