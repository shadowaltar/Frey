using System;
using System.Collections.Generic;
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

        public DataCriteria DataCriteria { get; set; }

        public long TestSecId { get; set; }
        public int TestDate { get; set; }

        private Context context;

        public async void Initialize()
        {
            context = new Context();
            context.Initialize(DataCriteria);
            context.DataAccessFactory = DataAccessFactory;

            var progress = await ViewService.ShowProgress("Loading", "Loading data from db.");
            await context.Prepare();
            await progress.Stop();
        }

        public void SetDataCriteria(DateTime start, DateTime end, IEnumerable<Condition> dataConditions)
        {
            DataCriteria = new DataCriteria { Start = start, End = end };

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
        void Initialize();
        void SetDataCriteria(DateTime start, DateTime end, IEnumerable<Condition> dataConditions);
    }
}