using System.Collections.Generic;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class RandomizedPrims : IMazeGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Maze Generate()
        {
            var walls = new HashSet<Wall>();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (j < Height - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i, j + 1)));
                    if (i < Width - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i + 1, j)));
                }
            }
            var mazeCells = new HashSet<Cell>();
            var growingWalls = new HashSet<Wall>();
            var cell = Cell.RandomInMaze(Width, Height);
            mazeCells.Add(cell);
            foreach (var nextCellWall in cell.GetWalls(Width, Height))
            {
                growingWalls.Add(nextCellWall);
            }

            while (growingWalls.Count > 0)
            {
                var wall = growingWalls.Random();
                if (mazeCells.Contains(wall.One) && mazeCells.Contains(wall.Two))
                {
                    growingWalls.Remove(wall);
                }
                else
                {
                    var oppositeCell = mazeCells.Contains(wall.One) ? wall.Two : wall.One;

                    mazeCells.Add(oppositeCell);
                    foreach (var nextCellWall in oppositeCell.GetWalls(Width, Height))
                    {
                        growingWalls.Add(nextCellWall);
                    }
                    growingWalls.Remove(wall);

                    walls.Remove(wall);
                }
            }

            return new Maze { MazeWidth = Width, MazeHeight = Height, Walls = walls };
        }
    }
}