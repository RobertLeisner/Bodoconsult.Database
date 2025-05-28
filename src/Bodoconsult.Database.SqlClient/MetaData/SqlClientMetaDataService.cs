// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.MetaData;
using Bodoconsult.Database.MetaData.Model;
using Microsoft.Data.SqlClient;

namespace Bodoconsult.Database.SqlClient.MetaData
{
    /// <summary>
    /// SqlClient database implementation of a meta data service
    /// </summary>
    public class SqlClientMetaDataService : BaseMetaDataService
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public SqlClientMetaDataService()
        {
            ConnManagerName = "SqlClientConnManager";
        }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="sql">Current SQL statement to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public override MetaDataTable GetMetaData(string connectionString, string entityName, string sql, string nameOfPrimaryKeyField = null)
        {
            var cmd = new SqlCommand(sql);
            return GetMetaData(connectionString, entityName, cmd, nameOfPrimaryKeyField);
        }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// /// <param name="cmd">Current command to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public override MetaDataTable GetMetaData(string connectionString, string entityName, DbCommand cmd, string nameOfPrimaryKeyField = null)
        {
            var table = new MetaDataTable { Name = entityName };

            IConnManager db = new SqlClientConnManager(connectionString);

            table.Sql = cmd.CommandText;

            // Act
            var reader = (SqlDataReader)db.GetDataReader(cmd);

            //
            var schema = reader.GetColumnSchema();
            for (var i = 0; i < schema.Count; i++)
            {
                var col = schema[i];

                var colItem = new MetaDataField
                {
                    Name = col.ColumnName,
                    DatabaseType = col.DataType,
                    SourceDataType = reader.GetProviderSpecificFieldType(i).ToString(),
                    IsPrimaryKey = col.ColumnName == nameOfPrimaryKeyField
                };

                //Debug.Print(colItem.SourceDataType);

                if (col.ColumnSize != null)
                {
                    colItem.MaxLength = (int)col.ColumnSize;
                }

                table.Fields.Add(colItem);
            }
            return table;
        }


        /// <summary>
        /// Create as code for entity class
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the class code</returns>
        public override string CreateEntityClass(MetaDataTable table)
        {
            var result = new StringBuilder();


            result.AppendLine($"public class {table.DtoName}");
            result.AppendLine("{");

            foreach (var field in table.Fields)
            {
                result.AppendLine("");
                result.AppendLine(GetFieldProperty(field));
                result.AppendLine("");
            }

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }


        /// <summary>
        /// Creates a method for mapping from a <see cref="DataReader"/> to a corresponding entity class
        /// </summary>
        /// <returns>string with the method code</returns>
        public override string CreateMappingFromDbToEntityForDataReader(MetaDataTable table)
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Mapping datareader to entity class {table.DtoName}\r\n/// </summary>");

            result.AppendLine($"public static {table.DtoName} MapFromDbTo{table.DtoName}(IDataReader reader)");
            result.AppendLine("{");
            result.AppendLine("");

            result.AppendLine($"\tvar dto = new {table.DtoName}();");

            for (var index = 0; index < table.Fields.Count; index++)
            {
                var field = table.Fields[index];
                result.AppendLine($"\t{GetFieldMappingFromDb(field, index)}");
            }

            result.AppendLine("");
            result.AppendLine("\treturn dto;");

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }


        private static string GetFieldValues(MetaDataField field)
        {
            var result = $"{field.Name}=";

            switch (field.DatabaseType.Name.ToUpperInvariant())
            {
                case "BYTE":
                case "SBYTE":
                case "INT16":
                case "INT32":
                case "INT64":
                case "UINT16":
                case "UINT32":
                case "UINT64":
                    result += "1";
                    break;
                //case "SINGLE":
                //    result += "(Single)1.0";
                //    break;
                case "DOUBLE":
                    result += "1.0f";
                    break;
                case "FLOAT":
                    result += "1.0f";
                    break;
                case "DECIMAL":
                    result += "(decimal)1.0";
                    break;
                case "BOOLEAN":
                    result += "true";
                    break;
                case "DATETIME":
                    result += "DateTime.Now";
                    break;
                case "CHAR":
                    result += "'a'";
                    break;
                case "STRING":
                    if (field.MaxLength > 20)
                    {
                        result += "\"Test mit ü und ö\"";
                    }
                    else
                    {
                        result += "\"T\"";
                    }

                    break;
                default:
                    break;
            }

            return $"{result},\r\n";
        }


        private static string GetFieldTypeFromType(Type type)
        {
            var result = "";

            switch (type.Name.ToLowerInvariant())
            {
                case "byte":
                    result = "SqlDbType.TinyInt";
                    break;
                case "int16":
                    result = "SqlDbType.SmallInt";
                    break;
                case "short":
                case "int32":
                    result = "SqlDbType.Int";
                    break;
                case "long":
                case "int64":
                    result = "SqlDbType.BigInt";
                    break;
                case "uint16":
                    result = "SqlDbType.SmallInt";
                    break;
                case "uint32":
                    result = "SqlDbType.Int";
                    break;
                case "uint64":
                    result = "SqlDbType.BigInt";
                    break;
                case "single":
                    result = "SqlDbType.Real";
                    break;
                case "float":
                case "double":
                    result = "SqlDbType.Double";
                    break;
                case "currency":
                    result = "SqlDbType.Decimal";
                    break;
                case "decimal":
                    result = "SqlDbType.Decimal";
                    break;
                case "bool":
                case "boolean":
                    result = "SqlDbType.Bit";
                    break;
                case "datetime":
                    result = "SqlDbType.DateTime";
                    break;
                case "string":
                    result = "SqlDbType.NVarChar";
                    break;
                default:
                    break;
            }

            return result;
        }


        private static string GetFieldMappingFromDb(MetaDataField field, int index)
        {
            var fieldName = field.Name[..1].ToUpper() +
                            field.Name[1..];

            var result = "";

            switch (field.DatabaseType.Name.ToLowerInvariant())
            {
                case "byte":
                    result = $"dto.{fieldName}=reader.GetByte({index});";
                    break;
                case "int16":
                    result = $"dto.{fieldName}=reader.GetInt16({index});";
                    break;
                case "int32":
                    result = $"dto.{fieldName}=reader.GetInt32({index});";
                    break;
                case "int64":
                    result = $"dto.{fieldName}=reader.GetInt64([{index});";
                    break;
                case "uint16":
                    result = $"dto.{fieldName}=reader.GetUInt16({index});";
                    break;
                case "uint32":
                    result = $"dto.{fieldName}=reader.GetUInt32({index});";
                    break;
                case "uint64":
                    result = $"dto.{fieldName}=reader.GetUInt64({index});";
                    break;
                case "single":
                    result = $"dto.{fieldName}=reader.GetSingle({index});";
                    break;
                case "double":
                    result = $"dto.{fieldName}=reader.GetDouble({index});";
                    break;
                case "currency":
                    result = $"dto.{fieldName}=reader.GetDecimal({index});";
                    break;
                case "decimal":
                    result = $"dto.{fieldName}=reader.GetDecimal({index});";
                    break;
                case "boolean":
                    result = $"dto.{fieldName}=reader.GetBoolean({index});";
                    break;
                case "datetime":
                    result = $"dto.{fieldName}=reader.GetDate({index});";
                    break;
                case "string":
                    result = $"dto.{fieldName}=reader[{index}].ToString();";
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get a C# property for a database field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetFieldProperty(MetaDataField field)
        {

            var type = field.DatabaseType;

            var attr = type == typeof(string) ? $"\t[MaxLength({field.MaxLength}, ErrorMessage = \"Maximum number of characters that can be entered for {field.Name} is {field.MaxLength}!\")]\r\n" : "";


            var comment = type == typeof(string) ? $" Maximum length: {field.MaxLength} chars" : "";

            comment = $"\t/// <summary>\r\n\t/// {field.Name} {comment}\r\n\t/// </summary>\r\n";

            var fieldName = field.Name[..1].ToUpper() +
                            field.Name[1..];


            var result = $"{comment}{attr}\tpublic {type.Name} {fieldName} {{ get; set; }}";


            return result;

        }


        private static StringBuilder GetFieldParameter(MetaDataField field)
        {
            var erg = new StringBuilder();

            erg.AppendLine($"// Parameter @{field.Name}");
            erg.AppendLine($"p = new SqlParameter(\"@{field.Name}\", {GetFieldTypeFromType(field.DatabaseType)}) {{ Value = item.{field.Name} }};");
            erg.AppendLine("cmd.Parameters.Add(p); ");
            erg.AppendLine("");

            return erg;
        }


        /// <summary>
        /// Creates a method do provide a new entity object filled with sample data
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateNewEntity(MetaDataTable table)
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Get a filled new object from DTO class {table.DtoName} object\r\n/// </summary>");

            result.AppendLine($"public static {table.DtoName} New{table.DtoName}()");
            result.AppendLine("{");
            result.AppendLine("");

            result.AppendLine($"\tvar item = new {table.DtoName}{{");

            // Add parameters
            foreach (var field in table.Fields)
            {
                result.Append($"\t\t{GetFieldValues(field)}");
            }
            result.AppendLine("\t};");
            result.AppendLine("");

            result.AppendLine("\treturn item;");

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }

        /// <summary>
        /// Creates a method to create a new row in the database from an entity object
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateNewEntityCommand(MetaDataTable table)
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Insert a data row into table {table.Name} from entity class {table.DtoName} object\r\n/// </summary>");

            result.AppendLine($"public void AddNew({table.DtoName} item)");
            result.AppendLine("{");
            result.AppendLine("");

            // Fieldlist
            result.Append($"const string sql = \"INSERT INTO [{table.Name}] (");

            var fieldData = "";

            foreach (var field in table.Fields)
            {
                fieldData += $"[{field.Name}], ";
            }

            result.Append(fieldData[..^2]);
            result.Append(") \"+ \r\n");

            result.Append("\t\t\"VALUES (");


            // Parameter list
            fieldData = "";

            foreach (var field in table.Fields)
            {
                fieldData += $"@{field.Name}, ";
            }

            result.Append(fieldData[..^2]);
            result.Append(")\";\r\n");

            result.AppendLine("");
            result.AppendLine("var cmd = new SqlCommand(sql);");
            result.AppendLine("");
            result.AppendLine("SqlParameter p;");


            result.AppendLine("");

            // Add parameters
            foreach (var field in table.Fields)
            {
                result.Append(GetFieldParameter(field));

            }

            result.AppendLine("_db.Exec(cmd);");

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }

        /// <summary>
        /// Creates a method to update the database from an entity object
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateUpdateEntityCommand(MetaDataTable table)
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Update a data row in table {table.Name} from an entity class {table.DtoName} object\r\n/// </summary>");

            result.AppendLine($"public void Update({table.DtoName} item)");
            result.AppendLine("{");
            result.AppendLine("");

            // Fieldlist
            result.Append($"const string sql = \"UPDATE [{table.Name}] SET ");

            var fieldData = "";

            foreach (var field in table.Fields)
            {
                if (field.IsPrimaryKey)
                {
                    continue;
                }
                fieldData += $"[{field.Name}]=@{field.Name}, ";
            }

            result.Append(fieldData[..^2]);

            var pk = table.Fields.FirstOrDefault(x => x.IsPrimaryKey);

            if (pk != null)
            {
                var where = $" WHERE \\\"{pk.Name}\\\"=@{pk.Name};";
                result.Append($"{where} \";\r\n");
            }
            else
            {
                result.Append(" \";\r\n");
            }

            result.AppendLine("");
            result.AppendLine("var cmd = new SqlCommand(sql);");
            result.AppendLine("");
            result.AppendLine("SqlParameter p;");


            result.AppendLine("");

            // Add parameters
            foreach (var field in table.Fields)
            {
                result.Append(GetFieldParameter(field));
            }

            result.AppendLine("_db.Exec(cmd);");

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }


        ///// <summary>
        ///// Creates a service class for the entity
        ///// </summary>
        ///// <returns>string with class code</returns>
        //public override string CreateEntityServiceClass()
        //{
        //    throw new NotImplementedException();
        //}


        /// <summary>
        /// Creates a method to get all entities from the database
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateGetAllEntitiesCommand(MetaDataTable table)
        {

            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Get all rows in table {table.Name}\r\n/// </summary>");
            result.AppendLine($"public IList<{table.DtoName}> GetAll()");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine($"var result = new List<{table.DtoName}>();");
            result.AppendLine("");
            result.AppendLine($"var reader = _db.GetDataReader(\"SELECT * FROM [{table.Name}]\"); ");
            result.AppendLine("");
            result.AppendLine("while (reader.Read())");
            result.AppendLine("{");
            result.AppendLine($"\tvar dto = DataHelper.MapFromDbTo{table.DtoName}(reader);");
            result.AppendLine("\tresult.Add(dto);");
            result.AppendLine("");
            result.AppendLine("}");
            result.AppendLine("");
            result.AppendLine("reader.Dispose();");
            result.AppendLine("");
            result.AppendLine("return result;");

            result.AppendLine("}");


            return result.ToString();
        }


        /// <summary>
        /// Creates a count method for the number of rows in a table
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateCountCommand(MetaDataTable table)
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Count all rows in table {table.Name} \r\n/// </summary>");
            result.AppendLine("public int Count()");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine($"var result = _db.ExecWithResult(\"SELECT COUNT(*) FROM [{table.Name}]\"); ");
            result.AppendLine("");
            result.AppendLine("return Convert.ToInt32(result);");

            result.AppendLine("}");


            return result.ToString();
        }

        /// <summary>
        /// Creates a method to fetch a row by it's ID
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateGetByIdCommand(MetaDataTable table)
        {
            var result = new StringBuilder();

            var pk = table.Fields.FirstOrDefault(x => x.IsPrimaryKey);
            if (pk == null)
            {
                result.AppendLine("// No primary key field defined for method GetById");
                return result.ToString();
            }
            result.AppendLine($"/// <summary>\r\n/// Get all rows in table {table.Name}\r\n/// </summary>");
            result.AppendLine($"public {table.DtoName} GetById({pk.DatabaseType} pk{pk.Name})");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine($"{table.DtoName} dto = null;");
            result.AppendLine("");
            result.AppendLine($"var reader = _db.GetDataReader($\"SELECT * FROM [{table.Name}] WHERE [{pk.Name}]={{pk{pk.Name}}};\"); ");
            result.AppendLine("");
            result.AppendLine("while (reader.Read())");
            result.AppendLine("{");
            result.AppendLine($"\tdto = DataHelper.MapFromDbTo{table.DtoName}(reader);");
            result.AppendLine("\tbreak;");
            result.AppendLine("");
            result.AppendLine("}");
            result.AppendLine("");
            result.AppendLine("reader.Dispose();");
            result.AppendLine("");
            result.AppendLine("return dto;");

            result.AppendLine("}");


            return result.ToString();
        }

        ///// <summary>
        ///// Creates a service class for the entity
        ///// </summary>
        ///// <returns>string with class code</returns>
        //public override string CreateEntityServiceClass()
        //{
        //    var result = new StringBuilder();


        //    result.AppendLine($"public class {table.DtoName}Service");
        //    result.AppendLine("{");

        //    result.AppendLine("");
        //    result.AppendLine("private readonly IConnManager _db;");
        //    result.AppendLine("");

        //    result.AppendLine($"public {table.DtoName}Service(string connectionString)");
        //    result.AppendLine("{");
        //    result.AppendLine("_db = new SqlClientConnManager(connectionString);");

        //    result.AppendLine("}");
        //    result.AppendLine("");

        //    result.AppendLine(CreateNewEntityCommand());

        //    result.AppendLine(CreateUpdateEntityCommand());

        //    result.AppendLine(CreateGetAllEntitiesCommand());

        //    result.AppendLine(CreateGetByIdCommand());

        //    result.AppendLine(CreateCountCommand());

        //    result.AppendLine("");
        //    result.AppendLine("}");

        //    return result.ToString();
        //}


        /// <summary>
        /// Creates a method to delete an entity from the database by its ID
        /// </summary>
        /// <param name="table">Current table meta data</param>
        /// <returns>string with the method code</returns>
        public override string CreateDeleteEntityCommand(MetaDataTable table)
        {
            var result = new StringBuilder();

            var pk = table.Fields.FirstOrDefault(x => x.IsPrimaryKey);
            if (pk == null)
            {
                result.AppendLine("// No primary key field defined for method GetById");
                return result.ToString();
            }

            var paraName = pk.Name[..1].ToLowerInvariant() + pk.Name[1..];

            result.AppendLine($"/// <summary>\r\n/// Delete a row from table {table.Name} \r\n/// </summary>");
            result.AppendLine($"public void Delete({pk.DatabaseType} {paraName})");
            result.AppendLine("{");
            result.AppendLine("");
            result.AppendLine($"\tvar cmd = new SqlCommand(\"DELETE FROM [{table.Name}] WHERE [{pk.Name}] = @PK\");");
            result.AppendLine("");
            result.AppendLine($"\tvar p = new SqlParameter(\"@PK\", {GetFieldTypeFromType(pk.DatabaseType)}) {{Value = {paraName}}};");
            result.AppendLine("\tcmd.Parameters.Add(p);");
            result.AppendLine("");
            result.AppendLine("\t_db.Exec(cmd);");
            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }

    }
}