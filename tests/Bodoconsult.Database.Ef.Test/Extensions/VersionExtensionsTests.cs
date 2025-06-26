// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.Database.Ef.Extensions;

namespace Bodoconsult.Database.Ef.Test.Extensions;

[TestFixture]
public class VersionExtensionsTests
{
    //[SetUp]
    //public void Setup()
    //{
    //}



    [Test]
    public void IsEqualOrGreater_V000_V100_IsFalse()
    {
        // Arrange 
        var currentVersion = new Version(0, 0, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreater(otherVersion);

        // Assert
        Assert.That(result, Is.False);

    }

    [Test]
    public void IsEqualOrGreater_V050_V100_IsFalse()
    {
        // Arrange 
        var currentVersion = new Version(0, 5, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreater(otherVersion);

        // Assert
        Assert.That(result, Is.False);

    }

    [Test]
    public void IsEqualOrGreater_V100_V100_IsTrue()
    {
        // Arrange 
        var currentVersion = new Version(1, 0, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreater(otherVersion);

        // Assert
        Assert.That(result, Is.True);

    }

    [Test]
    public void IsEqualOrGreater_V150_V100_IsTrue()
    {
        // Arrange 
        var currentVersion = new Version(1, 5, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreater(otherVersion);

        // Assert
        Assert.That(result, Is.True);

    }


    [Test]
    public void IsEqualOrGreaterBuild_V000_V100_IsFalse()
    {
        // Arrange 
        var currentVersion = new Version(0, 0, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreaterBuild(otherVersion);

        // Assert
        Assert.That(result, Is.False);

    }

    [Test]
    public void IsEqualOrGreaterBuild_V050_V100_IsFalse()
    {
        // Arrange 
        var currentVersion = new Version(0, 5, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreaterBuild(otherVersion);

        // Assert
        Assert.That(result, Is.False);

    }

    [Test]
    public void IsEqualOrGreaterBuild_V100_V100_IsTrue()
    {
        // Arrange 
        var currentVersion = new Version(1, 0, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreaterBuild(otherVersion);

        // Assert
        Assert.That(result, Is.True);

    }

    [Test]
    public void IsEqualOrGreaterBuild_V100_V101_IsFalse()
    {
        // Arrange 
        var currentVersion = new Version(1, 0, 0);
        var otherVersion = new Version(1, 0, 1);

        // Act  
        var result = currentVersion.IsEqualOrGreaterBuild(otherVersion);

        // Assert
        Assert.That(result, Is.False);

    }

    [Test]
    public void IsEqualOrGreaterBuild_V101_V100_IsTrue()
    {
        // Arrange 
        var currentVersion = new Version(1, 0, 1);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreaterBuild(otherVersion);

        // Assert
        Assert.That(result, Is.True);

    }

    [Test]
    public void IsEqualOrGreaterBuild_V150_V100_IsTrue()
    {
        // Arrange 
        var currentVersion = new Version(1, 5, 0);
        var otherVersion = new Version(1, 0, 0);

        // Act  
        var result = currentVersion.IsEqualOrGreaterBuild(otherVersion);

        // Assert
        Assert.That(result, Is.True);

    }


}