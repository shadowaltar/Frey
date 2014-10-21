using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class AStar : IMazeSolver
    {
        private double[,] hValues;

        protected class Node
        {
            public Node()
            {
            }

            private Node(int x, int y, Node parent)
            {
                X = x;
                Y = y;
                Parent = parent;
            }

            public int X { get; set; }
            public int Y { get; set; }
            public Node Parent { get; set; }

            public double F { get { return G + H; } }
            public int G { get; set; }
            public double H { get; set; }

            public IEnumerable<Node> GetAccessibleNeighbors(int w, int h, bool[,][] walls, AStar solver)
            {
                if (X != 0) // left
                {
                    if (!walls[X - 1, Y][1])
                    {
                        var n = new Node(X - 1, Y, this) { Parent = this, G = G + 1 };
                        n.H = solver.hValues[n.X, n.Y];
                        yield return n;
                    }
                }
                if (X != w - 1) // right
                {
                    if (!walls[X, Y][1])
                    {
                        var n = new Node(X + 1, Y, this) { Parent = this, G = G + 1 };
                        n.H = solver.hValues[n.X, n.Y];
                        yield return n;
                    }
                }
                if (Y != 0) // top
                {
                    if (!walls[X, Y - 1][0])
                    {
                        var n = new Node(X, Y - 1, this) { Parent = this, G = G + 1 };
                        n.H = solver.hValues[n.X, n.Y];
                        yield return n;
                    }
                }
                if (Y != h - 1) // bottom
                {
                    if (!walls[X, Y][0])
                    {
                        var n = new Node(X, Y + 1, this) { Parent = this, G = G + 1 };
                        n.H = solver.hValues[n.X, n.Y];
                        yield return n;
                    }
                }
            }

            protected bool Equals(Node other)
            {
                return X == other.X && Y == other.Y;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Node)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (X * 397) ^ Y;
                }
            }

            public static bool operator ==(Node left, Node right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Node left, Node right)
            {
                return !Equals(left, right);
            }

            public override string ToString()
            {
                return string.Format("({0}, {1}), F,G,H: {2}, {3}, {4}", X, Y, F, G, H);
            }
        }

        protected List<Node> opens;
        protected HashSet<Node> closeds;
        protected Maze maze;

        public void Solve(Maze m)
        {
            maze = m;

            var simpliedWalls = maze.SimpliedWalls;
            var start = new Node { X = maze.Start.X, Y = maze.Start.Y };
            var exit = new Node { X = maze.Exit.X, Y = maze.Exit.Y };

            opens = new List<Node>();
            closeds = new HashSet<Node>();
            opens.Add(start);

            // init h values
            hValues = new double[maze.MazeWidth, maze.MazeHeight];
            var tx = exit.X;
            var ty = exit.Y;
            for (int i = 0; i < maze.MazeWidth; i++)
            {
                for (int j = 0; j < maze.MazeHeight; j++)
                {
                    hValues[i, j] = Math.Sqrt(Math.Pow(i - tx, 2) + Math.Pow(j - ty, 2));
                }
            }

            while (opens.Count > 0)
            {
                var s = opens.MinBy(o => o.F);
                opens.Remove(s);
                closeds.Add(s);

                if (s == exit)
                {
                    Solution = TraceBack(s).Select(c => new Cell(c.X, c.Y)).ToList();
                    return;
                }

                var neighbors = s.GetAccessibleNeighbors(maze.MazeWidth, maze.MazeHeight, simpliedWalls, this);
                // not know g
                foreach (var neighbor in neighbors)
                {
                    if (!closeds.Contains(neighbor))
                    {
                        var exist = opens.FirstOrDefault(o => o.X == neighbor.X && o.Y == neighbor.Y);
                        if (exist != null)
                        {
                            if (neighbor.F < exist.F)
                            {
                                opens.Remove(exist);
                                opens.Add(neighbor);
                            }
                        }
                        else
                        {
                            opens.Add(neighbor);
                        }
                    }
                }
            }
        }

        private IEnumerable<Node> TraceBack(Node node)
        {
            var n = node;
            var results = new List<Node>();
            while (n != null && n.Parent != null)
            {
                results.Add(n);
                n = n.Parent;
            }
            results.Reverse();
            return results;
        }

        public List<Cell> Solution { get; private set; }
    }
}