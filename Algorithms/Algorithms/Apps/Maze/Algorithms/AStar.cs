using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Algorithms.Algos;
using MoreLinq;

namespace Algorithms.Apps.Maze.Algorithms
{
    public class AStar : IMazeSolver
    {
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

            public static double GetH(Node a, Node b)
            {
                return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
            }

            public IEnumerable<Node> GetAccessibleNeighbors(int w, int h, bool[,][] walls, Node exit)
            {
                if (X != 0) // left
                {
                    var n = new Node(X - 1, Y, this);
                    if (!walls[n.X, Y][1])
                    {
                        n.Parent = this;
                        n.G = G + 1;
                        n.H = GetH(n, exit);
                        yield return n;
                    }
                }
                if (X != w - 1) // right
                {
                    var n = new Node(X + 1, Y, this);
                    if (!walls[X, Y][1])
                    {
                        n.Parent = this;
                        n.G = G + 1;
                        n.H = GetH(n, exit);
                        yield return n;
                    }
                }
                if (Y != 0) // top
                {
                    var n = new Node(X, Y - 1, this);
                    if (!walls[X, n.Y][0])
                    {
                        n.Parent = this;
                        n.G = G + 1;
                        n.H = GetH(n, exit);
                        yield return n;
                    }
                }
                if (Y != h - 1) // bottom
                {
                    var n = new Node(X, Y + 1, this);
                    if (!walls[X, Y][0])
                    {
                        n.Parent = this;
                        n.G = G + 1;
                        n.H = GetH(n, exit);
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
                if (obj.GetType() != this.GetType()) return false;
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
        protected List<Node> closeds;
        protected Maze maze;

        public void Solve(Maze maze)
        {
            opens = new List<Node>();
            closeds = new List<Node>();
            this.maze = maze;
            var simpliedWalls = maze.SimpliedWalls;
            var start = new Node { X = maze.Start.X, Y = maze.Start.Y };
            var exit = new Node { X = maze.Exit.X, Y = maze.Exit.Y };
            opens.Add(start);

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
                else
                {
                    var neighbors = s.GetAccessibleNeighbors(maze.MazeWidth, maze.MazeHeight, simpliedWalls, exit);
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
        }

        private List<Node> TraceBack(Node node)
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