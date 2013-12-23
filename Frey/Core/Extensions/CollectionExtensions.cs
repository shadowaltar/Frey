using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        /// <summary>
        /// Add an array of values to the end of <see cref="ICollection{T}"/>.
        /// The method does not check null for the array elements; it is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> collection, params T[] values)
        {
            collection.ThrowIfNull();
            foreach (var value in values)
                collection.Add(value);
        }

        /// <summary>
        /// Add an <see cref="IEnumerable{T}"/> of values to the end of <see cref="ICollection{T}"/>.
        /// The method does not check null for the array elements; it is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="values"></param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            collection.ThrowIfNull();
            foreach (var value in values)
                collection.Add(value);
        }

        /// <summary>
        /// Add the source dictionary contents into target dictionary. It replaces any target's values
        /// if the same keys exist in the source.
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void AddWithReplace<TK, TV>(this IDictionary<TK, TV> target, IDictionary<TK, TV> source)
        {
            target.ThrowIfNull();
            source.ThrowIfNull();
            foreach (var pair in source)
            {
                target[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Add the source dictionary contents into target dictionary. It skips any target's values
        /// if the same keys exist in the source.
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public static void AddWithoutReplace<TK, TV>(this IDictionary<TK, TV> target, IDictionary<TK, TV> source)
        {
            target.ThrowIfNull();
            source.ThrowIfNull();
            foreach (var pair in source)
            {
                if (!target.ContainsKey(pair.Key))
                    target[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Add a value to the <see cref="IList{T}"/> if the list does not contain the value.
        /// The method does not check null for either the list or the value; it is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void AddIfNotExist<T>(this IList<T> list, T value)
        {
            // do not check null for either the list or the value; shall be check outside!
            if (!list.Contains(value))
                list.Add(value);
        }

        /// <summary>
        /// Add a value to the <see cref="IList"/> if the list does not contain the value.
        /// The method does not check null for either the list or the value; it is not thread-safe.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        public static void AddIfNotExist(this IList list, object value)
        {
            // do not check null for either the list or the value; shall be check outside!
            if (!list.Contains(value))
                list.Add(value);
        }

        /// <summary>
        /// Add a value to the <see cref="IList"/> if the list does not contain the value by
        /// the criteria given; it is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static bool AddIfNotExist<T>(this IList<T> collection, T item, Func<T, bool> func)
        {
            if (!collection.Any(func))
            {
                collection.Add(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Inserts an item to the <see cref="IList{T}"/> at the specified index if it does not exist.
        /// The method does not check null for either the list or the value; it is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void InsertIfNotExist<T>(this IList<T> list, int index, T value)
        {
            index.ThrowIfNegative();
            // do not check null for either the list or the value; shall be check outside!);
            if (!list.Contains(value))
                list.Insert(index, value);
        }

        /// <summary>
        /// Search for an item which fulfills the condition provided by <paramref name="keySelector"/>.
        /// For example, for a list of User instance that contains a string property Name, one can
        /// set the criteria in <paramref name="keySelector"/> to identify a specific User by its Name.
        /// It is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="list"></param>
        /// <param name="keySelector"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T BinarySearch<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
            where TKey : IComparable<TKey>
        {
            int min = 0;
            int max = list.Count;
            while (min < max)
            {
                int mid = (max + min) / 2;
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);
                if (comp < 0)
                {
                    min = mid + 1;
                }
                else if (comp > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    return midItem;
                }
            }
            if (min == max &&
                keySelector(list[min]).CompareTo(key) == 0)
            {
                return list[min];
            }
            throw new InvalidOperationException("Item not found");
        }

        /// <summary>
        /// Check if the <paramref name="list"/> is null or contains no element. It is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }

        /// <summary>
        /// Replace all the occurences of <paramref name="replacement"/> in the
        /// <paramref name="list"/> by <paramref name="target"/>. It is not thread-safe.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="target"></param>
        /// <param name="replacement"></param>
        public static void Replace<T>(this IList<T> list, T target, T replacement)
        {
            list.ThrowIfNull();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(target))
                {
                    list[i] = replacement;
                }
            }
        }

        /// <summary>
        /// Advanced version of <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource},IEqualityComparer{TSource})"/> which
        /// could specify a key selector instead of the <see cref="IEqualityComparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            source.ThrowIfNull();
            keySelector.ThrowIfNull();
            return source.GroupBy(keySelector).Select(x => x.First());
        }

        /// <summary>
        /// Flatten a tree structure.
        /// For example: var descendants = children.Flatten(c => c.Children);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hierarchy"></param>
        /// <param name="childrenSelector"></param>
        /// <returns></returns>
        public static IList<T> Flatten<T>(this IList<T> hierarchy, Func<T, IList<T>> childrenSelector)
        {
            var result = new List<T>();

            foreach (var item in hierarchy)
            {
                result.AddRange(Flatten(childrenSelector(item), childrenSelector));
                if (!result.Contains(item))
                    result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Dequeue all items from the <see cref="ConcurrentQueue"/> using <see cref="ConcurrentQueue.TryDequeue"/>
        /// method. when fetching the items in the loop, if <see cref="ConcurrentQueue.TryDequeue"/>
        /// returns false, the loop will exit and the result items are returned.
        /// It is possible that during the loop other items are inserted
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static IList<T> DequeueAll<T>(this ConcurrentQueue<T> queue)
        {
            var results = new List<T>();
            T item;
            while (queue.TryDequeue(out item))
            {
                results.Add(item);
            }
            return results;
        }
    }
}
