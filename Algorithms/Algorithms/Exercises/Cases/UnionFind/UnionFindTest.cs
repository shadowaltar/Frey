using System;
using Algorithms.Utils;

namespace Algorithms.Exercises.Cases.UnionFind
{
    public static class UnionFindTest
    {
        public static void Test()
        {
            using (new ReportTime("Total {0}"))
            {
                int n = 1000000;

                int[,] generated;
                using (new ReportTime("Random Number Generation {0}"))
                    generated = GenerateData(n);

                var uf = new WeightedQuickUnionFind(n);
                using (new ReportTime("Union Find NONOPTIMIZE {0}"))
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (uf.IsConnected(generated[i, 0], generated[i, 1]))
                        {
                            continue;
                        }
                        uf.Union(generated[i, 0], generated[i, 1]);
                    }
                }
                Console.WriteLine(uf.Count() + " components");
            }
        }

        private static int[,] GenerateData(int count)
        {
            var series = new int[count, 2];
            var r = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < count; i++)
            {
                var p = -1;
                var q = -1;
                while (p == q)
                {
                    p = r.Next(0, count);
                    q = r.Next(0, count);
                }
                series[i, 0] = p;
                series[i, 1] = q;
            }
            return series;
        }
    }
}