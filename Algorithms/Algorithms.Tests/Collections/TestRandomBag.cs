using System;
using Algorithms.Algos;
using Algorithms.Collections;
using NUnit.Framework;

namespace Algorithms.Tests.Collections
{
    [TestFixture]
    public class TestRandomBag
    {
        [Test]
        public void TestGetEnumerator()
        {
            var bag = new RandomBag<int>();
            foreach (var i in Series.Sequence(1, 50))
            {
                bag.Add(i);
            }
            foreach (var i in bag)
            {
                Console.WriteLine(i);
            }
        }
    }
}