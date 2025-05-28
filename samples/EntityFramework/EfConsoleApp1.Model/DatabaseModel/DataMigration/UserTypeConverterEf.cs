// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;
using EfConsoleApp1.Model.DatabaseModel.Entities;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

public class UserTypeConverterEf : BaseModelDataConverter
{
    private readonly IRepository<UserType> _userTypeRepo;
    private readonly IRepository<Users> _usersRepo;

    public UserTypeConverterEf(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
    {
        RequiredAppVersion = new Version(1, 0, 0);
        UnitOfWork = unitOfWork;
        _userTypeRepo = UnitOfWork.GetRepository<UserType>();
        _usersRepo = UnitOfWork.GetRepository<Users>();
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
        using (var scope = UnitOfWork.GetContextScope())
        {
            var userTypes = _usersRepo.GetAll().GroupBy(x => x.UserType);

            foreach (var userType in userTypes)
            {

                var ut = new UserType
                {
                    UserTypeName = userType.Key
                };
                _userTypeRepo.Add(ut);
            }

            scope.SaveChanges();
        }
    }
}