using System;
using Ninject;
using Ninject.Activation.Caching;
using PropertyChanged;
using Trading.Common.Data;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;
using Trading.StrategyBuilder.Data;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class RunTestViewModel : ViewModelBase, IRunTestViewModel
    {
        public IViewService ViewService { get; set; }
        [Inject]
        public IDataAccessFactory<Access> DataAccessFactory { get; set; }

        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public DataCriteria DataCriteria { get; set; }

        public long TestSecId { get; set; }
        public int TestDate { get; set; }

        private Context context;

        public async void Initialize()
        {
            var start = StartTime.FromDateInt();
            var end = EndTime.FromDateInt();
            context = new Context();
            context.Initialize(start, end, null);
            context.DataAccessFactory = DataAccessFactory;

            var progress = await ViewService.ShowProgress("Loading", "Loading data from db.");
            await context.Prepare();
            await progress.Stop();
        }

        public void TestGetPricesByDate()
        {
            using (ReportTime.Start())
            {
                var byDate = Database.Prices[TestDate];
            }
        }

        public void TestGetPricesBySecId()
        {
            using (ReportTime.Start())
            {
                var byDate = Database.Prices[TestSecId];
            }
        }
    }

    public interface IRunTestViewModel : IHasViewService, IHasDataAccessFactory<Access>
    {
        int StartTime { get; set; }
        int EndTime { get; set; }
        void Initialize();
    }
}