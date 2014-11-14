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
using Trading.StrategyBuilder.Data;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public partial class MainViewModel : MainViewModelBase<Access>, IMainViewModel
    {
        public override string ProgramName { get { return "Strategy Builder"; } }

        public Security SelectedPickedSecurity { get; set; }
        public string SelectedBenchmark { get; set; }
        public string PartialSecurityInfo { get; set; }

        [Inject]
        public SelectViewModel SelectViewModel { get; set; }

        private string selectedSecurityUniverseType;
        public string SelectedSecurityUniverseType
        {
            get { return selectedSecurityUniverseType; }
            set
            {
                selectedSecurityUniverseType = value;
                ShowHideSecurityUniverse(value);
            }
        }

        public bool IsTradingWholeMarketSecurities { get; set; }
        public bool IsTradingPickedSecurities { get; set; }

        public BindableCollection<string> Benchmarks { get; private set; }
        public BindableCollection<string> SecurityUniverseTypes { get; private set; }
        public BindableCollection<string> Markets { get; private set; }
        public BindableCollection<Security> PickedSecurities { get; private set; }

        public IEnterSetupViewModel EnterSetup { get; set; }

        public MainViewModel(IEnterSetupViewModel enterSetup,
            IDataAccessFactory<Access> dataAccessFactory)
            : base(dataAccessFactory)
        {
            Benchmarks = new BindableCollection<string> { "S&P", "RUSSELL 2000" };
            SecurityUniverseTypes = new BindableCollection<string> { "Selected Securities", "Whole Market" };
            Markets = new BindableCollection<string> { "US" };
            PickedSecurities = new BindableCollection<Security>();

            EnterSetup = enterSetup;

            SelectedSecurityUniverseType = SecurityUniverseTypes[0];
        }

        protected override void OnLoaded(MetroWindow view)
        {
            ViewService.Window = view;
            EnterSetup.ViewService = ViewService;
        }

        private void ShowHideSecurityUniverse(string value)
        {
            switch (value)
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
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}