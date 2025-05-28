// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.Database.Ef.Extensions;

public static class VersionExtensions
{
    /// <summary>
    /// Check if <see cref="currentVersion"/> is equal or greater than <see cref="otherVersion"/>. Check Major and Minor, not Build
    /// </summary>
    /// <param name="currentVersion">Version to check</param>
    /// <param name="otherVersion">Version to compare with</param>
    /// <returns>True if <see cref="currentVersion"/> is equal or greater than <see cref="otherVersion"/> else false</returns>
    public static bool IsEqualOrGreater(this Version currentVersion, Version otherVersion)
    {
        if (otherVersion == null)
        {
            return false;
        }

        if (currentVersion.Major < otherVersion.Major)
        {
            return false;
        }

        return currentVersion.Minor >= otherVersion.Minor;
    }

    /// <summary>
    /// Check if <see cref="currentVersion"/> is equal or greater than <see cref="otherVersion"/>. Check Major and Minor AND Build
    /// </summary>
    /// <param name="currentVersion">Version to check</param>
    /// <param name="otherVersion">Version to compare with</param>
    /// <returns>True if <see cref="currentVersion"/> is equal or greater than <see cref="otherVersion"/> else false</returns>
    public static bool IsEqualOrGreaterBuild(this Version currentVersion, Version otherVersion)
    {
        if (otherVersion == null)
        {
            return false;
        }

        if (currentVersion.Major < otherVersion.Major)
        {
            return false;
        }

        if (currentVersion.Minor < otherVersion.Minor)
        {
            return false;
        }

        return currentVersion.Build >= otherVersion.Build;
    }

}