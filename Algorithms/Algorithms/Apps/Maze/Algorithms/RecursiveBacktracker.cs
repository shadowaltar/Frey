using System.Collections.Generic;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class RecursiveBacktracker : IMazeGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Maze Generate()
        {
            var stack = new Stack<Cell>();
            var visited = new bool[Width, Height];
            var unvisited = new HashSet<Cell>();
            var walls = new bool[Width, Height][]; // f, invalid/not-init; t, valid

            Cell currentCell;
            // prepare cells n walls
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    currentCell = new Cell(i, j);
                    unvisited.Add(currentCell);
                    walls[i, j] = new bool[2];
                    if (j < Height - 1)
                        walls[i, j][0] = true; //horizontal, bottom edge, #0
                    if (i < Width - 1)
                        walls[i, j][1] = true; //vertical, right edge, #1
                }
            }

            currentCell = new Cell(0, 0);
            unvisited.Remove(currentCell);
            visited[currentCell.X, currentCell.Y] = true;
            while (unvisited.Count > 0)
            {
                var neighbors = FindUnvisitedNeighborCells(currentCell, visited);
                var neighbor = neighbors.Random();
                if (neighbor.IsInitialized)
                {
                    stack.Push(currentCell);
                    RemoveWall(walls, currentCell, neighbor);
                    currentCell = neighbor;
                    unvisited.Remove(currentCell);
                    visited[currentCell.X, currentCell.Y] = true;
                }
                else if (stack.Count > 0)
                {
                    currentCell = stack.Pop();
                }
                else
                {
                    currentCell = unvisited.Random();
                    unvisited.Remove(currentCell);
                    visited[currentCell.X, currentCell.Y] = true;
                }
            }

            var newWalls = new HashSet<Wall>();
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (walls[i, j][0]) // horizontal
                    {
                        newWalls.Add(new Wall(i, j, i, j + 1));
                    }
                    if (walls[i, j][1]) // vertical
                    {
                        newWalls.Add(new Wall(i, j, i + 1, j));
                    }
                }
            }
            return new Maze { MazeWidth = Width, MazeHeight = Height, Walls = newWalls };
        }

        private HashSet<Cell> FindUnvisitedNeighborCells(Cell cell, bool[,] visited)
        {
            var results = new HashSet<Cell>();
            if (cell.X != 0 && !visited[cell.X - 1, cell.Y]) results.Add(new Cell(cell.X - 1, cell.Y));
            if (cell.Y != 0 && !visited[cell.X, cell.Y - 1]) results.Add(new Cell(cell.X, cell.Y - 1));
            if (cell.X != Width - 1 && !visited[cell.X + 1, cell.Y]) results.Add(new Cell(cell.X + 1, cell.Y));
            if (cell.Y != Height - 1 && !visited[cell.X, cell.Y + 1]) results.Add(new Cell(cell.X, cell.Y + 1));
            return results;
        }

        private void RemoveWall(bool[,][] walls, Cell one, Cell two)
        {
            // swap one & two, make sure one.x<=two.x and one.y<=two.y
            if (one.X > two.X || (one.X == two.X && one.Y > two.Y))
            {
                var temp = two;
                two = one;
                one = temp;
            }
            if (one.X == two.X)
                walls[one.X, one.Y][0] = false;
            else
                walls[one.X, one.Y][1] = false;
        }
    }
}