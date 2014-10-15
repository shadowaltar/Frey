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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            var graphics = new Graphics(Canvas);
            graphics.LineThickness = 1;
            var mg = new MazeGraphics(graphics);
            var game = new Game();
            var maze = game.GenerateRectangular(20, 20);
            mg.DrawMaze(maze);
        }
    }
}
