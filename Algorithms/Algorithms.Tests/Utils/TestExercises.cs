using System;
using System.Linq;
using Algorithms.Algos;
using NUnit.Framework;

namespace Algorithms.Tests.Utils
{
    [TestFixture]
    public class TestExercises
    {
        [Test]
        public void TestCheckParentheses()
        {
            Assert.True(Exercises.Exercises.CheckParentheses("[()]{}{[()()]()}"));
            Assert.False(Exercises.Exercises.CheckParentheses("[(])"));
        }
        [Test]
        public void TestSolveJosephus()
        {
            var results = Games.SolveJosephus(4, 7, 1).ToList();
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}