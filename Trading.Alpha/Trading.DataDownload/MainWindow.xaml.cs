using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using Trading.Common.Services;
using Path = System.IO.Path;
using System.Windows;
using System.Windows.Shapes;

namespace Trading.DataDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string PricesDirectory
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "Prices");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(PricesDirectory))
            {
                Directory.CreateDirectory(PricesDirectory);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var downloader = new YahooDailyPriceWorker();
            var symbol = "QQQ";

            Task.Run(() =>
                downloader.Download(symbol, Path.Combine(PricesDirectory, symbol + ".csv")));
        }
    }
}
