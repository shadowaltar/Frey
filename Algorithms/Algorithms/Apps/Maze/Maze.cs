using System.Collections.Generic;

namespace Algorithms.Apps.Maze
{
    public class Maze
    {
        public int MazeWidth { get; set; }
        public int MazeHeight { get; set; }

        public Cell Start { get; set; }
        public Cell Exit { get; set; }

        public HashSet<Wall> Walls { get; set; }

        public List<Cell> Solution { get; set; } 

        public bool[,][] SimpliedWalls { get { return GenerateSimpliedWalls(); } }

        private bool[,][] GenerateSimpliedWalls()
        {
            var walls = new bool[MazeWidth, MazeHeight][];
            for (int i = 0; i < MazeWidth; i++)
            {
                for (int j = 0; j < MazeHeight; j++)
                {
                    walls[i, j] = new bool[2];
                }
            }
            foreach (var wall in Walls)
            {
                if (wall.X1 == wall.X2) // horizontal
                    walls[wall.X1, wall.Y1][0] = true;
                else
                    walls[wall.X1, wall.Y1][1] = true;
            }
            return walls;
        }

        public void SetDefaultExit()
        {
            Exit = new Cell(MazeWidth - 1, MazeHeight - 1);
        }
    }
}