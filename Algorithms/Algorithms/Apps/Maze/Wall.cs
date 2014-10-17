using System;

namespace Algorithms.Apps.Maze
{
    public struct Wall
    {
        private readonly Cell one;
        private readonly Cell two;

        public int X1 { get { return one.X; } }
        public int Y1 { get { return one.Y; } }
        public int X2 { get { return two.X; } }
        public int Y2 { get { return two.Y; } }

        public Cell One {get { return one; }}
        public Cell Two { get { return two; } }

        public bool IsHorizontal { get { return X1 == X2; } }

        public bool IsInitialized { get; private set; }

        public Wall(Cell one, Cell two)
            : this()
        {
            this.one = one;
            this.two = two;
            if (one.X == two.X && one.Y == two.Y)
                throw new ArgumentException();

            // swap one & two, make sure one.x<=two.x and one.y<=two.y
            if (one.X > two.X || (one.X == two.X && one.Y > two.Y))
            {
                var temp = this.two;
                this.two = one;
                this.one = temp;
            }

            IsInitialized = true;
        }

        public Cell GetOpposite(Cell c)
        {
            if (c == one)
                return two;
            if (c == two)
                return one;
            return default(Cell);
        }

        public bool Contains(Cell cellOne, Cell cellTwo)
        {
            if (cellOne == one && cellTwo == two)
                return true;
            if (cellOne == two && cellTwo == one)
                return true;
            return false;
        }

        public bool Equals(Wall other)
        {
            return one.Equals(other.one) && two.Equals(other.two) && IsInitialized.Equals(other.IsInitialized);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Wall && Equals((Wall)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = one.GetHashCode();
                hashCode = (hashCode * 397) ^ two.GetHashCode();
                hashCode = (hashCode * 397) ^ IsInitialized.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Wall left, Wall right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Wall left, Wall right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("{0}--{1}", one, two);
        }
    }
}