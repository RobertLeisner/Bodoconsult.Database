// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Data.Common;
using System.Linq;
using System.Text;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.MetaData;
using Bodoconsult.Database.MetaData.Model;
using Npgsql;

namespace Bodoconsult.Database.Postgres.MetaData
{
    /// <summary>
    /// Postgres database implementation of a meta data service
    /// </summary>
    public class PostgresMetaDataService : BaseMetaDataService
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public PostgresMetaDataService()
        {
            ConnManagerName = "PostgresConnManager";
        }


        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// <param name="sql">Current SQL statement to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public override void GetMetaData(string connectionString, string entityName, string sql, string nameOfPrimaryKeyField = null)
        {
            var cmd = new NpgsqlCommand(sql);
            GetMetaData(connectionString, entityName, cmd, nameOfPrimaryKeyField);
        }

        /// <summary>
        /// Get meta data from a SQL statement
        /// </summary>
        /// <param name="connectionString">Connection string to use</param>
        /// <param name="entityName">Name of the entity class</param>
        /// /// <param name="cmd">Current command to get meta data for</param>
        /// <param name="nameOfPrimaryKeyField">Name of the primary key field. Default: null</param>
        public override void GetMetaData(string connectionString, string entityName, DbCommand cmd, string nameOfPrimaryKeyField = null)
        {
            var table = new MetaDataTable { Name = entityName };

            IConnManager db = new PostgresConnManager(connectionString);

            table.Sql = cmd.CommandText;

            // Act
            var reader = (NpgsqlDataReader)db.GetDataReader(cmd);

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

            //var p = new NpgsqlParameter()

            Table = table;
        }

        /// <summary>
        /// Create as code for entity class
        /// </summary>
        /// <returns>string with the class code</returns>
        public override string CreateEntityClass()
        {
            var result = new StringBuilder();


            result.AppendLine($"public class {Table.DtoName}");
            result.AppendLine("{");

            foreach (var field in Table.Fields)
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
        public override string CreateMappingFromDbToEntityForDataReader()
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Mapping datareader to entity class {Table.DtoName}\r\n/// </summary>");

            result.AppendLine($"public static {Table.DtoName} MapFromDbTo{Table.DtoName}(IDataReader reader)");
            result.AppendLine("{");
            result.AppendLine("");

            result.AppendLine($"\tvar dto = new {Table.DtoName}();");

            for (var index = 0; index < Table.Fields.Count; index++)
            {
                var field = Table.Fields[index];
                result.AppendLine("\t" + GetFieldMappingFromDb(field, index));
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

            return result + ",\r\n";
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
                    result = $"dto.{fieldName}=reader.GetInt64({index});";
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
                    result = $"var x{index}= (NpgsqlDateTime)reader.GetProviderSpecificValue({index});\r\n\tif (!x.IsNegativeInfinity) dto.{fieldName}=x;";
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

        private static string GetFieldTypeFromType(Type type)
        {
            var result = "";

            switch (type.Name.ToLowerInvariant())
            {
                case "byte":
                    result = "NpgsqlDbType.Bit";
                    break;
                case "int16":
                    result = "NpgsqlDbType.SmallInt";
                    break;
                case "short":
                case "int32":
                    result = "NpgsqlDbType.Integer";
                    break;
                case "long":
                case "int64":
                    result = "NpgsqlDbType.BigInt";
                    break;
                case "uint16":
                    result = "NpgsqlDbType.SmallInt";
                    break;
                case "uint32":
                    result = "NpgsqlDbType.Integer";
                    break;
                case "uint64":
                    result = "NpgsqlDbType.BigInt";
                    break;
                case "single":
                    result = "NpgsqlDbType.Real";
                    break;
                case "float":
                case "double":
                    result = "NpgsqlDbType.Real";
                    break;
                case "currency":
                    result = "NpgsqlDbType.Money";
                    break;
                case "decimal":
                    result = "NpgsqlDbType.Numeric";
                    break;
                case "bool":
                case "boolean":
                    result = "NpgsqlDbType.Bit";
                    break;
                case "datetime":
                    result = "NpgsqlDbType.Date";
                    break;
                case "string":
                    result = "NpgsqlDbType.Varchar";
                    break;
                default:
                    break;
            }

            return result;
        }


        private static StringBuilder GetFieldParameter(MetaDataField field)
        {
            var erg = new StringBuilder();

            erg.AppendLine($"// Parameter @{field.Name}");
            erg.AppendLine($"p = new NpgsqlParameter(\"@{field.Name}\", {GetFieldTypeFromType(field.DatabaseType)}) {{ Value = item.{field.Name} }};");
            erg.AppendLine($"cmd.Parameters.Add(p); ");
            erg.AppendLine("");

            return erg;
        }




        /// <summary>
        /// Creates a method do provide a new entity object filled with sample data
        /// </summary>
        /// <returns>string with the method code</returns>
        public override string CreateNewEntity()
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Get a filled new object from DTO class {Table.DtoName} object\r\n/// </summary>");

            result.AppendLine($"public static {Table.DtoName} New{Table.DtoName}()");
            result.AppendLine("{");
            result.AppendLine("");

            result.AppendLine($"\tvar item = new {Table.DtoName}{{");

            // Add parameters
            foreach (var field in Table.Fields)
            {
                result.Append("\t\t" + GetFieldValues(field));
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
        /// <returns>string with the method code</returns>
        public override string CreateNewEntityCommand()
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Insert a data row into table {Table.Name} from entity class {Table.DtoName} object\r\n/// </summary>");

            result.AppendLine($"public void AddNew({Table.DtoName} item)");
            result.AppendLine("{");
            result.AppendLine("");

            // Fieldlist
            result.Append($"const string sql = \"INSERT INTO \\\"{Table.Name}\\\"(");

            var fieldData = "";

            foreach (var field in Table.Fields)
            {
                fieldData += $"\\\"{field.Name}\\\", ";
            }

            result.Append(fieldData[..^2]);
            result.Append(") \"+ \r\n");

            result.Append($"\t\t\"VALUES (");


            // Parameter list
            fieldData = "";

            foreach (var field in Table.Fields)
            {
                fieldData += "@" + field.Name + ", ";
            }

            result.Append(fieldData[..^2]);
            result.Append(")\";\r\n");

            result.AppendLine("");
            result.AppendLine("var cmd = new NpgsqlCommand(sql);");
            result.AppendLine("");
            result.AppendLine("NpgsqlParameter p;");


            result.AppendLine("");

            // Add parameters
            foreach (var field in Table.Fields)
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
        /// <returns>string with the method code</returns>
        public override string CreateUpdateEntityCommand()
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Update a data row in table {Table.Name} from an entity class {Table.DtoName} object\r\n/// </summary>");

            result.AppendLine($"public void Update({Table.DtoName} item)");
            result.AppendLine("{");
            result.AppendLine("");

            // Fieldlist
            result.Append($"const string sql = \"UPDATE \\\"{Table.Name}\\\" SET ");

            var fieldData = "";

            foreach (var field in Table.Fields)
            {
                if (field.IsPrimaryKey) continue;
                fieldData += $"\\\"{field.Name}\\\"=@{field.Name}, ";
            }

            result.Append(fieldData[..^2]);

            var pk = Table.Fields.FirstOrDefault(x => x.IsPrimaryKey);

            if (pk != null)
            {
                var where = $" WHERE \\\"{pk.Name}\\\"=@{pk.Name};";
                result.Append(where + " \";\r\n");
            }
            else
            {
                result.Append(" \";\r\n");
            }

            result.AppendLine("");
            result.AppendLine("var cmd = new NpgsqlCommand(sql);");
            result.AppendLine("");
            result.AppendLine("NpgsqlParameter p;");


            result.AppendLine("");

            // Add parameters
            foreach (var field in Table.Fields)
            {
                result.Append(GetFieldParameter(field));
            }

            result.AppendLine("_db.Exec(cmd);");

            result.AppendLine("");
            result.AppendLine("}");

            return result.ToString();
        }

        /// <summary>
        /// Creates a method to delete an entity from the database by its ID
        /// </summary>
        /// <returns>string with the method code</returns>
        public override string CreateDeleteEntityCommand()
        {
            var result = new StringBuilder();

            var pk = Table.Fields.FirstOrDefault(x => x.IsPrimaryKey);
            if (pk == null)
            {
                result.AppendLine("// No primary key field defined for method GetById");
                return result.ToString();
            }

            var paraName = pk.Name[..1].ToLowerInvariant() + pk.Name[1..];

            result.AppendLine($"/// <summary>\r\n/// Delete a row from table {Table.Name} \r\n/// </summary>");
            result.AppendLine($"public void Delete({pk.DatabaseType} {paraName})");
            result.AppendLine("{");
            result.AppendLine($"");
            result.AppendLine($"\tvar cmd = new NpgsqlCommand(\"DELETE FROM \\\"{Table.Name}\\\" WHERE \\\"{pk.Name}\\\" = @PK\");");
            result.AppendLine($"");
            result.AppendLine($"\tvar p = new NpgsqlParameter(\"@PK\", {GetFieldTypeFromType(pk.DatabaseType)}) {{Value = {paraName}}};");
            result.AppendLine($"\tcmd.Parameters.Add(p);");
            result.AppendLine($"");
            result.AppendLine($"\t_db.Exec(cmd);");
            result.AppendLine($"");
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
        /// <returns>string with the method code</returns>
        public override string CreateGetAllEntitiesCommand()
        {

            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Get all rows in table {Table.Name}\r\n/// </summary>");
            result.AppendLine($"public IList<{Table.DtoName}> GetAll()");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine($"var result = new List<{Table.DtoName}>();");
            result.AppendLine("");
            result.AppendLine($"var reader = _db.GetDataReader(\"SELECT * FROM \\\"{Table.Name}\\\"\"); ");
            result.AppendLine("");
            result.AppendLine("while (reader.Read())");
            result.AppendLine("{");
            result.AppendLine($"\tvar dto = DataHelper.MapFromDbTo{Table.DtoName}(reader);");
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
        /// <returns>string with the method code</returns>
        public override string CreateCountCommand()
        {
            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Count all rows in table {Table.Name} \r\n/// </summary>");
            result.AppendLine($"public int Count()");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine($"var result = _db.ExecWithResult(\"SELECT COUNT(*) FROM \\\"{Table.Name}\\\"\"); ");
            result.AppendLine("");
            result.AppendLine("return Convert.ToInt32(result);");

            result.AppendLine("}");


            return result.ToString();
        }

        public override string CreateGetByIdCommand()
        {
            var result = new StringBuilder();

            var pk = Table.Fields.FirstOrDefault(x => x.IsPrimaryKey);
            if (pk == null)
            {
                result.AppendLine("// No primary key field defined for method GetById");
                return result.ToString();
            }
            result.AppendLine($"/// <summary>\r\n/// Get all rows in table {Table.Name}\r\n/// </summary>");
            result.AppendLine($"public {Table.DtoName} GetById({pk.DatabaseType} pk{pk.Name})");
            result.AppendLine("{");

            result.AppendLine("");
            result.AppendLine($"{Table.DtoName} dto = null;");
            result.AppendLine("");
            result.AppendLine($"var reader = _db.GetDataReader($\"SELECT * FROM \\\"{Table.Name}\\\" WHERE \\\"{pk.Name}\\\"={{pk{pk.Name}}};\"); ");
            result.AppendLine("");
            result.AppendLine("while (reader.Read())");
            result.AppendLine("{");
            result.AppendLine($"\tdto = DataHelper.MapFromDbTo{Table.DtoName}(reader);");
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

    }
}