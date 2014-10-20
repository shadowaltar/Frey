using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Algorithms.Apps.Maze
{
    public class MazeGraphics
    {
        private readonly Graphics graphics;

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
        public int PathDisplayWidth { get; set; }
        public int PathDisplayHeight { get; set; }
        public int WallDisplayWidth { get; set; }
        public int WallDisplayHeight { get; set; }
        protected int MarginTop { get; set; }
        protected int MarginLeft { get; set; }

        public void DrawingMazeEx(ref Image image, Maze maze)
        {
            var mazeDisplayWidth = maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + WallDisplayWidth - 1;
            var mazeDisplayHeight = maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight - 1;

            var canvasWidth = mazeDisplayWidth + MarginLeft * 2;
            var canvasHeight = mazeDisplayHeight + MarginTop * 2;

            var bitmap = BitmapFactory.New(canvasWidth, canvasHeight);
            image.Width = canvasWidth;
            image.Height = canvasHeight;
            image.Source = bitmap;

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
    }
}