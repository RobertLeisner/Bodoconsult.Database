// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Bodoconsult.Database.Interfaces;

namespace Bodoconsult.Database.EntityBackup.Formatters;

/// <summary>
/// The default XML formatter used for entity backup
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultXmlEntityBackupFormatter<T> : IEntityBackupFormatter<T> where T : class
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
        var ser = new XmlSerializer(Data.GetType());

        var result = new StringBuilder();

        using (var textWriter = new StringWriter())
        {
            ser.Serialize(textWriter, Data);
            result.Append(textWriter);
        }

        return result;
    }
}