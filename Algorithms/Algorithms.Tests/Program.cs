using System;
using Algorithms.Apps.Maze;
using Algorithms.Exercises.Cases.UnionFind;

namespace Algorithms.Tests
{
    public class Program
    {
        [STAThread]
        public static void Main(params string[] args)
        {
            var game = new Game();
            var maze = game.GenerateRectangular(10, 10);
            Console.ReadLine();
        }
    }
}