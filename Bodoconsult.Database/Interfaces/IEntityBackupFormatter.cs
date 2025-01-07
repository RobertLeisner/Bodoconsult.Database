// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Collections.Generic;
using System.Text;

namespace Bodoconsult.Database.Interfaces;

/// <summary>
/// Interface for string creation from given data for the backup file created by <see cref="IEntityBackupService&lt;T&gt;"/>
/// </summary>
/// <typeparam name="T">Type to backup</typeparam>
public interface IEntityBackupFormatter<T> where T : class
{
    /// <summary>
    /// Data to format
    /// </summary>
    IList<T> Data { get; }

    /// <summary>
    /// Load the data in the formatter
    /// </summary>
    /// <param name="data">Data to format</param>
    void LoadData(IList<T> data);

    /// <summary>
    /// The the data as a formatted string
    /// </summary>
    /// <returns></returns>
    StringBuilder GetResult();
}