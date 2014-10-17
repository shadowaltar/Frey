using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.TradeWatch.ViewModels.Entities;
using Trading.TradeWatch.Views.Controls;

namespace Trading.TradeWatch.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<TradesScreenDataAccess>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Trade Screen"; } }

        public MainViewModel(IDataAccessFactory<TradesScreenDataAccess> dataAccessFactory,
            IFilterFlyoutViewModel filterFlyout,
            ISettings settings)
            : base(dataAccessFactory, settings)
        {
            FilterFlyout = filterFlyout;
            SecurityCode = "Security Watch";
            SecurityCurrentPrice = "";
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            Load();
        }

        public async void HandleDoubleClick()
        {
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.F: ToggleFilter(); break;
                    case Key.R: Refresh(); break;
                }
            }

            if (e.Key == Key.Escape)
            {
                CloseAllFlyouts();
            }
        }

        public void ToggleFilter()
        {
            IsSecurityLoaded = !IsSecurityLoaded;

            if (IsSecurityLoaded)
            {
                if (!FilterFlyout.IsReady)
                {
                    FilterFlyout.Markets.ClearAndAddRange(allMarkets.Values
                        .Select(m => new MarketViewModel(m)).OrderBy(m => m.Code));
                }
                FilterFlyout.IsReady = true;
            }
        }

        private void CloseAllFlyouts()
        {
            if (IsSecurityLoaded) ToggleFilter();
        }

        public void HandleSearchBoxKeys(KeyEventArgs c)
        {
            if (c.Key == Key.Enter)
            {
                Add();
            }
        }

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
        }

        public async void Add()
        {
            if (SelectedSecurity == null)
                return;

            SecurityCode = SelectedSecurity.Code;

            progress = await ViewService.ShowProgress("Loading Prices", "Loading portfolio top 5 security price histories...");
            var results = await LoadPrices(SelectedSecurity.Code, new DateTime(2006, 1, 1), new DateTime(2013, 1, 1));

            if (results != null)
            {
                AddSecurityToChart(results);
                var averages = CalculateMovingAverage(results, 15);
                AddMovingAverageToChart(averages);
                AddOrderBookData();
            }

            // fake selections

            var code = "0003.HK";
            results = await LoadPrices(code, new DateTime(2006, 1, 1), new DateTime(2013, 1, 1));
            if (results != null)
            {
                SecondSecurityCode = code;

                SecondSecurityPrices.ClearAndAddRange(results);

                SecondSecurityCurrentPrice = SecondSecurityPrices.Count > 0
                    ? SecondSecurityPrices.Last().Close.ConvertString() : "N/A";
            }

            code = "0004.HK";
            results = await LoadPrices("0004.HK", new DateTime(2006, 1, 1), new DateTime(2013, 1, 1));
            if (results != null)
            {
                ThirdSecurityCode = code;

                ThirdSecurityPrices.ClearAndAddRange(results);

                ThirdSecurityCurrentPrice = ThirdSecurityPrices.Count > 0
                    ? ThirdSecurityPrices.Last().Close.ConvertString() : "N/A";
            }

            await progress.Stop();

        }

        private List<TimeValue> CalculateMovingAverage(List<Price> inputs, int periods)
        {
            var results = new List<TimeValue>();
            var queue = new Queue<decimal>();
            foreach (var result in inputs.Select(x => new { x.Close, x.At }))
            {
                queue.Enqueue(result.Close);
                if (queue.Count == periods)
                {
                    results.Add(new TimeValue(queue.Average(), result.At));
                    queue.Dequeue();
                }
                else
                {
                    results.Add(new TimeValue(result.Close, result.At));
                }
            }
            return results;
        }

        private List<TimeValue> CalculateStochasticMovingAverage(List<Price> inputs, int periods)
        {
            var results = new List<TimeValue>();
            var queue = new Queue<decimal>();
            foreach (var result in inputs.Select(x => new { x.Close, x.At }))
            {
                queue.Enqueue(result.Close);
                if (queue.Count == periods)
                {
                    results.Add(new TimeValue(queue.Average(), result.At));
                    queue.Dequeue();
                }
                else
                {
                    results.Add(new TimeValue(result.Close, result.At));
                }
            }
            return results;
        }

        private void AddMovingAverageToChart(List<TimeValue> values)
        {
            MainPriceIndicatorValues.ClearAndAddRange(values);
        }

        public void Remove(string code)
        {
        }

        public void About()
        {
            ViewService.ShowMessage("About Portfolio Holdings Dashboard", "Demo application written by Mars Wang");
        }
    }
}