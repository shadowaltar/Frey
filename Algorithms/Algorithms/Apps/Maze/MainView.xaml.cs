using System;
using System.Windows;
using Algorithms.Apps.Maze.Algorithms;

namespace Algorithms.Apps.Maze
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView
    {
        private MazeView mazeView;

        public MainView()
        {
            InitializeComponent();

            Algorithms.Items.Add("Recursive Backtracker");
            Algorithms.Items.Add("Randomized Kruskal");
            Algorithms.Items.Add("Randomized Prims");
            Algorithms.SelectedIndex = 0;

            Width.Text = "100";
            Height.Text = "100";
        }

        private void GenerateMaze(object sender, RoutedEventArgs routedEventArgs)
        {
            mazeView = new MazeView();
            var width = Convert.ToInt32(Width.Text);
            var height = Convert.ToInt32(Height.Text);

            switch (Algorithms.SelectedValue.ToString())
            {
                case "Randomized Kruskal":
                    mazeView.Generate<RandomizedKruskal>(width, height);
                    break;
                case "Randomized Prims":
                    mazeView.Generate<RandomizedPrims>(width, height);
                    break;
                case "Recursive Backtracker":
                    mazeView.Generate<RecursiveBacktracker>(width, height);
                    break;
            }
            mazeView.Show();
        }

        private void SolveMaze(object sender, RoutedEventArgs e)
        {
            if (mazeView != null && mazeView.IsVisible)
            {
                mazeView.Solve<AStar>();
            }
        }
    }
}
