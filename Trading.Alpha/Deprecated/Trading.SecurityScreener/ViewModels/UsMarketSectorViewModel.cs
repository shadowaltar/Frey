using System.Collections.Generic;
using System.Data;
using Caliburn.Micro;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.SecurityScreener.ViewModels
{
    public class UsMarketScreenerViewModel : ViewModelBase, IUsMarketScreenerViewModel
    {
        private readonly Dictionary<long, Market> allMarkets = new Dictionary<long, Market>();

        public UsMarketScreenerViewModel(IDataAccessFactory<SecurityScreenerDataAccess> daf)
        {
            Sectors = new BindableCollection<string>();
            DataAccessFactory = daf;
        }

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<SecurityScreenerDataAccess> DataAccessFactory { get; set; }

        public BindableCollection<string> Sectors { get; private set; }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            Load();
        }

        private void Load()
        {
            LoadMarkets();
        }

        private void LoadMarkets()
        {
            using (var access = DataAccessFactory.New())
            {
                var table = access.GetAllMarkets();
                foreach (DataRow row in table.Rows)
                {
                    var market = new Market
                    {
                        Id = row["Id"].ConvertLong(),
                        Code = row["Acronym"].ConvertString(),
                        Name = row["Name"].ConvertString(),
                    };
                    allMarkets[market.Id] = market;
                }
            }
        }
    }

    public interface IUsMarketScreenerViewModel : IHasViewService, IHasDataAccessFactory<SecurityScreenerDataAccess>
    {
    }
}