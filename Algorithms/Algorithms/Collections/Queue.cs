namespace Algorithms.Collections
{
    public class Queue<T> : LinkedList<T>, IQueue<T>
    {
        public void Enqueue(T item)
        {
            Add(item);
        }

        public T Dequeue()
        {
            if (!IsEmpty())
            {
                var item = firstNode.Item;
                RemoveAt(0);
                return item;
            }
            return default(T);
        }

        public bool IsEmpty()
        {
            return Count() == 0;
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