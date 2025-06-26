// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System;
using System.Globalization;

namespace Bodoconsult.Database.Dbase.Helpers
{
    public static class ParseHelper
    {

        private const string Format = "yyyyMMdd";

        #region Public methods

        public static bool? ParseBool(string columnValue, bool returnNull = false)
        {
            if (string.IsNullOrWhiteSpace(columnValue))
            {
                return returnNull ? (bool?)null : false;
            }

            if (bool.TryParse(columnValue.ToLower(), out var result))
            {
                return result;
            }

            return returnNull ? (bool?)null : false;
        }

        public static double? ParseDouble(string columnValue, bool returnNull = false)
        {
            if (string.IsNullOrWhiteSpace(columnValue))
            {
                return returnNull ? (double?)null : 0;
            }

            if (double.TryParse(columnValue.ToLower(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return returnNull ? (double?)null : 0;
        }

        public static float? ParseFloat(string columnValue, bool returnNull = false)
        {
            if (string.IsNullOrWhiteSpace(columnValue))
            {
                return returnNull ? (float?)null : 0;
            }

            return float.TryParse(columnValue.ToLower(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : returnNull ? (float?)null : 0;
        }

        public static int? ParseInt(string columnValue, bool returnNull = false)
        {
            if (string.IsNullOrWhiteSpace(columnValue))
            {
                return returnNull ? (int?)null : 0;
            }

            return int.TryParse(columnValue.ToLower(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : returnNull ? (int?)null : 0;
        }

        public static DateTime? ParseNullableDate(string datestring)
        {
            if (string.IsNullOrEmpty(datestring))
            {
                return null;
            }

            var success = DateTime.TryParseExact(datestring.Trim(' '), Format, CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal, out var result);

            return success ? (DateTime?)result : null;
        }

        private static DateTime? ParseNullableTime(string timestring)
        {
            //const string format = "HH:mm:ss";
            //var success = DateTime.TryParseExact(timestring, format, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out var result);

            if (string.IsNullOrEmpty(timestring))
            {
                return null;
            }

            var success = DateTime.TryParse(timestring.Trim(' '), out var result);

            return success ?
                (DateTime?)result :
                null;
        }

        public static DateTime? ParseNullableDateTime(string dateString, string timeString)
        {

            if (string.IsNullOrEmpty(dateString) ||
                string.IsNullOrEmpty(timeString))
            {
                return null;
            }

            var date = ParseNullableDate(dateString);
            var time = ParseNullableTime(timeString);

            return date?.Date + time?.TimeOfDay;
        }

        public static string ProcessString(string columnValue)
        {

            // return string.IsNullOrWhiteSpace(columnValue) ? string.Empty : columnValue.TrimEnd(' ');
            return string.IsNullOrWhiteSpace(columnValue) ?
                null :
                columnValue.TrimEnd(' ');
        }

        #endregion Protected Methods

    }
}
