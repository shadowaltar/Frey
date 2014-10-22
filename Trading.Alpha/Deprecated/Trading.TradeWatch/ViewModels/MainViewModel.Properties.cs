using Caliburn.Micro;
using System.Collections.Generic;
using Trading.Common.Entities;
using Trading.TradeWatch.ViewModels.Entities;

namespace Trading.TradeWatch.ViewModels
{
    public partial class MainViewModel
    {
        private readonly BindableCollection<Price> prices = new BindableCollection<Price>();
        public BindableCollection<Price> Prices { get { return prices; } }

        private readonly BindableCollection<TimeValue> mainPriceIndicatorValues = new BindableCollection<TimeValue>();
        public BindableCollection<TimeValue> MainPriceIndicatorValues { get { return mainPriceIndicatorValues; } }

        private readonly BindableCollection<Price> secondSecurityPrices = new BindableCollection<Price>();
        public BindableCollection<Price> SecondSecurityPrices { get { return secondSecurityPrices; } }

        private readonly BindableCollection<Price> thirdSecurityPrices = new BindableCollection<Price>();
        public BindableCollection<Price> ThirdSecurityPrices { get { return thirdSecurityPrices; } }

        private readonly BindableCollection<VolumeValue> orderBookPriceVolumes = new BindableCollection<VolumeValue>();
        public BindableCollection<VolumeValue> OrderBookPriceVolumes { get { return orderBookPriceVolumes; } }

        private readonly BindableCollection<HoldingPerformance> holdingsPerformances = new BindableCollection<HoldingPerformance>();
        public BindableCollection<HoldingPerformance> HoldingsPerformances { get { return holdingsPerformances; } }

        private readonly BindableCollection<KeyValuePair<string, decimal>> portfolioPerformanceItems = new BindableCollection<KeyValuePair<string, decimal>>();
        public BindableCollection<KeyValuePair<string, decimal>> PortfolioPerformanceItems { get { return portfolioPerformanceItems; } }

        private readonly Dictionary<long, Market> allMarkets = new Dictionary<long, Market>();
        private readonly Dictionary<string, Security> allSecurities = new Dictionary<string, Security>();

        private readonly BindableCollection<SecurityViewModel> securities = new BindableCollection<SecurityViewModel>();
        public BindableCollection<SecurityViewModel> Securities { get { return securities; } }

        private readonly BindableCollection<SecurityViewModel> securitySearches = new BindableCollection<SecurityViewModel>();
        public BindableCollection<SecurityViewModel> SecuritySearches { get { return securitySearches; } }

        public IFilterFlyoutViewModel FilterFlyout { get; set; }

        private bool isSecurityLoaded;
        public bool IsSecurityLoaded
        {
            get { return isSecurityLoaded; }
            set { SetNotify(ref isSecurityLoaded, value); }
        }

        private string securityCode;
        public string SecurityCode
        {
            get { return securityCode; }
            set { SetNotify(ref securityCode, value); }
        }

        private string securityCurrentPrice;
        public string SecurityCurrentPrice
        {
            get { return securityCurrentPrice; }
            set { SetNotify(ref securityCurrentPrice, value); }
        }

        private string secondSecurityCode;
        public string SecondSecurityCode
        {
            get { return secondSecurityCode; }
            set { SetNotify(ref secondSecurityCode, value); }
        }

        private string secondSecurityCurrentPrice;
        public string SecondSecurityCurrentPrice
        {
            get { return secondSecurityCurrentPrice; }
            set { SetNotify(ref secondSecurityCurrentPrice, value); }
        }

        private string thirdSecurityCode;
        public string ThirdSecurityCode
        {
            get { return thirdSecurityCode; }
            set { SetNotify(ref thirdSecurityCode, value); }
        }

        private string thirdSecurityCurrentPrice;
        public string ThirdSecurityCurrentPrice
        {
            get { return thirdSecurityCurrentPrice; }
            set { SetNotify(ref thirdSecurityCurrentPrice, value); }
        }

        private SecurityViewModel selectedSecurity;
        public SecurityViewModel SelectedSecurity
        {
            get { return selectedSecurity; }
            set { SetNotify(ref selectedSecurity, value); }
        }
    }
}