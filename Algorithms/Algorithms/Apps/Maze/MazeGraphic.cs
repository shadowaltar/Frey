using System;
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
            PathDisplayWidth = 9;
            PathDisplayHeight = 9;
            WallDisplayWidth = 1;
            WallDisplayHeight = 1;
        }

        public double MazeDisplayWidth { get; set; }
        public double MazeDisplayHeight { get; set; }
        public double PathDisplayWidth { get; set; }
        public double PathDisplayHeight { get; set; }
        public double WallDisplayWidth { get; set; }
        public double WallDisplayHeight { get; set; }

        public void DrawMaze(Maze maze)
        {
            var totalDisplayWidth = maze.MazeWidth * PathDisplayWidth + (maze.MazeWidth + 1) * WallDisplayWidth;
            var totalDisplayHeight = maze.MazeHeight * PathDisplayHeight + (maze.MazeHeight + 1) * WallDisplayHeight;
            var leftOffset = 5;
            var topOffset = 5;

            graphics.SetCanvasSize(totalDisplayWidth + leftOffset * 2, totalDisplayHeight + topOffset * 2);
            graphics.DrawRectangle(leftOffset, topOffset, totalDisplayWidth, totalDisplayHeight);

            foreach (var wall in maze.Walls)
            {
                if (wall.IsHorizontal)
                {
                    var x1 = wall.X1 * (PathDisplayWidth + WallDisplayWidth) + leftOffset;
                    var x2 = x1 + PathDisplayWidth + WallDisplayWidth;
                    var y = (wall.Y1 + 1) * (PathDisplayHeight + WallDisplayHeight) + topOffset + WallDisplayHeight / 2;
                    graphics.DrawLine(x1, y, x2, y);
                }
                else
                {
                    var x = (wall.X1 + 1) * (PathDisplayWidth + WallDisplayWidth) + leftOffset + WallDisplayWidth / 2;
                    var y1 = wall.Y1 * (PathDisplayHeight + WallDisplayHeight) + topOffset;
                    var y2 = y1 + PathDisplayHeight + WallDisplayHeight;
                    graphics.DrawLine(x, y1, x, y2);
                }
            }
        }

        public void DrawMazeEx(ref Image image, Maze maze)
        {
            var totalDisplayWidth = maze.MazeWidth * (PathDisplayWidth + WallDisplayWidth) + WallDisplayWidth - 1;
            var totalDisplayHeight = maze.MazeHeight * (PathDisplayHeight + WallDisplayHeight) + WallDisplayHeight - 1;
            var leftOffset = 5;
            var topOffset = 5;

            var canvasWidth = totalDisplayWidth + leftOffset*2;
            var canvasHeight = totalDisplayHeight + topOffset * 2;

            var bitmap = BitmapFactory.New((int)canvasWidth, (int)canvasHeight);
            image.Width = canvasWidth;
            image.Height = canvasHeight;
            image.Source = bitmap;

            graphics.DrawRectangle(bitmap, leftOffset, topOffset, (int)totalDisplayWidth, (int)totalDisplayHeight);

            foreach (var wall in maze.Walls)
            {
                if (wall.IsHorizontal)
                {
                    var x1 = (wall.X1 + 1) * WallDisplayWidth + wall.X1 * PathDisplayWidth + leftOffset - 1;
                    var x2 = x1 + PathDisplayWidth + WallDisplayWidth;
                    var y = (wall.Y1 + 2) * WallDisplayHeight + (wall.Y1 + 1) * PathDisplayHeight + topOffset - 1;
                    graphics.DrawLine(bitmap, (int)x1, (int)y, (int)x2, (int)y);
                }
                else
                {
                    var y1 = (wall.Y1 + 1) * WallDisplayHeight + wall.Y1 * PathDisplayHeight + topOffset - 1;
                    var y2 = y1 + PathDisplayHeight + WallDisplayHeight;
                    var x = (wall.X1 + 2) * WallDisplayWidth + (wall.X1 + 1) * PathDisplayWidth + leftOffset - 1;
                    graphics.DrawLine(bitmap, (int)x, (int)y1, (int)x, (int)y2);
                }
            }
        }
    }
}