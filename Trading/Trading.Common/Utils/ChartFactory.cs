using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Caliburn.Micro;
using Infragistics.Controls.Charts;
using Trading.Common.Entities;

namespace Trading.Common.Utils
{
    public class ChartFactory
    {
        private XamDataChart chart;

        private Dictionary<string, Series> seriesCollection = new Dictionary<string, Series>();
        private Axis xAxis;
        private Axis yAxis;

        public ChartFactory(Window window)
        {
            View = window;
            chart = window.FindChild<XamDataChart>("DataChart");
            chart.ThrowIfNull("chart", "Window must contains a child XamDataChart with name DataChart.");
            xAxis = chart.Axes.First(axis => !axis.IsVertical);
            yAxis = chart.Axes.First(axis => axis.IsVertical);
        }


        public Window View { get; private set; }

        public void AddMainPriceSeries(BindableCollection<Price> inputs, string seriesName, string valueMemberPath, string title = "")
        {
            if (seriesCollection.ContainsKey(seriesName))
                return;

            if (chart != null && !inputs.IsNullOrEmpty())
            {
                var series = new FinancialPriceSeries
                {
                    OpenMemberPath = "Open",
                    HighMemberPath = "High",
                    LowMemberPath = "Low",
                    CloseMemberPath = "Close",
                    VolumeMemberPath = "Volume",
                    DisplayType = PriceDisplayType.Candlestick,
                    XAxis = (CategoryDateTimeXAxis)xAxis,
                    YAxis = (NumericYAxis)yAxis,
                    Title = title,
                    Thickness = 1,
                };

                series.XAxis.ItemsSource = inputs;
                series.ItemsSource = inputs;

                chart.Series.Insert(0, series);

                seriesCollection[seriesName] = series;
            }
        }

        //public void AddMainPriceSeries(BindableCollection<Price> inputs, string seriesName, string valueMemberPath, string title = "")
        //{
        //    if (seriesCollection.ContainsKey(seriesName))
        //        return;

        //    if (chart != null && !inputs.IsNullOrEmpty())
        //    {
        //        var series = new FinancialPriceSeries
        //        {
        //            OpenMemberPath = "Open",
        //            HighMemberPath = "High",
        //            LowMemberPath = "Low",
        //            CloseMemberPath = "Close",
        //            VolumeMemberPath = "Volume",
        //            DisplayType = PriceDisplayType.Candlestick,
        //            XAxis = (CategoryDateTimeXAxis)xAxis,
        //            YAxis = (NumericYAxis)yAxis,
        //            Title = title,
        //            Thickness = 1,
        //        };

        //        series.XAxis.ItemsSource = inputs;
        //        series.ItemsSource = inputs;

        //        chart.Series.Insert(0, series);

        //        seriesCollection[seriesName] = series;
        //    }
        //}

        public bool RemoveSeries(string code)
        {
            return chart.Series.Remove(seriesCollection[code]);
        }
    }
}