using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Algorithms.Apps.Maze
{
    /// <summary>
    /// Interaction logic for MazeView.xaml
    /// </summary>
    public partial class MazeView : Window
    {
        private Graphics graphics;

        public MazeView()
        {
            InitializeComponent();
            graphics = new Graphics(Canvas) { LineThickness = 1 };
        }

        public void Generate(int width, int height, int pathDisplayWidth = 5)
        {
            graphics.Clear();
            var mg = new MazeGraphics(graphics);
            var game = new Game();
            var maze = game.GenerateRectangular(width, height);

            mg.PathDisplayHeight = pathDisplayWidth;
            mg.PathDisplayWidth = pathDisplayWidth;
            mg.DrawMazeEx(ref Image, maze);
        }
    }
}
