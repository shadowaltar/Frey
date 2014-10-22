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
using MahApps.Metro.Controls;
using Telerik.Windows.Controls.ChartView;
using Trading.Common.Entities;

namespace Trading.TradeWatch.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();

            //var s = Chart.Series[0] as CandlestickSeries;
            //s.RenderOptions = new Direct2DRenderOptions();

        }

        private void ChartTrackBallBehavior_OnTrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            var dpi = e.Context.ClosestDataPoint;
            if (dpi != null)
            {
                var data = dpi.DataPoint.DataItem as Price;
                if (data != null)
                {
                    MainPriceChartOpen.Text = data.Open.ToString("F");
                    MainPriceChartHigh.Text = data.High.ToString("F");
                    MainPriceChartLow.Text = data.Low.ToString("F");
                    MainPriceChartClose.Text = data.Close.ToString("F");
                    MainPriceChartVolume.Text = data.Volume.ToString("F");

                }
            }
        }
    }
}
