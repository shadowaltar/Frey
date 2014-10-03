using System.Collections;
using System.Collections.Generic;

namespace Algorithms.Collections
{
    public class ArrayStack<T> : IStack<T>, IEnumerable<T>
    {
        private T[] items;
        private int count;

        public ArrayStack()
        {
            items = new T[8];
        }

        public ArrayStack(int count)
        {
            items = new T[count];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = count - 1; i >= 0; i--)
            {
                yield return items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Push(T item)
        {
            if (count == items.Length) Resize(2 * items.Length);

            items[count++] = item;
        }

        public T Pop()
        {
            var item = items[--count];
            items[count] = default(T);
            if (count > 0 && count == items.Length / 4) Resize(items.Length / 2);

            return item;
        }

        public T Peek()
        {
            return items[count - 1];
        }

        public bool IsEmpty()
        {
            return count == 0;
        }

        public int Count()
        {
            return count;
        }

        public void Clear()
        {
            items.Initialize();
            count = 0;
        }

        private void Resize(int max)
        {
            var temp = new T[max];
            for (int i = 0; i < count; i++)
            {
                temp[i] = items[i];
            }
            items = temp;
        }
    }

    public class LinkedListStack<T> : LinkedList<T>, IStack<T>
    {
        public void Push(T item)
        {
            Add(item);
        }

        public T Pop()
        {
            var i = lastNode.Item;
            RemoveAt(count - 1);
            return i;
        }

        public T Peek()
        {
            return lastNode.Item;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

        int IStack<T>.Count()
        {
            return Count;
        }
    }

    public interface IStack<T>
    {
        void Push(T item);
        T Pop();
        T Peek();
        bool IsEmpty();
        int Count();
        void Clear();
    }
}
