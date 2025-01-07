// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bodoconsult.Database.EntityBackup.Formatters;
using Bodoconsult.Database.Test.Helpers;
using NUnit.Framework;

namespace Bodoconsult.Database.Test.EntityBackup.Formatters;

[TestFixture]
internal class DefaultXmlEntityBackupFormatterTests
{
    [Test]
    public void Ctor_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var from = new DateTime(2023, 7, 19);
        var to = new DateTime(2023, 7, 22);

        var demoEntities = new List<DemoEntity>();

        TestHelper.GetDataForService(demoEntities, from, to);

        // Act  
        var f = new DefaultXmlEntityBackupFormatter<DemoEntity>();

        // Assert
        Assert.That(f.Data, Is.Null);

    }


    [Test]
    public void LoadData_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var from = new DateTime(2023, 7, 19);
        var to = new DateTime(2023, 7, 22);

        var demoEntities = new List<DemoEntity>();

        TestHelper.GetDataForService(demoEntities, from, to);

        var f = new DefaultXmlEntityBackupFormatter<DemoEntity>();

        // Act  

        f.LoadData(demoEntities);

        // Assert
        Assert.That(f.Data, Is.Not.Null);

    }


    [Test]
    public void GetResult_ValidSetup_PropsSetCorrectly()
    {
        // Arrange 
        var from = new DateTime(2023, 7, 19);
        var to = new DateTime(2023, 7, 22);

        var demoEntities = new List<DemoEntity>();

        TestHelper.GetDataForService(demoEntities, from, to);

        var f = new DefaultXmlEntityBackupFormatter<DemoEntity>();
        f.LoadData(demoEntities);

        Assert.That(f.Data, Is.Not.Null);

        // Act  
        var result = f.GetResult();


        // Assert
        Assert.That(string.IsNullOrEmpty(result.ToString()), Is.False);

        Debug.Print(result.ToString());

    }

}