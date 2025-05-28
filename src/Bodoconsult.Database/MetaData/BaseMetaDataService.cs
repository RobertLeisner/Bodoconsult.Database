// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.MetaData.Model;

namespace Bodoconsult.Database.MetaData
{
    /// <summary>
    /// Basic implementations of a meta data service
    /// </summary>
    public class BaseMetaDataService : IMetaDataService
    {
        /// <summary>
        /// Naming schema for entity objects
        /// </summary>
        public string EntityNameSchema { get; set; }

        /// <summary>
        /// Name of the IConnManager implemenation
        /// </summary>
        public string ConnManagerName { get; set; } = "DummyMetaDataService";

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="sql">Current SQL statement to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public virtual MetaDataTable GetMetaData(string connectionString, string entityName, string sql, string nameOfPrimaryKeyField = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="cmd">Current command to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public virtual MetaDataTable GetMetaData(string connectionString, string entityName, DbCommand cmd, string nameOfPrimaryKeyField = null)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Create as code for entity class
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the class code</returns>
        public virtual string CreateEntityClass(MetaDataTable table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a method for mapping from a <see cref="DbDataReader"/> to a corresponding entity class
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateMappingFromDbToEntityForDataReader(MetaDataTable table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a method do provide a new entity object filled with sample data
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateNewEntity(MetaDataTable table)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Creates a method to create a new row in the database from an entity object
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateNewEntityCommand(MetaDataTable table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a method to update the database from an entity object
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateUpdateEntityCommand(MetaDataTable table)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Creates a method to delete an entity from the database by its ID
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateDeleteEntityCommand(MetaDataTable table)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Creates a service class for the entity
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with class code</returns>
        public virtual string CreateEntityServiceClass(MetaDataTable table)
        {
            var result = new StringBuilder();


            result.AppendLine($"public class {table.DtoName}Service");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine("private readonly IConnManager _db;");
            result.AppendLine("");

            result.AppendLine($"public {table.DtoName}Service(string connectionString)");
            result.AppendLine("{");
            result.AppendLine($"_db = new {ConnManagerName}(connectionString);");

            result.AppendLine("}");
            result.AppendLine("");

            result.AppendLine(CreateNewEntityCommand(table));

            result.AppendLine(CreateUpdateEntityCommand(table));

            result.AppendLine(CreateDeleteEntityCommand(table));

            result.AppendLine(CreateGetAllEntitiesCommand(table));

            result.AppendLine(CreateGetByIdCommand(table));

            result.AppendLine(CreateCountCommand(table));

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }


        /// <summary>
        /// Creates a method to get all entities from the database
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateGetAllEntitiesCommand(MetaDataTable table)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Creates a count method for the number of rows in a table
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateCountCommand(MetaDataTable table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates a method to fetch a row by it's ID
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public virtual string CreateGetByIdCommand(MetaDataTable table)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Export all code to a directory path
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <param name="dirPath">Full path to a folder to store the exported files in</param>
        public virtual IList<string> ExportAll(MetaDataTable table, string dirPath)
        {
            if (dirPath == null)
            {
                throw new ArgumentNullException(nameof(dirPath));
            }

            var result = new List<string>();

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // Export entity class
            var content = CreateEntityClass(table);
            var fileName = Path.Combine(dirPath, $"{table.DtoName}_EntityClass_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            // Export mapping method
            content = CreateMappingFromDbToEntityForDataReader(table);
            fileName = Path.Combine(dirPath, $"{table.DtoName}_DataHelperMethod_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            // Export service class
            content = CreateEntityServiceClass(table);
            fileName = Path.Combine(dirPath, $"{table.DtoName}_ServiceClass_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            // Export service class
            content = CreateNewEntity(table);
            fileName = Path.Combine(dirPath, $"{table.DtoName}_TestDataHelperMethod_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            return result;
        }
    }
}