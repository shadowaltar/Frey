using System.Collections.Generic;

namespace Algorithms.Apps.Maze
{
    public class Cell
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        protected bool Equals(Cell other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Cell)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public static bool operator ==(Cell left, Cell right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Cell left, Cell right)
        {
            return !Equals(left, right);
        }

        private sealed class XYEqualityComparer : IEqualityComparer<Cell>
        {
            public bool Equals(Cell x, Cell y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.X == y.X && x.Y == y.Y;
            }

            public int GetHashCode(Cell obj)
            {
                unchecked
                {
                    return (obj.X * 397) ^ obj.Y;
                }
            }
        }

        private static readonly IEqualityComparer<Cell> XYComparerInstance = new XYEqualityComparer();

        public static IEqualityComparer<Cell> XYComparer
        {
            get { return XYComparerInstance; }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}