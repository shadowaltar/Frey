using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Abt.Controls.SciChart;
using Abt.Controls.SciChart.Model.DataSeries;
using Abt.Controls.SciChart.Visuals.RenderableSeries;
using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.Common.Technicals;
using Trading.Common.Technicals.Entities;
using Trading.Common.ViewModels;

namespace Trading.SecurityScreener.ViewModels
{
    public class ChartPaneBaseViewModel : ViewModelBaseSlim, IChildPane
    {
        public IMainViewModel ParentViewModel { get; set; }

        protected ChartPaneBaseViewModel(IMainViewModel parent)
        {
            ParentViewModel = parent;
        }

        private readonly BindableCollection<IChartSeriesViewModel> chartSeriesViewModels = new BindableCollection<IChartSeriesViewModel>();
        public BindableCollection<IChartSeriesViewModel> ChartSeriesViewModels { get { return chartSeriesViewModels; } }

        private bool isFirstChartPane;
        public bool IsFirstChartPane
        {
            get { return isFirstChartPane; }
            set { SetNotify(ref isFirstChartPane, value); }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { SetNotify(ref title, value); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { SetNotify(ref height, value); }
        }

        private string yAxisTextFormatting;
        public string YAxisTextFormatting
        {
            get { return yAxisTextFormatting; }
            set { SetNotify(ref yAxisTextFormatting, value); }
        }

        public void ZoomExtents()
        {
        }

        public void Close()
        {

        }
    }
    public class PricePaneViewModel : ChartPaneBaseViewModel
    {
        public PricePaneViewModel(IMainViewModel parentViewModel, PriceSeries prices)
            : base(parentViewModel)
        {
            // We can add Series via the SeriesSource API, where SciStockChart or SciChartSurface bind to IEnumerable<IChartSeriesViewModel>
            // Alternatively, you can delcare your RenderableSeries in the SciStockChart and just bind to DataSeries
            // A third method (which we don't have an example for yet, but you can try out) is to create an Attached Behaviour to transform a collection of IDataSeries into IRenderableSeries
            // 

            // Add the main OHLC chart
            var stockPrices = new OhlcDataSeries<DateTime, double>();
            stockPrices.Append(prices.TimeData, prices.OpenData, prices.HighData, prices.LowData, prices.CloseData);
            base.ChartSeriesViewModels.Add(new ChartSeriesViewModel(stockPrices, new FastCandlestickRenderableSeries() { AntiAliasing = false }));

            // Add a moving average
            var maLow = new XyDataSeries<DateTime, double>();
            maLow.Append(prices.TimeData, prices.CloseData.MovingAverage(20));
            base.ChartSeriesViewModels.Add(new ChartSeriesViewModel(maLow, new FastLineRenderableSeries() { SeriesColor = Color.FromArgb(0xFF, 0xFF, 0x33, 0x33), StrokeThickness = 2 }));

            // Add a moving average
            var maHigh = new XyDataSeries<DateTime, double>();
            maHigh.Append(prices.TimeData, prices.CloseData.MovingAverage(50));
            base.ChartSeriesViewModels.Add(new ChartSeriesViewModel(maHigh, new FastLineRenderableSeries() { SeriesColor = Color.FromArgb(0xFF, 0x33, 0xDD, 0x33), StrokeThickness = 2 }));

            YAxisTextFormatting = "$0.0000";
        }
    }

    public class VolumePaneViewModel : ChartPaneBaseViewModel
    {
        public VolumePaneViewModel(IMainViewModel parentViewModel, PriceSeries prices)
            : base(parentViewModel)
        {
            var volumePrices = new XyDataSeries<DateTime, long>();
            volumePrices.Append(prices.TimeData, prices.Select(x => x.Volume));

            ChartSeriesViewModels.Add(new ChartSeriesViewModel(volumePrices, new FastColumnRenderableSeries()));

            YAxisTextFormatting = "###E+0";
        }
    }

    public class MacdPaneViewModel : ChartPaneBaseViewModel
    {
        public MacdPaneViewModel(IMainViewModel parentViewModel, PriceSeries prices)
            : base(parentViewModel)
        {
            IEnumerable<MacdPoint> macdPoints = prices.CloseData.Macd(12, 26, 9).ToList();

            var histogramDataSeries = new XyDataSeries<DateTime, double>();
            histogramDataSeries.Append(prices.TimeData, macdPoints.Select(x => x.Divergence));
            base.ChartSeriesViewModels.Add(new ChartSeriesViewModel(histogramDataSeries, new FastColumnRenderableSeries()));

            var macdDataSeries = new XyyDataSeries<DateTime, double>();
            macdDataSeries.Append(prices.TimeData, macdPoints.Select(x => x.Macd), macdPoints.Select(x => x.Signal));
            base.ChartSeriesViewModels.Add(new ChartSeriesViewModel(macdDataSeries, new FastBandRenderableSeries() { Opacity = 0.7, StrokeThickness = 2 }));

            YAxisTextFormatting = "0.00";

            Height = 100;
        }
    }

    public class RsiPaneViewModel : ChartPaneBaseViewModel
    {
        public RsiPaneViewModel(IMainViewModel parentViewModel, PriceSeries prices)
            : base(parentViewModel)
        {
            var rsiSeries = new XyDataSeries<DateTime, double>();
            rsiSeries.Append(prices.TimeData, prices.Rsi(14));
            base.ChartSeriesViewModels.Add(new ChartSeriesViewModel(rsiSeries, new FastLineRenderableSeries()));

            YAxisTextFormatting = "0.0";

            Height = 100;
        }
    }
}