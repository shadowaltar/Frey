using System.Collections.Generic;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class RecursiveBacktracker : IMazeGenerator
    {
        private int width;
        private int height;

        public Maze Generate(int w, int h)
        {
            width = w;
            height = h;
            var stack = new Stack<Cell>();
            var visited = new HashSet<Cell>();
            var unvisited = new HashSet<Cell>();
            var walls = new HashSet<Wall>();
            Cell currentCell;
            // prepare cells n walls
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    currentCell = new Cell(i, j);
                    unvisited.Add(currentCell);

                    if (j < height - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i, j + 1)));
                    if (i < width - 1)
                        walls.Add(new Wall(new Cell(i, j), new Cell(i + 1, j)));
                }
            }

            currentCell = new Cell(0, 0);
            unvisited.Remove(currentCell);
            visited.Add(currentCell);
            while (unvisited.Count > 0)
            {
                var neighbors = FindUnvisitedNeighborCells(currentCell, visited);
                var neighbor = neighbors.Random();
                if (neighbor != null)
                {
                    stack.Push(currentCell);
                    RemoveWall(walls, currentCell, neighbor);
                    currentCell = neighbor;
                    unvisited.Remove(currentCell);
                    visited.Add(currentCell);
                }
                else if (stack.Count > 0)
                {
                    currentCell = stack.Pop();
                }
                else
                {
                    currentCell = unvisited.Random();
                    unvisited.Remove(currentCell);
                    visited.Add(currentCell);
                }
            }

            return new Maze { MazeWidth = w, MazeHeight = h, Walls = walls };
        }

        private HashSet<Cell> FindUnvisitedNeighborCells(Cell cell, HashSet<Cell> visited)
        {
            Cell neighbor;
            var results = new HashSet<Cell>();
            if (cell.X != 0)
            {
                neighbor = new Cell(cell.X - 1, cell.Y);
                if (!visited.Contains(neighbor))
                    results.Add(neighbor);
            }
            if (cell.Y != 0)
            {
                neighbor = new Cell(cell.X, cell.Y - 1);
                if (!visited.Contains(neighbor))
                    results.Add(neighbor);
            }
            if (cell.X != width - 1)
            {
                neighbor = new Cell(cell.X + 1, cell.Y);
                if (!visited.Contains(neighbor))
                    results.Add(neighbor);
            }
            if (cell.Y != height - 1)
            {
                neighbor = new Cell(cell.X, cell.Y + 1);
                if (!visited.Contains(neighbor))
                    results.Add(neighbor);
            }
            return results;
        }

        private void RemoveWall(HashSet<Wall> walls, Cell cellOne, Cell cellTwo)
        {
            walls.RemoveWhere(wall => wall.Contains(cellOne, cellTwo));
        }
    }
}