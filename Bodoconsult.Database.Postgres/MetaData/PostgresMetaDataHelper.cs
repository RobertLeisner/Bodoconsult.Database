using System.Data.Common;
using System.IO;
using System.Text;
using Bodoconsult.Database.Interfaces;
using Bodoconsult.Database.Postgres.MetaData.Model;

namespace Bodoconsult.Database.Postgres.MetaData
{
    /// <summary>
    /// Helper class for building mapping objects and methods for data returned by Postgres database
    /// </summary>
    public class PostgresMetaDataHelper
    {

        //

        /// <summary>
        /// Get meta data from the access database file
        /// </summary>
        /// <returns></returns>
        public static DtoPostgresTable GetTableOrViewMetaDataFromDatabase(string connectionString, string tableOrViewName, string dtoTableOrViewName)
        {

            var erg = new DtoPostgresTable {Name = dtoTableOrViewName};


            IConnManager db = new PostgresConnManager(connectionString);

            var sql =
                "SELECT * " +
                $"FROM {tableOrViewName};";

            erg.Sql = sql;

            // Act
            var reader =db.GetDataReader(sql);

            //
            var schema = reader.GetColumnSchema();

            
            for (int i = 0; i < schema.Count; i++)
            {
                var col = schema[i];


                var colItem = new DtoPostgresField
                {
                    Name = col.ColumnName, 
                    DatabaseType = col.DataType
                };


                if (col.ColumnSize!=null) colItem.MaxLength = (int)col.ColumnSize;

                erg.Fields.Add(colItem);
            }


            return erg;
        }


        public static string CreateDto(DtoPostgresTable table, string targetPath, string myNamespace="Test.Model")
        {
            var fileName = Path.Combine(targetPath, $"{table.DtoName}.txt");

            var result = new StringBuilder();

            result.AppendLine("using System;");
            result.AppendLine("using System.ComponentModel.DataAnnotations;");

            result.AppendLine("");
            result.AppendLine($"namespace {myNamespace}");
            result.AppendLine("{");
            result.AppendLine("");

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
            result.AppendLine("");
            result.AppendLine("}");

            File.WriteAllText(fileName, result.ToString());

            return fileName;
        }


        /// <summary>
        /// Get a C# property for a database field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetFieldProperty(DtoPostgresField field)
        {

            var type = field.DatabaseType;

            var attr = type == typeof(string) ? $"[MaxLength({field.MaxLength}, ErrorMessage = \"Maximum number of characters that can be entered for {field.Name} is {field.MaxLength}!\")]\r\n" : "";


            var comment = type == typeof(string) ? $" Maximum length: {field.MaxLength} chars" : "";

            comment = $"/// <summary>\r\n/// {field.Name} {comment}\r\n/// </summary>\r\n";

            var fieldName = field.Name[..1].ToUpper() +
                            field.Name[1..];


            var result = $"{comment}{attr}public {type.Name} {fieldName} {{ get; set; }}";


            return result;

        }

        public static string CreateMappingFromDbToDto(DtoPostgresTable table, string targetPath)
        {
            var fileName = Path.Combine(targetPath, $"MappingFromDbTo{table.DtoName}.txt");

            var result = new StringBuilder();

            result.AppendLine($"/// <summary>\r\n/// Mapping datareader from {table.Name} to DTO class {table.DtoName}\r\n/// </summary>");

            result.AppendLine($"public static {table.DtoName} MapFromDbTo{table.DtoName}(IDataReader reader)");
            result.AppendLine("{");
            result.AppendLine("");

            result.AppendLine($"var dto = new {table.DtoName}();");

            for (var index = 0; index < table.Fields.Count; index++)
            {
                var field = table.Fields[index];
                result.AppendLine(GetFieldMappingFromDb(field, index));
            }

            result.AppendLine("");
            result.AppendLine("return dto;");


            result.AppendLine("");
            result.AppendLine("}");
            //result.AppendLine("");
            //result.AppendLine("}");

            File.WriteAllText(fileName, result.ToString());

            return fileName;
        }


        private static string GetFieldMappingFromDb(DtoPostgresField field, int index)
        {
            //IDataReader reader;

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
                    result = $"dto.{fieldName}=reader.GetDateTime({index});";
                    break;
                case "string":
                    result = $"dto.{fieldName}=reader[{index}].ToString();";
                    break;
                default:
                    break;
            }


            return result;


        }
    }
}