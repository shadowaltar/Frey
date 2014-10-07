using Caliburn.Micro;
using Trading.Common.ViewModels;
using Trading.TradeWatch.ViewModels.Entities;

namespace Trading.TradeWatch.ViewModels
{
    public class FilterFlyoutViewModel : FilterViewModelBase<IMainViewModel>, IFilterFlyoutViewModel
    {
        public bool IsReady { get; set; }

        private readonly BindableCollection<MarketViewModel> markets = new BindableCollection<MarketViewModel>();
        public BindableCollection<MarketViewModel> Markets { get { return markets; } }


        private MarketViewModel selectedMarket;
        public MarketViewModel SelectedMarket
        {
            get { return selectedMarket; }
            set
            {
                if (SetNotify(ref selectedMarket, value))
                {
                    Filter("Market", value.Code);
                }
            }
        }


        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                if (SetNotify(ref id, value) && IsReady) // empty string is useful here
                    Filter("StartOfId", Id);
            }
        }

        private string code;
        public string Code
        {
            get { return code; }
            set
            {
                if (SetNotify(ref code, value) && IsReady) // empty string is useful here
                    Filter("StartOfCode", Code);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetNotify(ref name, value) && IsReady) // empty string is useful here
                    Filter("StartOfName", Name);
            }
        }

        private string portfolioManagerName;
        public string PortfolioManagerName
        {
            get { return portfolioManagerName; }
            set
            {
                if (SetNotify(ref portfolioManagerName, value) && IsReady) // empty string is useful here
                    Filter("StartOfPmName", value);
            }
        }

        private string benchmarkCode;
        public string BenchmarkCode
        {
            get { return benchmarkCode; }
            set
            {
                if (SetNotify(ref benchmarkCode, value) && IsReady) // empty string is useful here
                    Filter("StartOfBmCode", value);
            }
        }

        public void ClearAllFields()
        {
            Id = string.Empty;
            Code = string.Empty;
            Name = string.Empty;
            PortfolioManagerName = string.Empty;
            BenchmarkCode = string.Empty;
            CurrentOptions.Clear();
        }

        /// <summary>
        /// Clear all fields.
        /// </summary>
        public void Reset()
        {
            ClearAllFields();
            ResetFilterTarget();
        }
    }
}