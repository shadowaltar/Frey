using Trading.Common.Entities;
using Trading.Common.ViewModels;

namespace Trading.SecurityResearch.ViewModels.Entities
{
    public class MarketViewModel : ViewModelBaseSlim
    {
        public Market Market { get; private set; }

        public MarketViewModel(Market market)
        {
            Market = market;
        }

        public string Code
        {
            get { return Market.Code; }
            set
            {
                Market.Code = value;
                Notify();
            }
        }

        public string Name
        {
            get { return Market.Name; }
            set
            {
                Market.Name = value;
                Notify();
            }
        }
    }
}