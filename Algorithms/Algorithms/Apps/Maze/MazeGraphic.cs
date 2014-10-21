using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Algorithms.Apps.Maze
{
    public class MazeGraphics
    {
        private readonly Graphics graphics;
        private int pathDisplayWidth;
        private int pathDisplayHeight;

        public MazeGraphics(Graphics graphics)
        {
            this.graphics = graphics;
            PathDisplayWidth = 4;
            PathDisplayHeight = 4;
            WallDisplayWidth = 2;
            WallDisplayHeight = 2;
            MarginTop = 4;
            MarginLeft = 4;
        }

        public double MazeDisplayWidth { get; set; }
        public double MazeDisplayHeight { get; set; }

        public int PathDisplayWidth
        {
            get { return pathDisplayWidth; }
            set
            {
                if (value % 2 == 1) pathDisplayWidth = value;
                else pathDisplayWidth = value + 1;
            }
        }

        public int PathDisplayHeight
        {
            get { return pathDisplayHeight; }
            set
            {
                if (value % 2 == 1) pathDisplayHeight = value;
                else pathDisplayHeight = value + 1;
            }
        }

        public int WallDisplayWidth { get; set; }
        public int WallDisplayHeight { get; set; }
        protected int MarginTop { get; set; }
        protected int MarginLeft { get; set; }

        public void DrawMaze(ref Image image, Maze maze)
        {
            var mazeDisplayWidth = maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + WallDisplayWidth - 1;
            var mazeDisplayHeight = maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight - 1;

            var canvasWidth = mazeDisplayWidth + MarginLeft * 2;
            var canvasHeight = mazeDisplayHeight + MarginTop * 2;

            WriteableBitmap bitmap;
            if (image.Source == null)
            {
                bitmap = BitmapFactory.New(canvasWidth, canvasHeight);
                image.Width = canvasWidth;
                image.Height = canvasHeight;
                image.Source = bitmap;
            }
            else
            {
                bitmap = (WriteableBitmap)image.Source;
            }

            // top edge
            graphics.FillRectangle(bitmap,
                MarginLeft + 1,
                MarginTop + 1,
                MarginLeft + maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + 1,
                MarginTop + WallDisplayHeight);
            // left edge
            graphics.FillRectangle(bitmap,
                MarginLeft + 1,
                MarginTop + 1,
                MarginLeft + WallDisplayWidth,
                MarginTop + maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight);
            // right edge
            graphics.FillRectangle(bitmap,
                MarginLeft + maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + 1,
                MarginTop + 1,
                MarginLeft + maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + WallDisplayWidth,
                MarginTop + maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight);
            // bottom edge
            graphics.FillRectangle(bitmap,
                MarginLeft + 1,
                MarginTop + maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + 1,
                MarginLeft + maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + WallDisplayWidth,
                MarginTop + maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight);

            foreach (var wall in maze.Walls)
            {
                if (wall.IsHorizontal)
                {
                    graphics.FillRectangle(bitmap, MarginLeft + wall.X1 * (WallDisplayWidth + PathDisplayWidth) + 1,
                        MarginTop + (wall.Y1 + 1) * (WallDisplayHeight + PathDisplayHeight) + 1,
                        MarginLeft + (wall.X1 + 1) * (WallDisplayWidth + PathDisplayWidth) + WallDisplayWidth,
                        MarginTop + (wall.Y1 + 1) * (WallDisplayHeight + PathDisplayHeight) + WallDisplayHeight);
                }
                else
                {
                    graphics.FillRectangle(bitmap, MarginLeft + (wall.X1 + 1) * (WallDisplayWidth + PathDisplayWidth) + 1,
                        MarginTop + wall.Y1 * (WallDisplayHeight + PathDisplayHeight) + 1,
                        MarginLeft + (wall.X1 + 1) * (WallDisplayWidth + PathDisplayWidth) + WallDisplayWidth,
                        MarginTop + (wall.Y1 + 1) * (WallDisplayHeight + PathDisplayHeight) + WallDisplayHeight);
                }
            }
        }

        public void DrawSolution(ref Image image, Maze maze)
        {
            if (maze.Solution == null || maze.Solution.Count <= 0)
                return;

            var mazeDisplayWidth = maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + WallDisplayWidth - 1;
            var mazeDisplayHeight = maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight - 1;

            var canvasWidth = mazeDisplayWidth + MarginLeft * 2;
            var canvasHeight = mazeDisplayHeight + MarginTop * 2;

            WriteableBitmap bitmap;
            if (image.Source == null)
            {
                bitmap = BitmapFactory.New(canvasWidth, canvasHeight);
                image.Width = canvasWidth;
                image.Height = canvasHeight;
                image.Source = bitmap;
            }
            else
            {
                bitmap = (WriteableBitmap)image.Source;
            }

            var one = maze.Start;
            for (int i = 0; i < maze.Solution.Count; i++)
            {
                var two = maze.Solution[i];
                graphics.DrawLine(bitmap,
                    MarginLeft + WallDisplayWidth + one.X * (WallDisplayWidth + PathDisplayWidth) + (PathDisplayWidth + 1) / 2,
                    MarginTop + WallDisplayHeight + one.Y * (WallDisplayHeight + PathDisplayHeight) + (PathDisplayHeight + 1) / 2,
                    MarginLeft + WallDisplayWidth + two.X * (WallDisplayWidth + PathDisplayWidth) + (PathDisplayWidth + 1) / 2,
                    MarginTop + WallDisplayHeight + two.Y * (WallDisplayHeight + PathDisplayHeight) + (PathDisplayHeight + 1) / 2);

                one = two;
            }
        }
    }
}