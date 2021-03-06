﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Olive.Csv
{
    public static class CsvWriter
    {
        /// <summary>
        /// Saves csv content into a file
        /// </summary>
        /// <param name="csv">Csv string</param>
        /// <param name="path">File save path</param>
        public static void Save(this string csv, FileInfo path) => File.WriteAllText(path.FullName, csv);

        /// <summary>
        /// Converts a Dictionary object to CSV string
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns>CSV string content</returns>
        public static string ToCsv(this Dictionary<string, string> dictionary)
        {
            var CSV = new StringBuilder();
            foreach (var pair in dictionary)
                CSV.AppendLine(string.Format("{0},{1}", pair.Key, pair.Value));

            return CSV.ToString();
        }

        /// <summary>
        /// Converts an IEnumerable object to CSV string
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable</typeparam>
        /// <param name="items"></param>
        /// <returns>CSV string content</returns>
        public static string ToCsv<T>(this IEnumerable<T> items)
            where T : class
        {
            var csvBuilder = new StringBuilder();
            var properties = typeof(T).GetProperties();

            foreach (var item in items)
            {
                var line = string.Join(",", properties.Select(p => p.GetValue(item, null).ToCsvValue()).ToArray());
                csvBuilder.AppendLine(line);
            }

            return csvBuilder.ToString();
        }

        static string ToCsvValue<T>(this T item)
        {
            if (item == null) return "\"\"";

            if (item is string)
                return string.Format("\"{0}\"", item.ToString().Replace("\"", "\\\""));

            if (double.TryParse(item.ToString(), out var dummy))
                return string.Format("{0}", item);

            return string.Format("\"{0}\"", item);
        }

        /// <summary>
        /// Converts a DataTable object to CSV string
        /// </summary>
        /// <param name="dtDataTable"></param>
        /// <returns>CSV string content</returns>
        public static string ToCsv(this DataTable dtDataTable)
        {
            var sb = new StringBuilder();
            // headers
            for (var i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sb.Append(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1) sb.Append(",");
            }

            sb.AppendLine();
            foreach (var dr in dtDataTable.GetRows())
            {
                for (var i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        var value = dr[i].ToString();
                        if (value.Contains(',')) sb.Append(string.Format("\"{0}\"", value));
                        else sb.Append(dr[i].ToString());
                    }

                    if (i < dtDataTable.Columns.Count - 1) sb.Append(",");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}