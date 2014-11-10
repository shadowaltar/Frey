using System;
using System.Collections.Generic;
using System.Windows.Input;
using Caliburn.Micro;
using MahApps.Metro.Controls;
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

        public MainViewModel(IDataAccessFactory<Access> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            Benchmarks = new BindableCollection<string> { "S&P", "RUSSELL 2000" };
            SecurityUniverseTypes = new BindableCollection<string> { "Whole Market", "Selected Securities" };
            Markets = new BindableCollection<string> { "US" };
            Constants.InitializeDirectories();
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