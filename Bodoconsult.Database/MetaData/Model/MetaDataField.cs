// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;

namespace Bodoconsult.Database.MetaData.Model
{
    public class MetaDataField
    {
        /// <summary>
        /// Name of the database table field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Database data type of the field
        /// </summary>
        public Type DatabaseType { get; set; }

        /// <summary>
        /// Max length for string fields
        /// </summary>
        public int MaxLength { get; set; }


        /// <summary>
        /// Source data type from provider as string
        /// </summary>
        public string SourceDataType { get; set; }

        /// <summary>
        /// Is the current field the primary key field
        /// </summary>
        public bool IsPrimaryKey { get; set; }


        ///// <summary>
        ///// Auto incremented value
        ///// </summary>
        //public bool AutoWert { get; set; }
    }
}