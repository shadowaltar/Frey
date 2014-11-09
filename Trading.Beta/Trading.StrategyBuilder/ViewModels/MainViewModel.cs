using System;
using System.Diagnostics;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Ninject;
using PropertyChanged;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public partial class MainViewModel : MainViewModelBase<TradingDataAccess>, IMainViewModel
    {
        public string SelectedBenchmark { get; set; }

        public BindableCollection<string> Benchmarks { get; private set; }

        public MainViewModel(IDataAccessFactory<TradingDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
            Benchmarks = new BindableCollection<string> { "S&P", "RUSSELL 2000" };
            Constants.InitializeDirectories();
        }

        protected override void OnLoaded(MetroWindow view)
        {
            ViewService.Window = view;
        }

        public override string ProgramName { get { return "Strategy Builder"; } }

        public void Test()
        {
            Console.WriteLine(SelectedBenchmark);
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}