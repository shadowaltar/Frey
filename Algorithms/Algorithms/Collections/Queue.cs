using System.Collections;
using System.Collections.Generic;

namespace Algorithms.Collections
{
    public class Queue<T> : LinkedList<T>, IQueue<T>
    {
        public void Enqueue(T item)
        {
            throw new System.NotImplementedException();
        }

        public T Dequeue()
        {
            throw new System.NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new System.NotImplementedException();
        }

        public new int Count()
        {
            return base.Count;
        }

        public new void Clear()
        {
            base.Clear();
        }
    }

    public interface IQueue<T>
    {
        void Enqueue(T item);
        T Dequeue();
        bool IsEmpty();
        int Count();
        void Clear();
    }
}