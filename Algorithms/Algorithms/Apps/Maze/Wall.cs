using System;

namespace Algorithms.Apps.Maze
{
    public class Wall
    {
        private readonly Cell one;
        private readonly Cell two;

        public int X1 { get { return one.X; } }
        public int Y1 { get { return one.Y; } }
        public int X2 { get { return two.X; } }
        public int Y2 { get { return two.Y; } }

        public bool IsHorizontal { get { return X1 == X2; } }

        public Wall(Cell one, Cell two)
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
        }

        public bool Contains(Cell cellOne, Cell cellTwo)
        {
            if (cellOne == one && cellTwo == two)
                return true;
            if (cellOne == two && cellTwo == one)
                return true;
            return false;
        }

        protected bool Equals(Wall other)
        {
            return Equals(one, other.one) && Equals(two, other.two);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Wall)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((one != null ? one.GetHashCode() : 0) * 397) ^ (two != null ? two.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Wall left, Wall right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Wall left, Wall right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0}--{1}", one, two);
        }
    }

}