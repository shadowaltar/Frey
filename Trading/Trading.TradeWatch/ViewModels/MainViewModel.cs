using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Infragistics.Controls.Charts;
using MahApps.Metro.Controls;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.TradeWatch.ViewModels.Entities;

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
        }



        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Load();
            LoadPrices("0001.HK");
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

        public void Handle(ActivityMessage<IMainViewModel> message)
        {
            switch (message.Type)
            {
                case ActivityType.Filter:
                    Filter(message.Item as FilterOptions);
                    break;
            }
        }

        private void Filter(FilterOptions filterOptions)
        {
            foreach (var option in filterOptions)
            {
                var pair = option;
                switch (option.Key)
                {
                    case "Market":
                        var results = allSecurityViewModels.Where(m => m.Security.Market.Code == pair.Value).OrderBy(vm => vm.Code);
                        Securities.ClearAndAddRange(results);
                        break;
                }
            }
        }




    }
}