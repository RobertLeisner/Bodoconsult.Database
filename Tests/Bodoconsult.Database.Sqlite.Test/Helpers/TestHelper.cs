using System.IO;
using System.Reflection;

namespace Bodoconsult.Database.Sqlite.Test.Helpers
{
    public static class TestHelper
    {

        private static string dbPath;

        /// <summary>
        /// Connection string to postgress db. Adjust path to your connectionstring file
        /// </summary>
        public static string SqliteConnectionString
        {
            get
            {

                if (string.IsNullOrEmpty(dbPath))
                {
                    var path = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.Parent.FullName;

                    dbPath = Path.Combine(path, "Chinook_Sqlite.sqlite");
                }


                
                return $"Data Source={dbPath};";
            }
        }


        /// <summary>
        /// Get a text from a embedded resource file
        /// </summary>
        /// <param name="resourceName">resource name = file name</param>
        /// <returns></returns>
        public static string GetTextResource(string resourceName)
        {
            var ass = Assembly.GetExecutingAssembly();
            var str = ass.GetManifestResourceStream(resourceName);

            if (str == null) return null;

            string s;

            using (var file = new StreamReader(str))
            {
                s = file.ReadToEnd();
            }

            return s;
        }
    }
}