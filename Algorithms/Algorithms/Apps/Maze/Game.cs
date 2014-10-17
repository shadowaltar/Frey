using Algorithms.Apps.Maze.Algorithms;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze
{
    public class Game
    {
        public Maze GenerateRectangular(int width, int height)
        {
            //IMazeGenerator gen = new RandomizedPrims();
            IMazeGenerator gen = new RandomizedKruskal();
           // IMazeGenerator gen = new RecursiveBacktracker();
            gen.Width = width;
            gen.Height = height;
            using (new ReportTime())
                return gen.Generate();
        }
    }
}