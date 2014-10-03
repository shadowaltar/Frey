using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Utils
{
    public static class Extensions
    {
        public static IEnumerable<T> Values<T>(this Type x)
        {
            return Enum.GetValues(x).Cast<T>();
        }
    }
}