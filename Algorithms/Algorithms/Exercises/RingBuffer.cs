using System.Threading;

namespace Algorithms.Exercises
{
    public class RingBuffer<T>
    {
        private T[] buffer;

        private int startIndex = 0;
        private int endIndex = 0;
        private int count = 0;

        public RingBuffer(int size)
        {
            buffer = new T[size];
        }

        public void Pop(T item)
        {
            buffer[endIndex++] = item;

            if (endIndex == buffer.Length)
                endIndex = 0;
        }

        public T Push()
        {
            var item = buffer[startIndex++];

            if (startIndex == buffer.Length)
                startIndex = 0;

            if (endIndex + 1 == startIndex || (startIndex == 0 && endIndex == buffer.Length))
            {
                // full
            }

            return item;
        }

        public bool IsEmpty()
        {
            return startIndex == endIndex;
        }
    }
}