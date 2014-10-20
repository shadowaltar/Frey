using System.Collections.Generic;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class DepthFirstSearch : IMazeGenerator
    {
        public Maze Generate()
        {
            var cells = new List<Cell>();
            var walls = new HashSet<Wall>();
            var marks = new bool[Width,Height];
            var q = new Queue<Cell>();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (j < Height - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i, j + 1)));
                    if (i < Width - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i + 1, j)));

                    cells.Add(new Cell(i, j));
                }
            }

            var n = cells.Random();
           q.Enqueue(n);
            marks[n.X, n.Y] = true;
            while (true)
            {
                var neighbors = FindNeighbors(n);
                var allNeighborsVisited = true;
                foreach (var neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        allNeighborsVisited = false;
                        break;
                    }
                }
                if (allNeighborsVisited)
                {
                    
                }

                var a = neighbors.Random();
                if ()
            }
        }

        private HashSet<Cell> FindNeighbors(Cell cell)
        {
            Cell neighbor;
            var results = new HashSet<Cell>();
            if (cell.X != 0)
            {
                neighbor = new Cell(cell.X - 1, cell.Y);
                results.Add(neighbor);
            }
            if (cell.Y != 0)
            {
                neighbor = new Cell(cell.X, cell.Y - 1);
                results.Add(neighbor);
            }
            if (cell.X != Width - 1)
            {
                neighbor = new Cell(cell.X + 1, cell.Y);
                results.Add(neighbor);
            }
            if (cell.Y != Height - 1)
            {
                neighbor = new Cell(cell.X, cell.Y + 1);
                results.Add(neighbor);
            }
            return results;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}