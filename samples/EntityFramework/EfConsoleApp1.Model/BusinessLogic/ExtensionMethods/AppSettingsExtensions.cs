using EfConsoleApp1.Model.DatabaseModel.Entities;

namespace EfConsoleApp1.Model.BusinessLogic.ExtensionMethods;

/// <summary>
/// Extension method for <see cref="AppSettings"/> entity
/// </summary>
public static class AppSettingsExtensions
{
    /// <summary>
    /// Returns the entity data as a friendly readable string
    /// </summary>
    /// <param name="appSettings">Current entity instance</param>
    /// <returns>Entity data as a friendly readable string</returns>
    public static string ToFormattedString(this AppSettings appSettings)
    {

        return $"{appSettings.Key}: {appSettings.RowVersion}";

    }
}