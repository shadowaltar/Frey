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

namespace Charting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var w = Canvas.ActualWidth;
            var h = Canvas.ActualHeight;

            Charter.LineStroke=new SolidColorBrush(Colors.Red);
            Canvas.DrawLine(0, 25, 50, 50);
            Canvas.DrawRectangle(120, 25, 50, 100);
        }
    }
}
