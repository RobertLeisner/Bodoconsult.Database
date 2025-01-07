// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using NUnit.Framework;

namespace Bodoconsult.Database.SqlClient.Test;

[TestFixture]
public class UnitTestBcg
{

    [Test]
    public void Test()
    {
        // Arrange 
        var conn = "Data Source=bcgs03ds;Initial Catalog=BodoCMS_Bildarchiv;Trusted_Connection=True;TrustServerCertificate=True;";

        var db = new SqlClientConnManager(conn);

        // Act  
        var data = db.GetDataTable("EXEC dbo.spImages_NoThumbs");

        // Assert
        Assert.That(data, Is.Not.Null);

    }
    
}