using System.Collections.Generic;

namespace Bodoconsult.Database.Postgres.MetaData.Model
{
    public class DtoPostgresTable
    {
        /// <summary>
        /// Name of the database table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the corresponding dto class
        /// </summary>
        public string DtoName => $"DtoPostgres{Name}";

        /// <summary>
        /// List of all database fields
        /// </summary>
        public IList<DtoPostgresField> Fields { get; } = new List<DtoPostgresField>();

        /// <summary>
        /// SQL statement to get all records from the table or view
        /// </summary>
        public string Sql { get; set; }

    }
}