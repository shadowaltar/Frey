using System.Collections.Generic;
using Algorithms.Utils;

namespace Algorithms.Apps.Maze
{
    public struct Cell
    {
        public Cell(int x, int y)
            : this()
        {
            X = x;
            Y = y;
            IsInitialized = true;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public bool IsInitialized { get; private set; }

        public static Cell RandomInMaze(int mazeWidth, int mazeHeight)
        {
            return new Cell(StaticRandom.Instance.Next(0, mazeWidth), StaticRandom.Instance.Next(0, mazeHeight));
        }

        public bool Equals(Cell other)
        {
            return X == other.X && IsInitialized.Equals(other.IsInitialized) && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Cell && Equals((Cell)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ IsInitialized.GetHashCode();
                hashCode = (hashCode * 397) ^ Y;
                return hashCode;
            }
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}){2}", X, Y, IsInitialized ? "" : " INVALID");
        }

        public IEnumerable<Wall> GetWalls(int width, int height)
        {
            if (X != 0)
                yield return new Wall(this, new Cell(X - 1, Y));
            if (X != width - 1)
                yield return new Wall(this, new Cell(X + 1, Y));
            if (Y != 0)
                yield return new Wall(this, new Cell(X, Y - 1));
            if (Y != height - 1)
                yield return new Wall(this, new Cell(X, Y + 1));
        }

        public IEnumerable<Cell> GetNeighbors(int width, int height)
        {
            if (X != 0)
                yield return new Cell(X - 1, Y);
            if (X != width - 1)
                yield return new Cell(X + 1, Y);
            if (Y != 0)
                yield return new Cell(X, Y - 1);
            if (Y != height - 1)
                yield return new Cell(X, Y + 1);
        }
    }
}