using System;
using Algorithms.Algos;
using Algorithms.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms.Tests.Sortings
{
    [TestFixture]
    public class TestSortings
    {
        [Test]
        public void Test()
        {
            var count = 10000000;
            var input1 = Series.RandomArray(count, 0, count).ToArray();
            var input2 = new int[count];
            var input3 = new int[count];
            Array.Copy(input1, input2, count);
            Array.Copy(input1, input3, count);

            using (new ReportTime()) // 18ms 100k; 3.8s 10M
                Algos.Sortings.ShellSort(input1);
            Assert.True(Assertions.IsSorted(input1));

            //using (new ReportTime()) // 6.0s 100k
            //    Algos.Sortings.InsertionSort(input2);
            //Assert.True(Assertions.IsSorted(input2));

            //using (new ReportTime()) // 6.7s 100k
            //    Algos.Sortings.SelectionSort(input3);
            //Assert.True(Assertions.IsSorted(input3));
        }
    }
}