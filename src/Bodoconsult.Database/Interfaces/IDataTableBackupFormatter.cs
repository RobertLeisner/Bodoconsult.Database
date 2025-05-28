// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Data;
using System.Text;

namespace Bodoconsult.Database.Interfaces;

/// <summary>
/// Interface for string creation from given data for the backup file created by <see cref="IDataTableBackupDataService"/>
/// </summary>
public interface IDataTableBackupFormatter
{
    /// <summary>
    /// Data to format
    /// </summary>
    DataTable Data { get; }

    /// <summary>
    /// Load the data in the formatter
    /// </summary>
    /// <param name="data">Data to format</param>
    void LoadData(DataTable data);

    /// <summary>
    /// The the data as a formatted string
    /// </summary>
    /// <returns></returns>
    StringBuilder GetResult();
}