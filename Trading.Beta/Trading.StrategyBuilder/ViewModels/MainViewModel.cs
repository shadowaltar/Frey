using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Ninject;
using PropertyChanged;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Data;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public partial class MainViewModel : MainViewModelBase<Access>, IMainViewModel
    {
        public override string ProgramName { get { return "Strategy Builder"; } }

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

        public bool IsTradingSelectedMarket { get; set; }
        public bool IsTradingSelectiveSecurities { get; set; }

        public BindableCollection<string> Benchmarks { get; private set; }
        public BindableCollection<string> SecurityUniverseTypes { get; private set; }
        public BindableCollection<string> Markets { get; private set; }
        public BindableCollection<string> SelectedSecurities { get; private set; }

        public MainViewModel(IDataAccessFactory<Access> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            Benchmarks = new BindableCollection<string> { "S&P", "RUSSELL 2000" };
            SecurityUniverseTypes = new BindableCollection<string> { "Selected Securities", "Whole Market"};
            Markets = new BindableCollection<string> { "US" };
            SelectedSecurities = new BindableCollection<string>();
            Constants.InitializeDirectories();

            SelectedSecurityUniverseType = SecurityUniverseTypes[0];
        }

        protected override void OnLoaded(MetroWindow view)
        {
            ViewService.Window = view;
        }

        private void ShowHideSecurityUniverse(string value)
        {
            switch (value)
            {
                case "Whole Market":
                    IsTradingSelectedMarket = true;
                    IsTradingSelectiveSecurities = false;
                    return;
                default:
                    IsTradingSelectedMarket = false;
                    IsTradingSelectiveSecurities = true;
                    return;
            }
        }

        public void SearchBySecurityCode(ActionExecutionContext context)
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

                    }
                    else if (securities.Count > 0)
                    {
                        ViewService.ShowDialog(SelectViewModel);
                        SelectViewModel.Add(securities.ToArray());
                    }
                    else
                    {
                        ViewService.ShowWarning("No security is found.");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}