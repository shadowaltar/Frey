using System;
using System.Collections;
using System.Collections.Generic;

namespace Algorithms.Collections
{
    /// <summary>
    /// A collection where removing items is not supported. The order of enumeration is unspecified and
    /// immaterial.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bag<T> : IBag<T>, IEnumerable<T>
    {
        private readonly LinkedList<T> list = new LinkedList<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public bool IsEmpty()
        {
            return list.Count == 0;
        }

        public int Count { get { return list.Count; } }
    }

    public class RandomBag<T> : IBag<T>, IEnumerable<T>
    {
        private readonly LinkedList<T> list = new LinkedList<T>();

        public IEnumerator<T> GetEnumerator()
        {
            var r = new Random();
            var count = list.Count;
            var items = new T[count];
            for (int i = 0; i < list.Count; i++)
            {
                items[i] = list[i];
            }

            while (true)
            {
                var i = r.Next(0, count);
                yield return items[i];
                items[i] = items[count - 1];
                count--;
                if (count == 0)
                    break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public bool IsEmpty()
        {
            return list.Count == 0;
        }

        public int Count { get { return list.Count; } }
    }

    public interface IBag<in T>
    {
        void Add(T item);
        bool IsEmpty();
        int Count { get; }
    }
}