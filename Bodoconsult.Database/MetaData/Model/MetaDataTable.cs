// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;

namespace Bodoconsult.Database.MetaData.Model
{
    public class MetaDataTable
    {
        /// <summary>
        /// Naming schema for entity objects. Default "{0}"
        /// </summary>
        public string EntityNameSchema { get; set; } = "{0}";

        /// <summary>
        /// Name of the database table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the corresponding entity class
        /// </summary>
        public string DtoName => string.Format(EntityNameSchema, Name);

        /// <summary>
        /// List of all database fields
        /// </summary>
        public IList<MetaDataField> Fields { get; } = new List<MetaDataField>();

        /// <summary>
        /// SQL statement to get records from the table, view  or stored procedure
        /// </summary>
        public string Sql { get; set; }

    }
}