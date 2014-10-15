using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Utils
{
    public static class Extensions
    {
        public static IEnumerable<T> Values<T>(this Type x, params T[] excepts)
        {
            return Enum.GetValues(x).Cast<T>().Except(excepts);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        public static T Random<T>(this IList<T> items) where T : class
        {
            if (items.Count == 0)
                return null;
            var i = StaticRandom.Instance.Next(items.Count);
            return items[i];
        }

        public static T Random<T>(this ISet<T> items) where T : class
        {
            if (items.Count == 0)
                return null;
            var i = StaticRandom.Instance.Next(items.Count);
            return items.ElementAtOrDefault(i);
        }
    }
}