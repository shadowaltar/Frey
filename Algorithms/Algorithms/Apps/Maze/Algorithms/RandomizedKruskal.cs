using System.Collections.Generic;
using Algorithms.Exercises.Cases.UnionFind;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class RandomizedKruskal : IMazeGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Maze Generate()
        {
            var walls = new HashSet<Wall>();
            var cellCount = Width * Height;
            var uf = new WeightedQuickUnionFind(cellCount);

            var cellIndexes = new int[Width, Height];
            var cellIndex = 0;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (j < Height - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i, j + 1)));
                    if (i < Width - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i + 1, j)));

                    cellIndexes[i, j] = cellIndex;
                    cellIndex++;
                }
            }

            while (true)
            {
                var wall = walls.Random();
                var index1 = cellIndexes[wall.X1, wall.Y1];
                var index2 = cellIndexes[wall.X2, wall.Y2]; // they represent two adjacent cells
                if (!uf.IsConnected(index1, index2))
                {
                    walls.Remove(wall);
                    uf.Union(index1, index2);
                    cellCount--;
                }
                if (cellCount == 1)
                    break;
            }
            return new Maze { MazeWidth = Width, MazeHeight = Height, Walls = walls };
        }
    }
}