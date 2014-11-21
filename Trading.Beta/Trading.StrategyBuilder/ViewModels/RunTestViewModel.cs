using Ninject;
using PropertyChanged;
using System.Collections.Generic;
using Trading.Common.Data;
using Trading.Common.Entities;
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

        public void SetDataCriteria(int start, int end, Condition dataCondition)
        {
            DataCriteria = new DataCriteria(dataCondition)
            {
                Start = start.FromDateInt(),
                End = end.FromDateInt()
            };
        }

        public void TestGetPricesByDate()
        {
            using (ReportTime.Start())
            {
                Dictionary<long, Price> securitiesOfDay;
                if (Database.Prices.TryGet(TestDate, out securitiesOfDay))
                {
                    ViewService.ShowMessage("INFO", securitiesOfDay.Count + " records retrieved.");
                }
            }
        }

        public void TestGetPricesBySecId()
        {
            using (ReportTime.Start())
            {
                Security sec;
                if (!Database.Securities.TryGetValue(TestSecId, out sec))
                {
                    ViewService.ShowWarning("Security not found.");
                    return;
                }

                Dictionary<int, Price> oneSecurity;
                if (Database.Prices.TryGet(TestSecId, out oneSecurity))
                {
                    ViewService.ShowMessage("INFO", oneSecurity.Count + " records retrieved.");
                }
            }
        }
    }

    public interface IRunTestViewModel : IHasViewService, IHasDataAccessFactory<Access>
    {
        void Initialize();
        void SetDataCriteria(int start, int end, Condition dataCondition);
    }
}