using System.Windows;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.TradeWatch.Views;
using Trading.TradeWatch.Views.Controls;

namespace Trading.TradeWatch.ViewModels
{
    public partial class MainViewModel
    {
        private Point mainPricePanOffset;
        public Point MainPricePanOffset
        {
            get { return mainPricePanOffset; }
            set { SetNotify(ref mainPricePanOffset, value); }
        }
        private Size mainPriceChartZoom;
        public Size MainPriceChartZoom
        {
            get { return mainPriceChartZoom; }
            set { SetNotify(ref mainPriceChartZoom, value); }
        }

        private void AddSecurityToChart(List<Price> list)
        {
            Prices.ClearAndAddRange(list);

            if (prices.Count > 0)
            {
                SecurityCurrentPrice = prices.Last().Close.ConvertString();
            }
            else
            {
                SecurityCurrentPrice = "N/A";
            }

            IsSecurityLoaded = true;
        }

        /// <summary>
        /// add fake order book data here.
        /// </summary>
        private void AddOrderBookData()
        {
            OrderBookPriceVolumes.Clear();

            var currentLastPrice = prices.Last().Close;
            var bid1 = currentLastPrice - 0.05m;
            var bid2 = bid1 - 0.05m;
            var bid3 = bid2 - 0.05m;
            var ask1 = currentLastPrice + 0.05m;
            var ask2 = ask1 + 0.05m;
            var ask3 = ask2 + 0.05m;

            OrderBookPriceVolumes.Add(new VolumeValue(bid3, 300));
            OrderBookPriceVolumes.Add(new VolumeValue(bid2, 289));
            OrderBookPriceVolumes.Add(new VolumeValue(bid1, 310));
            OrderBookPriceVolumes.Add(new VolumeValue(currentLastPrice, 0));
            OrderBookPriceVolumes.Add(new VolumeValue(ask1, 281));
            OrderBookPriceVolumes.Add(new VolumeValue(ask2, 275));
            OrderBookPriceVolumes.Add(new VolumeValue(ask3, 320));
        }
    }
}