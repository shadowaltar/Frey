using System.Collections.Generic;

namespace Algorithms.Apps.Maze
{
    public class Maze
    {
        public int MazeWidth { get; set; }
        public int MazeHeight { get; set; }

        public HashSet<Wall> Walls { get; set; }
    }
}