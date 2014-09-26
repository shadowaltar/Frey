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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Algorithms.Randoms;

namespace Charting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Charter charter;

        public MainWindow()
        {
            InitializeComponent();
            charter = new Charter(Canvas);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            charter.CanvasWidth = 400;
            charter.CanvasHeight = 400;

            int n = 200;
            for (int i = 0; i < n; i++)
            {
                double x = ThreadSafeRandom.Next(0, 400);
                double y = x/400.0*300;
                double rh = 300 - y;
                charter.DrawLine(x, 300, x, rh);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            charter.Clear();
        }
    }
}
