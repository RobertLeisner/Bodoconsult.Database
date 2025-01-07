// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using Bodoconsult.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bodoconsult.Database.EntityBackup.Formatters
{

    /// <summary>
    /// The default CSV formatter used for entity backup
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultCsvEntityBackupFormatter<T> : IEntityBackupFormatter<T> where T : class
    {

        private PropertyInfo[] _properties;
        private int _propCount;

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
            _properties = typeof(T).GetProperties();
            _propCount = _properties.Length - 1;

            var result = new StringBuilder();

            AddHeader(result);

            AddContent(result);

            return result;

        }

        private void AddContent(StringBuilder result)
        {
            

            foreach (var item in Data)
            {
                var count = 0;
                foreach (var propertyInfo in _properties)
                {
                    result.Append(propertyInfo.GetValue(item));
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
            foreach (var propertyInfo in _properties)
            {
                result.Append(propertyInfo.Name);
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
