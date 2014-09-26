using Algorithms.Collections;
using System;

namespace Algorithms
{
    public class Tests
    {
        public static double Test(double p, double q)
        {

            var list = new LinkedList<string>();

            list.Add("a");
            list.Add("b");
            list.Add("c");
            list.Add("d");
            list.Add("e");
            list.Add("f");
            list.Add("g");

            list.Remove("b");

            list.Insert(1,"b");
            list.RemoveAt(5);
            list.RemoveAt(1);

            return 0;
        }
    }
}