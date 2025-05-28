// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Collections.Generic;
using System.Data.Common;
using Bodoconsult.Database.MetaData.Model;

namespace Bodoconsult.Database.Interfaces
{

    /// <summary>
    /// Interface for meta data services creating C# source codes for development purposes.
    /// Implementations of this interface create C# code i.e. for mappings to simple entity classes, for insert and update commands
    /// </summary>
    public interface IMetaDataService
    {

        /// <summary>
        /// Naming schema for entity objects
        /// </summary>
        public string EntityNameSchema { get; set; }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="sql">Current SQL statement to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        MetaDataTable GetMetaData(string connectionString, string entityName, string sql, string nameOfPrimaryKeyField = null);


        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="cmd">Current command to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        MetaDataTable GetMetaData(string connectionString, string entityName, DbCommand cmd, string nameOfPrimaryKeyField = null);

        /// <summary>
        /// Create as code for entity class
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the class code</returns>
        string CreateEntityClass(MetaDataTable table);


        /// <summary>
        /// Creates a method for mapping from a <see cref="DbDataReader"/> to a corresponding entity class
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateMappingFromDbToEntityForDataReader(MetaDataTable table);


        /// <summary>
        /// Creates a method do provide a new entity object filled with sample data
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateNewEntity(MetaDataTable table);


        /// <summary>
        /// Creates a method to create a new row in the database from an entity object
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateNewEntityCommand(MetaDataTable table);

        /// <summary>
        /// Creates a method to update the database from an entity object
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateUpdateEntityCommand(MetaDataTable table);


        /// <summary>
        /// Creates a method to delete an entity from the database by its ID
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateDeleteEntityCommand(MetaDataTable table);



        /// <summary>
        /// Creates a service class for the entity
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with class code</returns>
        string CreateEntityServiceClass(MetaDataTable table);


        /// <summary>
        /// Creates a method to get all entities from the database
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateGetAllEntitiesCommand(MetaDataTable table);

        /// <summary>
        /// Creates a count method for the number of rows in a table
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateCountCommand(MetaDataTable table);

        /// <summary>
        /// Creates a method to ftech a row by it's ID
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        string CreateGetByIdCommand(MetaDataTable table);

        /// <summary>
        /// Export all code to a directory path
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <param name="dirPath">Full path to a folder to store the exported files in</param>
        IList<string> ExportAll(MetaDataTable table, string dirPath);

    }
}
