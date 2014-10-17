using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;

namespace Trading.Common.Utils
{
    public static class Extensions
    {
        public static void AddIfNotExist<T>(this IList<T> list, T item)
        {
            if (list.Contains(item))
                return;
            list.Add(item);
        }

        /// <summary>
        /// Format a date by "yyyy-MM-dd".
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string IsoFormat(this DateTime date)
        {
            return date.ToString(Constants.IsoFormatDate);
        }

        /// <summary>
        /// Format a date by "yyyy-MM-dd HH:mm:ss".
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string IsoFormatDateTime(this DateTime date)
        {
            return date.ToString(Constants.IsoFormatDateTime);
        }

        /// <summary>
        /// Format a date by "yyyy-MM-dd HH:mm:ss".
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string IsoFormatTimestamp(this DateTime date)
        {
            return date.ToString(Constants.IsoFormatTimestamp);
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

        public static bool EqualsIgnoreCase(this string x, string y)
        {
            return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string value, string substring)
        {
            return value.IndexOf(substring, StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        public static void AddRange<T, T2>(this Dictionary<T, T2> target, Dictionary<T, T2> source)
        {
            if (target != null && source != null)
            {
                foreach (var pair in source)
                {
                    target[pair.Key] = source[pair.Key];
                }
            }
        }

        public static void AddRange<T>(this IList<T> target, params T[] items)
        {
            foreach (var item in items)
            {
                target.Add(item);
            }
        }

        public static void AddRange<T>(this ISet<T> target, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                target.Add(item);
            }
        }

        public static void ClearAndAddRange<T>(this BindableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            collection.AddRange(items);
        }

        public static void ClearAndAddRange<T, T2>(this Dictionary<T, T2> target, Dictionary<T, T2> source)
        {
            if (target != null && source != null)
            {
                target.Clear();
                target.AddRange(source);
            }
        }

        public static void ClearAndAddRange<T>(this List<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            collection.AddRange(items);
        }

        public static bool MoveTo<T>(this List<T> collection, T item, int position)
        {
            collection.ThrowIfNull("collection", "Must provide a collection.");
            item.ThrowIfNull("item", "Must provide an item to be moved.");
            if (!collection.Remove(item))
                return false;
            collection.Insert(position, item);
            return true;
        }

        public static bool MoveToEnd<T>(this List<T> collection, T item)
        {
            collection.ThrowIfNull("collection", "Must provide a collection.");
            item.ThrowIfNull("item", "Must provide an item to be moved.");
            if (!collection.Remove(item))
                return false;
            collection.Insert(collection.Count, item);
            return true;
        }

        public static bool IsNullOrDBNull(this object value)
        {
            return value == null || value == DBNull.Value;
        }

        public static bool IsNullOrEmpty<T>(this IList<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool IsNullOrEmpty(this DataTable table)
        {
            return table == null || table.Rows.Count == 0;
        }

        public static int ConvertInt(this object value, int defaultValue = 0)
        {
            return value.IsNullOrDBNull() ? defaultValue : Convert.ToInt32(value);
        }

        public static long ConvertLong(this object value, long defaultValue = 0L)
        {
            return value.IsNullOrDBNull() ? defaultValue : Convert.ToInt64(value);
        }

        public static decimal ConvertDecimal(this object value, decimal defaultValue = 0M)
        {
            return value.IsNullOrDBNull() ? defaultValue : Convert.ToDecimal(value);
        }

        public static double ConvertDouble(this object value, double defaultValue = 0d)
        {
            return value.IsNullOrDBNull() ? defaultValue : Convert.ToDouble(value);
        }

        public static bool? ConvertBoolean(this object value)
        {
            bool result;
            if (bool.TryParse(value.ToString(), out result))
            {
                return result;
            }
            return null;
        }

        public static bool ConvertBoolean(this object value, bool defaultValue)
        {
            bool result;
            if (bool.TryParse(value.ToString(), out result))
            {
                return result;
            }
            return defaultValue;
        }

        public static string ConvertString(this object value, string defaultValue = "")
        {
            return value.IsNullOrDBNull() ? defaultValue : value.ToString();
        }

        public static string Trim(this object value)
        {
            return value.ToString().Trim();
        }

        public static string TrimOrDefault(this string value, string defaultValue = null)
        {
            return value == null ? defaultValue : value.Trim();
        }

        public static DateTime ConvertDate(this object value)
        {
            return value.IsNullOrDBNull() ? DateTime.MinValue : Convert.ToDateTime(value);
        }

        public static DateTime ConvertDate(this object value, DateTime defaultValue)
        {
            return value.IsNullOrDBNull() ? defaultValue : Convert.ToDateTime(value);
        }

        public static DateTime ConvertDate(this object value, string format)
        {
            return value.IsNullOrDBNull() ? DateTime.MinValue
                : DateTime.ParseExact(value.ToString(), format, CultureInfo.InvariantCulture);
        }

        public static DateTime ConvertDate(this object value, string format, DateTime defaultValue)
        {
            return value.IsNullOrDBNull() ? defaultValue :
                DateTime.ParseExact(value.ToString(), format, CultureInfo.InvariantCulture);
        }

        public static DateTime ConvertIsoDate(this object value)
        {
            return value.IsNullOrDBNull() ? DateTime.MinValue :
                DateTime.ParseExact(value.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static DateTime ConvertIsoDateTime(this object value)
        {
            return value.IsNullOrDBNull() ? DateTime.MinValue :
                DateTime.ParseExact(value.ToString(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static T ParseEnum<T>(this object value)
        {
            return (T)Enum.Parse(typeof(T), value.ConvertString());
        }

        /// <summary>
        /// Convert the input object to a string (trimmed), then process the string by the
        /// <see cref="convertingFunction"/>. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="convertingFunction"></param>
        /// <returns></returns>
        public static T Process<T>(this object value, Func<string, T> convertingFunction)
        {
            return convertingFunction(value.ConvertString().Trim());
        }

        public static bool StartsWithIgnoreCase(this string value, string header)
        {
            return value.StartsWith(header, true, CultureInfo.InvariantCulture);
        }

        public static IList<T> Flatten<T>(this IEnumerable<T> hierarchy, Func<T, IEnumerable<T>> lambda)
        {
            var result = new List<T>();

            foreach (T item in hierarchy)
            {
                result.AddRange(Flatten(lambda(item), lambda));
                if (!result.Contains(item))
                    result.Add(item);
            }

            return result;
        }

        public static void ThrowIfNull(this object value, string parameterName, string message)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName, message);
        }

        public static void ThrowIfNullOrWhiteSpace(this string value, string parameterName, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(parameterName, message);
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(this DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public async static Task<T> TimeoutAfter<T>(this Task<T> task, int delay)
        {
            await Task.WhenAny(task, Task.Delay(delay));

            if (!task.IsCompleted)
                throw new TimeoutException("Timeout (ms) hit: " + delay);

            return await task;
        }

        public async static Task TimeoutAfter(this Task task, int delay)
        {
            await Task.WhenAny(task, Task.Delay(delay));

            if (!task.IsCompleted)
                throw new TimeoutException("Timeout (ms) hit: " + delay);
        }

        /// <summary>
        /// Get all the elements of a tree (flatten the tree), by providing the path (property)
        /// of the child node collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hierarchy"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static IList<T> Flatten<T>(this IList<T> hierarchy, Func<T, IList<T>> lambda)
        {
            var result = new List<T>();

            foreach (var item in hierarchy)
            {
                result.AddRange(Flatten(lambda(item), lambda));
                if (!result.Contains(item))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Check if any duplication of elements in the given collection <see cref="collection"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns>True if there is at least one duplication.</returns>
        public static bool ContainsDuplicates<T>(this IEnumerable<T> collection)
        {
            var set = new HashSet<T>();
            foreach (var item in collection)
            {
                if (!set.Add(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if any duplication of a property (by selector <see cref="selectFunction"/>)
        /// in the given collection <see cref="collection"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selectFunction"></param>
        /// <returns>True if there is at least one duplication.</returns>
        public static bool ContainsDuplicates<T, TE>(this IEnumerable<T> collection, Func<T, TE> selectFunction)
        {
            var set = new HashSet<TE>();
            foreach (var item in collection.Select(selectFunction))
            {
                if (!set.Add(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Surround the string by single quotes.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string QuoteMe(this string target)
        {
            return "'" + target + "'";
        }

        /// <summary>
        /// Output false if 0, true if 1.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool From01(this int value)
        {
            return value == 1;
        }

        /// <summary>
        /// Output 0 if false, 1 if true.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static int To01(this bool target)
        {
            return target ? 1 : 0;
        }

        /// <summary>
        /// Concatenate a list of strings, with arbitrary string <see cref="delimiter"/> separating
        /// them.
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string ConcatWithDelimiter(this IEnumerable<string> strings, string delimiter)
        {
            var sb = new StringBuilder();

            foreach (var str in strings)
            {
                sb.Append(str).Append(delimiter);
            }

            // in case no strings provided at all
            if (sb.Length == 0)
                return "";

            return sb.Remove(sb.Length - delimiter.Length, delimiter.Length).ToString();
        }

        /// <summary>
        /// Check if a string only contains alphanumeric characters.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool AreLettersOrDigits(this string value)
        {
            return value.All(char.IsLetterOrDigit);
        }

        /// <summary>
        /// Check if a string only contains alphanumeric characters or those
        /// listed in <see cref="chars"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static bool AreLettersOrDigitsOr(this string value, params char[] chars)
        {
            return value.All(c => char.IsLetterOrDigit(c) || (chars != null && chars.Contains(c)));
        }
    }
}