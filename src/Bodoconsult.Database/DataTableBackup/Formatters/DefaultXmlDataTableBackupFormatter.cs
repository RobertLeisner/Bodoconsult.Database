// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using System.Collections.Generic;
using System.Data;
using System.Text;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.DataTableBackup.Formatters;

/// <summary>
/// The default XML formatter used for DataTable based backup
/// </summary>
public class DefaultXmlDataTableBackupFormatter : IDataTableBackupFormatter
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

        AddContent(result);

        return result;

    }

    private void AddContent(StringBuilder result)
    {
        result.AppendLine("<DataRows>");
        foreach (DataRow row in Data.Rows)
        {
            result.AppendLine("   <DataRow>");
            foreach (var propertyInfo in _properties)
            {
                result.AppendLine($"      <{propertyInfo}>{row[propertyInfo]}</{propertyInfo}>");
            }
            result.AppendLine("   </DataRow>");
        }
        result.AppendLine("</DataRows>");
    }

}