using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abt.Controls.SciChart;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.SecurityScreener.ViewModels
{
    public class MainViewModel : MainViewModelBase<SecurityScreenerDataAccess>, IMainViewModel
    {
        public MainViewModel(IDataAccessFactory<SecurityScreenerDataAccess> factory,
            IUsMarketScreenerViewModel usMarketScreener,
            ISettings settings)
            : base(factory, settings)
        {
            ChartPaneViewModels = new BindableCollection<ChartPaneBaseViewModel>();

            UsMarketScreener = usMarketScreener;

            DataAccessFactory = factory;
            UsMarketScreener.DataAccessFactory = factory;
        }

        public override string ProgramName { get { return "Security Screener"; } }
        public IUsMarketScreenerViewModel UsMarketScreener { get; private set; }
        private IViewportManager viewportManager;
        private bool _isZoomEnabled;
        private bool _isPanEnabled;
        private string _verticalChartGroupId;
        private IndexRange _xAxisVisibleRange;
        public IViewportManager ViewportManager { get { return viewportManager; } }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = view as MetroWindow;
            UsMarketScreener.ViewService = ViewService;

            var progress = await ViewService.ShowProgress("Loading...", "...");
            viewportManager = new DefaultViewportManager();
            var data = await Load();
            await RenderCharts(data);
            await progress.Stop();
        }

        private Task RenderCharts(PriceSeries data)
        {
            IsPanEnabled = true;
            IsZoomEnabled = true;
            VerticalChartGroupId = Guid.NewGuid().ToString();
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var task = new Task(() =>
            {
                ChartPaneViewModels.Add(new PricePaneViewModel(this, data) { IsFirstChartPane = true });
                ChartPaneViewModels.Add(new MacdPaneViewModel(this, data) { Title = "MACD" });
                ChartPaneViewModels.Add(new RsiPaneViewModel(this, data) { Title = "RSI" });
                ChartPaneViewModels.Add(new VolumePaneViewModel(this, data) { Title = "Volume" });
            });
            task.Start(scheduler);
            return task;
        }

        public Task<PriceSeries> Load()
        {
            return Task.Run(() =>
            {
                PriceSeries data;
                using (var access = DataAccessFactory.New())
                {
                    var id = access.GetSecurityId("SPY", "INDEX");
                    data = access.GetPrices(id, "2013-01-01".ConvertIsoDate(), "2014-06-30".ConvertIsoDate());
                }
                return data;
            });
        }

        public void OpenScreener()
        {
            ViewService.ShowWindow((ViewModelBase)UsMarketScreener);
        }

        public bool IsPanEnabled
        {
            get { return _isPanEnabled; }
            set { SetNotify(ref _isPanEnabled, value); }
        }

        public bool IsZoomEnabled
        {
            get { return _isZoomEnabled; }
            set { SetNotify(ref _isZoomEnabled, value); }
        }

        public string VerticalChartGroupId
        {
            get { return _verticalChartGroupId; }
            set { SetNotify(ref _verticalChartGroupId, value); }
        }

        /// <summary>
        /// Shared XAxis VisibleRange for all charts
        /// </summary>
        public IndexRange XVisibleRange
        {
            get { return _xAxisVisibleRange; }
            set { SetNotify(ref _xAxisVisibleRange, value); }
        }

        public BindableCollection<ChartPaneBaseViewModel> ChartPaneViewModels { get; private set; }
    }
}