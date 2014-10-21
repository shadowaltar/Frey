using System;
using System.Collections.Generic;
using Algorithms.Apps.Maze.Algorithms;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze
{
    public class Game
    {
        public Maze Maze { get; protected set; }

        public void GenerateRectangular<T>(int width, int height) where T : IMazeGenerator
        {
            var gen = (IMazeGenerator)Activator.CreateInstance<T>();
            gen.Width = width;
            gen.Height = height;
            using (new ReportTime())
                Maze = gen.Generate();
        }

        public void Solve<T>() where T : IMazeSolver
        {
            var solver = (IMazeSolver)Activator.CreateInstance<T>();
            solver.Solve(Maze);
            Maze.Solution = solver.Solution;
        }
    }
}