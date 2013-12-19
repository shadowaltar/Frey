using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Automata.Core.Utils
{
    public static class DataExtensions
    {
        /// <summary>
        /// Parse a given object into integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this object value)
        {
            return Convert.ToInt32(value);
        }

        /// <summary>
        /// Parse a given object into boolean. If the string parsed by the object equals to "1",
        /// it will also return true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool(this object value)
        {
            return Convert.ToBoolean(value) || value.Trim() == "1";
        }

        /// <summary>
        /// Parse a given object into long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToLong(this object value)
        {
            return Convert.ToInt64(value);
        }

        /// <summary>
        /// Parse a given object into a decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object value)
        {
            return Convert.ToDecimal(value);
        }

        /// <summary>
        /// Parse a given object into double.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(this object value)
        {
            return Convert.ToDouble(value);
        }

        /// <summary>
        /// Parse a given object into DateTime. If the object is of type DateTime it will
        /// be casted directly, else the object will be parsed into a string and then parsed
        /// by <see cref="DateTime.Parse()"/> again.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static DateTime ToDateTime(this object value)
        {
            return value is DateTime ? (DateTime)value : DateTime.Parse(value.ToString());
        }

        /// <summary>
        /// Return true if the value is DBNull by given the row and the column key string.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        public static bool IsDBNull(this DataRow row, string columnKey)
        {
            row.ThrowIfNull();
            columnKey.ThrowIfNull();
            var cell = row[columnKey];
            cell.ThrowIfNull();
            return cell == DBNull.Value;
        }

        /// <summary>
        /// Return true if the value is null or DBNull.
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrDBNull(this object value)
        {
            return value == null || value == DBNull.Value;
        }

        /// <summary>
        /// Return true if the value is DBNull, or it is a decimal zero after conversion,
        /// by given the row and the column key string.
        /// If the value is not a decimal, conversion exception will be thrown.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        public static bool IsDBNullOrZero(this DataRow row, string columnKey)
        {
            row.ThrowIfNull();
            columnKey.ThrowIfNull();
            var cell = row[columnKey];
            cell.ThrowIfNull();
            return cell == DBNull.Value || Convert.ToDecimal(cell) == 0;
        }

        /// <summary>
        /// Return true if the value is null, DBNull, or it is a decimal zero after conversion,
        /// by given the row and the column key string.
        /// If the value is not a decimal, conversion exception will be thrown.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        public static bool IsNullOrDBNullOrZero(this DataRow row, string columnKey)
        {
            row.ThrowIfNull();
            columnKey.ThrowIfNull();
            var cell = row[columnKey];
            return IsNullOrDBNullOrZero(cell);
        }

        /// <summary>
        /// Return true if the value is null, DBNull, or it is a decimal zero after conversion.
        /// If the value is not a decimal, conversion exception will be thrown.
        /// </summary>
        /// <returns></returns>
        public static bool IsNullOrDBNullOrZero(this object value)
        {
            return value == null || value == DBNull.Value || Convert.ToDecimal(value) == 0;
        }

        /// <summary>
        /// Trim a given object's string form; if null, or DBNull, return <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimOrDefaultIfDBNull(this object value)
        {
            return value == null || value == DBNull.Value
                ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Parse a given object into a decimal; if DBNull, return zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal ToDecimalOrZeroIfDBNull(this object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToDecimal(value);
        }

        /// <summary>
        /// Parse a given object's int form; if DBNull, return zero.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToIntOrZeroIfDBNull(this object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        /// <summary>
        /// Get the row of the table specified by the row's primary key value.
        /// Identical to <see cref="DataRowCollection.Find(object)"/>.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DataRow Find(this DataTable table, object key)
        {
            table.ThrowIfNull();
            key.ThrowIfNull();
            return table.Rows.Find(key);
        }

        /// <summary>
        /// Get the row of the table specified by the row's primary key value.
        /// Identical to <see cref="DataRowCollection.Find(object)"/>.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static DataRow Find(this DataTable table, object[] keys)
        {
            table.ThrowIfNull();
            keys.ThrowIfNull();
            return table.Rows.Find(keys);
        }

        /// <summary>
        /// Same as <see cref="DataTable.Select()"/> to select rows matching the select statement.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="formattedSelect"></param>
        /// <param name="selectArgs"></param>
        /// <returns></returns>
        public static DataRow[] SelectFormat(this DataTable table, string formattedSelect, params object[] selectArgs)
        {
            table.ThrowIfNull();
            formattedSelect.ThrowIfNull();
            return table.Select(string.Format(formattedSelect, selectArgs));
        }

        /// <summary>
        /// Same as <see cref="DataTable.Select()"/> to select rows which their column with name
        /// <paramref name="columnName"/> equals to <paramref name="valueToCompare"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public static DataRow[] SelectEquals<T>(this DataTable table, string columnName, T valueToCompare)
        {
            table.ThrowIfNull();
            columnName.ThrowIfNullOrEmpty();

            var tType = typeof(T);
            if (tType == typeof(decimal) || tType == typeof(int) || tType == typeof(short)
                || tType == typeof(double) || tType == typeof(long) || tType == typeof(float))
            {
                return table.SelectFormat("[{0}] = {1}", columnName, valueToCompare);
            }

            return table.SelectFormat("[{0}] = '{1}'", columnName, valueToCompare.ToString());
        }

        /// <summary>
        /// Same as <see cref="DataTable.Select()"/> to select rows which their column with name
        /// <paramref name="columnName"/> does not equal to <paramref name="valueToCompare"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public static DataRow[] SelectNotEquals<T>(this DataTable table, string columnName, T valueToCompare)
        {
            table.ThrowIfNull();
            columnName.ThrowIfNullOrEmpty();

            var tType = typeof(T);
            if (tType == typeof(decimal) || tType == typeof(int) || tType == typeof(short)
                || tType == typeof(double) || tType == typeof(long) || tType == typeof(float))
            {
                return table.SelectFormat("[{0}] <> {1}", columnName, valueToCompare);
            }

            return table.SelectFormat("[{0}] <> '{1}'", columnName, valueToCompare.ToString());
        }

        /// <summary>
        /// Delete rows from a table by given a SELECT filter string.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static DataTable Delete(this DataTable table, string filter)
        {
            table.ThrowIfNull();
            filter.ThrowIfNull();

            table.Select(filter).Delete();
            return table;
        }

        /// <summary>
        /// Delete rows from a table by given a SELECT filter as a formatted string.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="formattedFilter"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DataTable Delete(this DataTable table, string formattedFilter, params object[] args)
        {
            table.ThrowIfNull();
            formattedFilter.ThrowIfNull();

            table.SelectFormat(formattedFilter, args).Delete();
            return table;
        }

        /// <summary>
        /// Delete rows.
        /// </summary>
        /// <param name="rows"></param>
        public static void Delete(this IEnumerable<DataRow> rows)
        {
            rows.ThrowIfNull();
            foreach (var row in rows)
                row.Delete();
        }

        /// <summary>
        /// Check if the table has no row.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpty(this DataTable source)
        {
            source.ThrowIfNull();
            return source.Rows.Count == 0;
        }

        public static bool IsNullOrEmpty(this DataTable table)
        {
            return table == null || table.Rows.Count == 0;
        }

        public static bool IsNullOrEmpty(this DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
                return true;

            foreach (DataTable table in dataSet.Tables)
            {
                if (table.IsNullOrEmpty())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Duplicate a <see cref="DataTable"/> which the source and the copied result are
        /// in difference reference values; optional allows to
        /// trim spaces for all the cell values which types are string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="trimText"></param>
        /// <returns></returns>
        public static DataTable Duplicate(this DataTable source, bool trimText)
        {
            if (source == null) return null;

            var result = source.Clone();
            result.TableName = source.TableName;
            result.BeginLoadData();
            foreach (DataRow row in source.Rows)
            {
                var r = result.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    if (trimText && source.Columns[i].DataType == typeof(string)) // if trim, and is text col
                    {
                        r[i] = row[i].ToString().Trim();
                    }
                    else
                    {
                        r[i] = row[i];
                    }
                }
                result.Rows.Add(r);
            }
            return result;
        }

        /// <summary>
        /// Duplicate a <see cref="DataTable"/> which the source and the copied result are
        /// in difference reference values.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable Duplicate(this DataTable source)
        {
            return Duplicate(source, false);
        }

        /// <summary>
        /// Trim all the text in those cells which types are string.
        /// The return object is exactly the same instance of the DataTable in parameter list.
        /// </summary>
        /// <param name="table"></param>
        public static DataTable TrimText(this DataTable table)
        {
            table.ThrowIfNull();
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType != typeof(string))
                    continue;

                foreach (DataRow row in table.Rows)
                    row[column] = row[column].ToString().Trim();
            }
            return table;
        }

        /// <summary>
        /// Find the first row which meets the predicate criteria in a data table; or return null if not found.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static DataRow FirstOrDefault(this DataTable table, Func<DataRow, bool> predicate)
        {
            table.ThrowIfNull();
            if (table.Rows.Count == 0)
                return null;
            predicate.ThrowIfNull("predicate", "You must include a predicate.");
            return table.Rows.OfType<DataRow>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// Select a single column's values into an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static IEnumerable<T> Select<T>(this DataTable table, string columnName)
        {
            table.ThrowIfNull();
            columnName.ThrowIfNullOrEmpty("columnName", "You must provide a non-empty column name string.");
            if (table.Rows.Count == 0)
                return null;
            return table.Rows.OfType<DataRow>().Select(r => r[columnName]).OfType<T>();
        }

        /// <summary>
        /// Linq version of <see cref="DataTable.Select()"/> which is faster.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<DataRow> Where(this DataTable table, Func<DataRow, bool> predicate)
        {
            table.ThrowIfNull();
            if (table.Rows.Count == 0)
                return null;
            predicate.ThrowIfNull("predicate", "You must include a predicate.");
            return table.Rows.OfType<DataRow>().Where(predicate);
        }

        /// <summary>
        /// Get values from the rows obtained by Linq version of <see cref="DataTable.Select()"/>
        /// which is faster.
        /// It will cast results to <typeparam name="T" />.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this DataTable table, string columnName, Func<DataRow, bool> predicate)
        {
            var rows = table.Where(predicate);
            if (rows == null)
                return null;
            columnName.ThrowIfNullOrEmpty("columnName", "You must provide a non-empty column name string.");
            return rows.Select(dataRow => dataRow["columnName"]).Cast<T>();
        }

        /// <summary>
        /// Fill all cells in a column with the provided text. Optionally can set to only filling
        /// those empty cells.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnKey"></param>
        /// <param name="text"></param>
        /// <param name="onlyIfEmpty"></param>
        public static void FillText(this DataTable table, string columnKey, string text, bool onlyIfEmpty)
        {
            table.ThrowIfNull();
            columnKey.ThrowIfNullOrEmpty();
            if (table.Columns.Contains(columnKey))
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!onlyIfEmpty || string.IsNullOrEmpty(row[columnKey].ToString()))
                    {
                        row[columnKey] = text;
                    }
                }
            }
        }

        /// <summary>
        /// Replace cell values in a column: if the original cell value equals to a dictionary key,
        /// replace it by the dictionary entry's value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnKey"></param>
        /// <param name="mapping"></param>
        public static void MapText<T>(this DataTable table, string columnKey, Dictionary<T, string> mapping)
        {
            table.ThrowIfNull();
            columnKey.ThrowIfNullOrEmpty();
            mapping.ThrowIfNull();
            if (table.Columns.Contains(columnKey))
            {
                foreach (DataRow row in table.Rows)
                {
                    var source = row[columnKey].ToString();
                    var result = mapping.ContainsValue(source)
                        ? mapping.FirstOrDefault(p => p.Value == source).Key : default(T);
                    row[columnKey] = result.ToString();
                }
            }
        }

        /// <summary> 
        /// Replace cell values in a column: if the original cell value equals to <paramref name="target"/>,
        /// replace it by the <paramref name="replacement"/>.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnKey"></param>
        /// <param name="target"></param>
        /// <param name="replacement"></param>
        public static void ReplaceText(this DataTable table, string columnKey, string target, string replacement)
        {
            table.ThrowIfNull();
            columnKey.ThrowIfNullOrEmpty();
            if (table.Columns.Contains(columnKey))
            {
                foreach (DataRow row in table.Rows)
                {
                    var source = row[columnKey].ToString();
                    if (source.Equals(target))
                    {
                        row[columnKey] = replacement;
                    }
                }
            }
        }

        /// <summary>
        /// Add a list of columns by given their names to a DataTable.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnNames"></param>
        public static void AddColumns(this DataTable table, params string[] columnNames)
        {
            table.ThrowIfNull();
            columnNames.ThrowIfNullOrEmpty();

            foreach (var columnName in columnNames)
            {
                table.Columns.Add(columnName);
            }
        }

        /// <summary>
        /// Get the count of values in a column, while not counting duplicated ones.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnKey"></param>
        /// <returns></returns>
        public static int CountColumnValue(this DataTable table, string columnKey)
        {
            table.ThrowIfNull("table", "Must provide a datatable to count.");
            columnKey.ThrowIfNullOrEmpty("columnKey", "Must provide a columen key to count the table column value.");
            if (!table.Columns.Contains(columnKey))
            {
                throw new ArgumentException("The table does not contain the column key provided: " + columnKey);
            }
            return table.AsEnumerable().Select(r => r[columnKey]).Distinct().Count();
        }

        /// <summary>
        /// Convert a list of data into a single column table. Returns null if the list is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(this IEnumerable<T> list, string columnName)
        {
            if (list == null)
                return null;
            columnName.ThrowIfNullOrEmpty();

            var result = new DataTable();
            result.Columns.Add(columnName, typeof(T));

            if (list.Count() == 0)
                return result;

            foreach (var item in list)
            {
                var row = result.NewRow();
                row[columnName] = item;
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
