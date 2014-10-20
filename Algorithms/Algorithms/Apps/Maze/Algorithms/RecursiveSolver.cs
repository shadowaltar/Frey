using System.Collections.Generic;

namespace Algorithms.Apps.Maze.Algorithms
{
    /// <summary>
    /// Not working
    /// </summary>
    public class RecursiveSolver : IMazeSolver
    {
        private int endX;
        private int endY;
        private int width;
        private int height;
        private bool[,] traversed;
        private bool[,] solution;

        public void Solve(Maze maze)
        {
            traversed = new bool[maze.MazeWidth, maze.MazeHeight];
            solution = new bool[maze.MazeWidth, maze.MazeHeight];
            endX = maze.Exit.X;
            endY = maze.Exit.Y;
            width = maze.MazeWidth;
            height = maze.MazeHeight;
            RecursiveSolve(maze.Start.X, maze.Start.Y);
        }

        private bool RecursiveSolve(int x, int y)
        {
            if (x == endX && y == endY)
                return true;
            if (traversed[x, y])
                return false;
            traversed[x, y] = true;
            if (x != 0)
            {
                if (RecursiveSolve(x - 1, y))
                {
                    solution[x, y] = true;
                    return true;
                }
            }
            if (x != width - 1)
            {
                if (RecursiveSolve(x + 1, y))
                {
                    solution[x, y] = true;
                    return true;
                }
            }
            if (y != 0)
            {
                if (RecursiveSolve(x, y - 1))
                {
                    solution[x, y] = true;
                    return true;
                }
            }
            if (y != height - 1)
            {
                if (RecursiveSolve(x, y + 1))
                {
                    solution[x, y] = true;
                    return true;
                }
            }
            return false;
        }

        public List<Cell> Solution { get; private set; }
    }
}