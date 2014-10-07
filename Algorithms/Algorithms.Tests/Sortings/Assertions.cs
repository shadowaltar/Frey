using System.Collections.Generic;

namespace Algorithms.Tests.Sortings
{
    public static class Assertions
    {
        public static bool IsSorted(int[] input)
        {
            var previous = input[0];
            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] < previous)
                {
                    return false;
                }
            }
            return true;
        }
    }
}