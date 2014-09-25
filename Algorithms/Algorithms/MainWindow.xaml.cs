using Algorithms.Algos;
using System;
using System.Windows;

namespace Algorithms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double pDouble { get { return Convert.ToDouble(P.Text); } }
        private double qDouble { get { return Convert.ToDouble(Q.Text); } }
        private int pInteger { get { return Convert.ToInt32(P.Text); } }
        private int qInteger { get { return Convert.ToInt32(Q.Text); } }

        private void ShowAnswer(long[] x)
        {
            var result = "";
            foreach (var l in x)
            {
                result += (l + ",");
            }
            MessageBox.Show(this, result);
        }

        private void ShowAnswer(string x)
        {
            MessageBox.Show(this, x);
        }

        private void ShowAnswer(long x)
        {
            MessageBox.Show(this, x.ToString());
        }

        private void ShowAnswer(double x)
        {
            MessageBox.Show(this, x.ToString());
        }

        private void GreatestCommonDivider_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(Arithmetics.GreatestCommonDivisor(pInteger, qInteger));
        }

        private void SquareRoot_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(pDouble.SquareRoot());
        }

        private void Hypotenuse_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(pDouble.Hypotenuse(qDouble));
        }

        private void HarmonicNumber_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(pInteger.Harmonics());
        }

        private void ToBinaryFormat_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(Formats.ToBinary(pInteger));
        }

        private void ToHexFormat_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(Formats.ToHexidecimal(pInteger));
        }

        private void FibonacciNumbers_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(Series.GetFibonacciNumbers(pInteger));
        }

        private void FibonacciNumberAt_Click(object sender, RoutedEventArgs e)
        {
            ShowAnswer(Series.GetFibonacciNumberAt(pInteger));
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            Tests.Test(pDouble, qDouble);
        }
    }
}
