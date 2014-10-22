using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.SecurityResearch.ViewModels.Entities;

namespace Trading.SecurityResearch.ViewModels
{
    public partial class MainViewModel : MainViewModelBase<SecuritiesDataAccess>,
        IMainViewModel, IHandle<ActivityMessage<IMainViewModel>>
    {
        public override sealed string ProgramName { get { return "Security Research"; } }

        public MainViewModel(IDataAccessFactory<SecuritiesDataAccess> dataAccessFactory,
            IResearchReportViewModel researchReport,
            IFilterFlyoutViewModel filterFlyout,
            ISettings settings)
            : base(dataAccessFactory, settings)
        {
            FilterFlyout = filterFlyout;
            ResearchReport = researchReport;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Load();
        }

        public void About()
        {
            ViewService.ShowMessage("About " + ProgramName,
                "Version: " + Assembly.GetExecutingAssembly().GetName().Version);
        }

        public async void HandleDoubleClick()
        {
            ResearchReport.SecuritySymbol = SelectedSecurity.Code;
            ResearchReport.SecurityName = SelectedSecurity.Name;
            ResearchReport.SecurityReportDate = DateTime.Today.ToShortDateString();
            ResearchReport.CurrentPrice = SelectedSecurity.Security.CurrencySymbol +
                                          (new Random().NextDouble() * 100).ToString("###.0000");
            await ViewService.ShowDialog(ResearchReport as ViewModelBase);
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
            IsFilterFlyoutOpened = !IsFilterFlyoutOpened;

            if (IsFilterFlyoutOpened)
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
            if (IsFilterFlyoutOpened) ToggleFilter();
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