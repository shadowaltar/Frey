using System.Collections.Generic;

namespace Algorithms.Exercises
{
    public class Buffer : IBuffer
    {
        private Stack<char> stack = new Stack<char>();

        public void Insert(char c)
        {

            throw new System.NotImplementedException();
        }

        public char Get()
        {
            throw new System.NotImplementedException();
        }

        public char Delete()
        {
            throw new System.NotImplementedException();
        }

        public void Left(int k)
        {
            throw new System.NotImplementedException();
        }

        public void Right(int k)
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; private set; }
    }

    public interface IBuffer
    {
        void Insert(char c);
        char Get();
        char Delete();
        void Left(int k);
        void Right(int k);
        int Count { get; }
    }
}