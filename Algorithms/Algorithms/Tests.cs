using System;
using System.Windows;
using Algorithms.Algos;

namespace Algorithms
{
    public class Tests
    {
        public static double Test(double p, double q)
        {
            var inputs = new[] { 1.1, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7, 8.8 };
            var outputs = inputs.Scramble();

            var result = "";
            foreach (var l in outputs)
            {
                result += (l + ",");
            }
            MessageBox.Show(result);

            return 0;
        }
    }
}