using System.IO;

namespace Bodoconsult.Database.Postgres.Test.Helpers
{
    public static class TestHelper
    {
        /// <summary>
        /// Connection string to postgress db. Adjust path to your connectionstring file
        /// </summary>
        public static string PostgresConnectionString => File.ReadAllText(@"D:\Daten\Projekte\_work\Data\postgres.txt");

    }
}