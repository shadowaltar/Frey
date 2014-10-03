using System;

namespace Algorithms.Collections
{
    public class Deque<T> : LinkedListStack<T>, IDeque<T>
    {
        public T PopLeft()
        {
            if (IsEmpty())
                throw new InvalidOperationException();

            var item = this[0];
            RemoveAt(0);
            return item;
        }

        public T PopRight()
        {
            if (IsEmpty())
                throw new InvalidOperationException();

            var item = this[Count - 1];
            RemoveAt(Count - 1);
            return item;
        }

        public void PushLeft(T item)
        {
            Insert(0, item);
        }

        public void PushRight(T item)
        {
            Add(item);
        }
    }

    public interface IDeque<T>
    {
        T PopLeft();
        T PopRight();
        void PushLeft(T item);
        void PushRight(T item);
    }
}