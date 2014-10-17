using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Algorithms.Apps.Maze
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private MazeView mazeView;

        public MainView()
        {
            InitializeComponent();
        }

        private void GenerateMaze(object sender, RoutedEventArgs routedEventArgs)
        {
            mazeView = new MazeView();
            mazeView.Generate(Convert.ToInt32(Width.Text), Convert.ToInt32(Height.Text), 4);
            mazeView.ShowDialog();
        }
    }
}
