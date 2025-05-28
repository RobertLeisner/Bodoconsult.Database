// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.DataTableBackup.Formatters
{

    /// <summary>
    /// The default CSV formatter used for DataTable based backup
    /// </summary>
    public class DefaultCsvDataTableBackupFormatter : IDataTableBackupFormatter
    {

        private readonly List<string> _properties = new();
        private int _propCount;

        /// <summary>
        /// Data to format
        /// </summary>
        public DataTable Data { get; private set; }

        /// <summary>
        /// Load the data in the formatter
        /// </summary>
        /// <param name="data">Data to format</param>
        public void LoadData(DataTable data)
        {
            Data = data;
        }

        /// <summary>
        /// The the data as a formatted string
        /// </summary>
        /// <returns></returns>
        public StringBuilder GetResult()
        {
            _properties.Clear();
            foreach (DataColumn column in Data.Columns)
            {
                _properties.Add(column.ColumnName);
            }

            _propCount = Data.Columns.Count - 1;

            var result = new StringBuilder();

            AddHeader(result);

            AddContent(result);

            return result;

        }

        private void AddContent(StringBuilder result)
        {

            foreach (DataRow row in Data.Rows)
            {
                var count = 0;
                foreach (var propertyInfo in _properties)
                {
                    result.Append(row[propertyInfo]);
                    if (count < _propCount)
                    {
                        result.Append(";");
                    }

                    count++;
                }
                result.Append(Environment.NewLine);
            }
        }

        private void AddHeader(StringBuilder result)
        {
            var count = 0;
            foreach (DataColumn column in Data.Columns)
            {
                result.Append(column.ColumnName);
                if (count < _propCount)
                {
                    result.Append(";");
                }
                count++;
            }
            result.Append(Environment.NewLine);
        }
    }
}
