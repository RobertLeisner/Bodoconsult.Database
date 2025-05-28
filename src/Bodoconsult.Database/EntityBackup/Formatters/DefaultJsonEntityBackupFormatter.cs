// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.EntityBackup.Formatters;

/// <summary>
/// The default Json formatter used for entity backup
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultJsonEntityBackupFormatter<T> : IEntityBackupFormatter<T> where T : class
{
    /// <summary>
    /// Data to format
    /// </summary>
    public IList<T> Data { get; private set; }

    /// <summary>
    /// Load the data in the formatter
    /// </summary>
    /// <param name="data">Data to format</param>
    public void LoadData(IList<T> data)
    {
        Data = data;
    }

    /// <summary>
    /// The the data as a formatted string
    /// </summary>
    /// <returns></returns>
    public StringBuilder GetResult()
    {
        var jsonString = JsonSerializer.Serialize(Data);
        return new StringBuilder(jsonString);
    }
}