using Caliburn.Micro;
using MahApps.Metro.Controls;
using Ninject;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.Data;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : MainViewModelBase<Access>, IMainViewModel
    {
        public override string ProgramName { get { return "Strategy Builder"; } }

        public Security SelectedPickedSecurity { get; set; }
        public string SelectedBenchmark { get; set; }
        public string PartialSecurityInfo { get; set; }

        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public double InitialCash { get; set; }
        public string SelectedSecurityUniverseType { get; set; }
        public string SelectedMarket { get; set; }
        public Condition SelectedMarketSecuritiesCondition { get; set; }

        public bool IsTradingWholeMarketSecurities { get; set; }
        public bool IsTradingPickedSecurities { get; set; }

        public BindableCollection<string> Benchmarks { get; private set; }
        public BindableCollection<string> SecurityUniverseTypes { get; private set; }
        public BindableCollection<string> Markets { get; private set; }
        public BindableCollection<Security> PickedSecurities { get; private set; }
        public BindableCollection<Condition> MarketSecuritiesFilters { get; private set; }

        public bool IsRunTestViewActive { get; set; }

        [Inject]
        public SelectViewModel SelectViewModel { get; set; }
        [Inject]
        public IEnterSetupViewModel EnterSetup { get; set; }
        [Inject]
        public ICreateConditionViewModel CreateCondition { get; set; }
        [Inject]
        public IRunTestViewModel RunTest { get; set; }

        private readonly SettingManager settingManager = new SettingManager();

        public MainViewModel(IDataAccessFactory<Access> dataAccessFactory)
            : base(dataAccessFactory)
        {
            Benchmarks = new BindableCollection<string> { "S&P 500", "NASDAQ", "RUSSELL 2000" };
            SecurityUniverseTypes = new BindableCollection<string> { "Whole Market", "Selected Securities", };
            Markets = new BindableCollection<string> { "US" };
            PickedSecurities = new BindableCollection<Security>();
            MarketSecuritiesFilters = new BindableCollection<Condition>();

            SelectedSecurityUniverseType = SecurityUniverseTypes[0];
            SelectedMarket = Markets[0];
        }

        protected override void OnLoaded(MetroWindow view)
        {
            ViewService.Window = view;
            EnterSetup.ViewService = ViewService;
            RunTest.ViewService = ViewService;

            LoadSettings();
        }

        public void OnSelectedSecurityUniverseTypeChanged()
        {
            switch (SelectedSecurityUniverseType)
            {
                case "Whole Market":
                    IsTradingWholeMarketSecurities = true;
                    IsTradingPickedSecurities = false;
                    return;
                default:
                    IsTradingWholeMarketSecurities = false;
                    IsTradingPickedSecurities = true;
                    return;
            }
        }

        public void OnIsRunTestViewActiveChanged()
        {
            if (IsRunTestViewActive)
            {
                RunTest.StartTime = StartTime;
                RunTest.EndTime = EndTime;
            }
        }

        private void LoadSettings()
        {
            settingManager.LoadSettings(this);
        }

        public async void SearchBySecurityCode(ActionExecutionContext context)
        {
            var args = context.EventArgs as KeyEventArgs;
            if (args != null && args.Key == Key.Enter)
            {
                try
                {
                    List<Security> securities;
                    using (var access = DataAccessFactory.New())
                    {
                        securities = access.FindSecurity(PartialSecurityInfo);
                    }
                    if (securities.Count == 1)
                    {
                        PickedSecurities.Add(securities[0]);
                    }
                    else if (securities.Count > 0)
                    {
                        SelectViewModel.Initialize(securities.ToArray());
                        await ViewService.ShowDialog(SelectViewModel);
                        if (SelectViewModel.SelectedSecurity != null)
                            PickedSecurities.Add(SelectViewModel.SelectedSecurity);
                    }
                    else
                    {
                        await ViewService.ShowWarning("No security is found.");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void ModifySelectedSecurities(ActionExecutionContext context)
        {
            var args = context.EventArgs as KeyEventArgs;
            if (args != null && args.Key == Key.Delete && SelectedPickedSecurity != null)
            {
                PickedSecurities.Remove(SelectedPickedSecurity);
            }
        }

        public async void AddSecurityFilter()
        {
            var r = await ViewService.ShowDialog(CreateCondition);
            if (r != null && (bool)r)
            {
                var condition = CreateCondition.To();
                if (MarketSecuritiesFilters.Contains(condition))
                {
                    await ViewService.ShowWarning("Condition already exists!");
                }
                MarketSecuritiesFilters.Add(condition);
            }
        }

        public void RemoveSecurityFilter()
        {
            if (SelectedMarketSecuritiesCondition != null)
                MarketSecuritiesFilters.Remove(SelectedMarketSecuritiesCondition);
        }

        public void RemoveAllSecurityFilters()
        {
            MarketSecuritiesFilters.Clear();
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);

            settingManager.ToSave("SelectedBenchmark", SelectedBenchmark)
                .ToSave("SelectedSecurityUniverseType", SelectedSecurityUniverseType)
                .ToSave("StartTime", StartTime.ToString())
                .ToSave("EndTime", EndTime.ToString())
                .ToSave("InitialCash", InitialCash.ToString())
                .ToSave("SelectedMarket", SelectedMarket)
                .ToXml("MarketSecuritiesFilters", MarketSecuritiesFilters.ToList());

            settingManager.Save();
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}