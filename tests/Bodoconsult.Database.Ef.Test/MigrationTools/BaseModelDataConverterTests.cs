// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Interfaces;
using Bodoconsult.Database.Ef.Infrastructure;
using Bodoconsult.Database.Ef.MigrationTools;
using Bodoconsult.Database.Test.Utilities.App;
using Bodoconsult.Database.Test.Utilities.Helpers;

namespace Bodoconsult.Database.Ef.Test.MigrationTools
{
    [TestFixture]
    public class BaseModelDataConverterTests
    {
        //[SetUp]
        //public void Setup()
        //{
        //}

        #pragma warning disable NUnit1032

        private readonly IAppLoggerProxy _appLogger = TestHelper.GetFakeAppLoggerProxy();

        [Test]
        public void Ctor_ValidSetup_PropsSetCorrectly()
        {
            // Arrange 
            var uow = new SimpleFakeUnitOfWork();
            uow.AppGlobals = Globals.Instance;

            // Act  
            var result = new BaseModelDataConverter(uow, _appLogger);

            // Assert
            Assert.That(result.Messages, Is.Not.Null);
            Assert.That(result.AppLogger, Is.Not.Null);
            Assert.That(result.RequiredAppVersion, Is.Null);
        }

        [Test]
        public void DoesAppVersionRequireToRunConverter_UnitOfWorkNotLoaded_IsFalse()
        {
            // Arrange 
            var uow = new SimpleFakeUnitOfWork();
            uow.AppGlobals = Globals.Instance;

            var conv = new BaseModelDataConverter(uow, _appLogger);

            // Act  
            var result = conv.DoesAppVersionRequireToRunConverter();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void DoesAppVersionRequireToRunConverter_NoSoftwareRevision_ThrowsArgumentNullException()
        {
            // Arrange 
            var uow = new SimpleFakeUnitOfWork();
            uow.AppGlobals = Globals.Instance;

            var version = uow.AppGlobals.AppStartParameter.SoftwareRevision;
            uow.AppGlobals.AppStartParameter.SoftwareRevision = null;

            var conv = new BaseModelDataConverter(uow, _appLogger);

            // Act and assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                conv.DoesAppVersionRequireToRunConverter();
            });

            uow.AppGlobals.AppStartParameter.SoftwareRevision = version;
        }

        [Test]
        public void DoesAppVersionRequireToRunConverter_SoftwareRevisionSet_ThrowsArgumentNullException()
        {
            // Arrange 
            var uow = new SimpleFakeUnitOfWork();
            uow.AppGlobals = Globals.Instance;

            var conv = new BaseModelDataConverter(uow, _appLogger);

            // Act  
            var result = conv.DoesAppVersionRequireToRunConverter();

            // Assert
            Assert.That(result, Is.False);
        }
    }
}