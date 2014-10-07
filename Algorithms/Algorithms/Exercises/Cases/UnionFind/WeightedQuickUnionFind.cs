using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Exercises.Cases.UnionFind
{
    public class WeightedQuickUnionFind : IUnionFind
    {
        /// <summary>
        /// Storage of component ids. Indexed by site id.
        /// </summary>
        private readonly List<int> ids;
        private readonly List<int> sizes;
        private int count;

        public WeightedQuickUnionFind(int n)
        {
            count = n;
            ids = new List<int>(n);
            sizes = new List<int>(n);
            for (int i = 0; i < n; i++)
            {
                ids.Add(i); // one site, one comp; comp id is the size id.
                sizes.Add(1); // depth of tree is all 1 init.
            }
        }

        public void Union(int p, int q)
        {
            int i = Find(p);
            int j = Find(q);
            if (i == j)
                return;

            if (sizes[i] < sizes[j])
            {
                ids[i] = j;
                sizes[j] += sizes[i];
            }
            else
            {
                ids[j] = i;
                sizes[i] += sizes[j];
            }

            count--;
        }

        public int Find(int p)
        {
            var i = p;
            while (i != ids[i])
                i = ids[i];

            return i;
        }

        public bool IsConnected(int p, int q)
        {
            if (p < 0 || q < 0)
                throw new IndexOutOfRangeException();

            return Find(p) == Find(q);
        }

        public int Count()
        {
            return count;
        }

        public int MaxDepth()
        {
            return sizes.Max();
        }
    }
}