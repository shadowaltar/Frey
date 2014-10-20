using System.Collections.Generic;

namespace Algorithms.Apps.Maze.Algorithms
{
    public interface IMazeSolver
    {
        void Solve(Maze maze);
        List<Cell> Solution { get; }
    }
}