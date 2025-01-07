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
    public class BaseMetaDataService: IMetaDataService
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
        /// Table with meta data 
        /// </summary>
        public MetaDataTable Table { get; set; }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="sql">Current SQL statement to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public virtual void GetMetaData(string connectionString, string entityName, string sql, string nameOfPrimaryKeyField = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="cmd">Current command to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public virtual void GetMetaData(string connectionString, string entityName, DbCommand cmd, string nameOfPrimaryKeyField = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create as code for entity class
        /// </summary>
        /// <returns>string with the class code</returns>
        public virtual string CreateEntityClass()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a method for mapping from a <see cref="DataReader"/> to a corresponding entity class
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateMappingFromDbToEntityForDataReader()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a method do provide a new entity object filled with sample data
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateNewEntity()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a method to create a new row in the database from an entity object
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateNewEntityCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a method to update the database from an entity object
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateUpdateEntityCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a method to delete an entity from the database by its ID
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateDeleteEntityCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a service class for the entity
        /// </summary>
        /// <returns>string with class code</returns>
        public virtual string CreateEntityServiceClass()
        {
            var result = new StringBuilder();


            result.AppendLine($"public class {Table.DtoName}Service");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine("private readonly IConnManager _db;");
            result.AppendLine("");

            result.AppendLine($"public {Table.DtoName}Service(string connectionString)");
            result.AppendLine("{");
            result.AppendLine($"_db = new {ConnManagerName}(connectionString);");

            result.AppendLine("}");
            result.AppendLine("");

            result.AppendLine(CreateNewEntityCommand());

            result.AppendLine(CreateUpdateEntityCommand());

            result.AppendLine(CreateDeleteEntityCommand());

            result.AppendLine(CreateGetAllEntitiesCommand());

            result.AppendLine(CreateGetByIdCommand());

            result.AppendLine(CreateCountCommand());

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }

        /// <summary>
        /// Creates a method to get all entities from the database
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateGetAllEntitiesCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a count method for the number of rows in a table
        /// </summary>
        /// <returns>string with the method code</returns>
        public virtual string CreateCountCommand()
        {
            throw new NotImplementedException();
        }

        public virtual string CreateGetByIdCommand()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Export all code to a directory path
        /// </summary>
        /// <param name="dirPath"></param>
        public virtual IList<string> ExportAll(string dirPath)
        {

            var result = new List<string>();

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // Export entity class
            var content = CreateEntityClass();
            var fileName = Path.Combine(dirPath, $"{Table.DtoName}_EntityClass_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            // Export mapping method
            content = CreateMappingFromDbToEntityForDataReader();
            fileName = Path.Combine(dirPath, $"{Table.DtoName}_DataHelperMethod_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            // Export service class
            content = CreateEntityServiceClass();
            fileName = Path.Combine(dirPath, $"{Table.DtoName}_ServiceClass_Code.txt");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.WriteAllText(fileName, content, Encoding.UTF8);
            result.Add(fileName);

            // Export service class
            content = CreateNewEntity();
            fileName = Path.Combine(dirPath, $"{Table.DtoName}_TestDataHelperMethod_Code.txt");
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