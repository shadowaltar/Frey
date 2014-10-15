using Algorithms.Apps.Maze.Algorithms;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze
{
    public class Game
    {
        public Maze GenerateRectangular(int width, int height)
        {
            IMazeGenerator gen = new RecursiveBacktracker();
            using (new ReportTime())
                return gen.Generate(width, height);
        }
    }
}