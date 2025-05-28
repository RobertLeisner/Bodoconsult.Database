// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.Database.Ef.SqlServer.Helpers
{
    /// <summary>
    /// Helper class to read text or SQL resources
    /// </summary>
    public static class ResourceHelper
    {

        /// <summary>
        /// Get a text from an embedded resource file
        /// </summary>
        /// <param name="resourceName">resource name = plain file name with extension and path</param>
        /// <returns></returns>
        public static string GetTextResource(string resourceName)
        {

            resourceName = $"Bodoconsult.Database.Ef.SqlServer.Resources.{resourceName}.txt";

            var ass = typeof(ResourceHelper).Assembly;
            var str = ass.GetManifestResourceStream(resourceName);

            if (str == null)
            {
                return null;
            }

            string s;

            using (var file = new StreamReader(str))
            {
                s = file.ReadToEnd();
            }

            return s;
        }


        /// <summary>
        /// Get a SQL statement from an embedded resource file
        /// </summary>
        /// <param name="resourceName">resource name = plain file name without extension and path</param>
        /// <returns></returns>
        public static string GetSqlResource(string resourceName)
        {
            resourceName = $"Bodoconsult.Database.Ef.SqlServer.Resources.{resourceName}.sql";

            var ass = typeof(ResourceHelper).Assembly;
            var str = ass.GetManifestResourceStream(resourceName);

            if (str == null)
            {
                return null;
            }

            string s;

            using (var file = new StreamReader(str))
            {
                s = file.ReadToEnd();
            }

            return s;
        }
    }
}