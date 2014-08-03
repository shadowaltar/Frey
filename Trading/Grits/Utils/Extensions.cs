using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace GritsMaintenance.Utils
{
    public static class Extensions
    {
        public static void AddIfNotExist<T>(this IList<T> list, T item)
        {
            if (list.Contains(item))
                return;
            list.Add(item);
        }

        public static StringBuilder AppendTab(this StringBuilder builder, string value = "")
        {
            return builder.Append(value).Append('\t');
        }

        public static StringBuilder AppendTab(this StringBuilder builder, object value)
        {
            return builder.Append(value).Append('\t');
        }

        public static int CompareToIgnoreCase(this string x, string y)
        {
            return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public static void ClearAndAddRange<T>(this BindableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            collection.AddRange(items);
        }

        public static void ClearAndAddRange<T>(this List<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            collection.AddRange(items);
        }

        public static int ConvertInteger(this object value, int defaultValue = 0)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            return Convert.ToInt32(value);
        }

        public static long ConvertLong(this object value, long defaultValue = 0L)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            return Convert.ToInt64(value);
        }

        public static string ConvertString(this object value, string defaultValue = "")
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;
            return value.ToString();
        }

        public static T ParseEnum<T>(this object value)
        {
            return (T)Enum.Parse(typeof(T), value.ConvertString());
        }

        public static T ParseEnum<T>(this object value, Func<string, T> convertingFunction)
        {
            return convertingFunction(value.ConvertString().Trim());
        }
    }
}