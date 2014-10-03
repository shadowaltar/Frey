using System.Collections.Generic;

namespace Algorithms.Exercises
{
    public class MoveToFront
    {
        private LinkedList<char> list = new LinkedList<char>();

        public void Sort(string input)
        {
            foreach (var c in input)
            {
                if (list.Contains(c))
                    list.Remove(c);
                list.AddFirst(c);
            }
        }

        public void Clear()
        {
            list = new LinkedList<char>();
        }
    }
}