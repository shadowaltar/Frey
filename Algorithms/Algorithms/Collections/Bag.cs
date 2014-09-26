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
        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new System.NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new System.NotImplementedException();
        }

        public int Count()
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IBag<in T>
    {
        void Add(T item);
        bool IsEmpty();
        int Count();
    }
}