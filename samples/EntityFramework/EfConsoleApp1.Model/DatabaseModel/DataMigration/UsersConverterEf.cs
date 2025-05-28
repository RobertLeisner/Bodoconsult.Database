// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Interfaces;
using Bodoconsult.Database.Ef.MigrationTools;
using EfConsoleApp1.Model.DatabaseModel.Entities;

namespace EfConsoleApp1.Model.DatabaseModel.DataMigration;

public class UsersConverterEf : BaseModelDataConverter
{

    private readonly IRepository<UserType> _userTypeRepo;
    private readonly IRepository<Users> _usersRepo;


    public UsersConverterEf(IUnitOfWork unitOfWork, IAppLoggerProxy appLogger) : base(unitOfWork, appLogger)
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
            return _usersRepo.Any(x => x.UserTypeId == 0);
        }
    }

    /// <summary>
    /// Run the converter
    /// </summary>
    public override void Run()
    {
        using (var scope = UnitOfWork.GetContextScope())
        {

            var userTypes = _userTypeRepo.GetAll();

            var users = _usersRepo.GetAllTracked().ToList();
            foreach (var user in users)
            {

                var userType = userTypes.FirstOrDefault(x => x.UserTypeName == user.UserType);

                if (userType == null)
                {
                    continue;
                }

                user.UserType = "old";
                user.UserTypeId = userType.ID;
                _usersRepo.Update(user);
            }

            scope.SaveChanges();
        }

    }
}
