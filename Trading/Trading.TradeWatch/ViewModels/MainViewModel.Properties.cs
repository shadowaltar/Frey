using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.TradeWatch.ViewModels.Entities;
using System.Collections.ObjectModel;

namespace Trading.TradeWatch.ViewModels
{
    public partial class MainViewModel
    {

        private readonly ObservableCollection<Price> prices = new ObservableCollection<Price>();
        public ObservableCollection<Price> Prices { get { return prices; } }

        private readonly Dictionary<long, Market> allMarkets = new Dictionary<long, Market>();
        private Dictionary<long, Security> allSecurities = new Dictionary<long, Security>();

        private readonly BindableCollection<SecurityViewModel> securities = new BindableCollection<SecurityViewModel>();
        public BindableCollection<SecurityViewModel> Securities { get { return securities; } }

        public IFilterFlyoutViewModel FilterFlyout { get; set; }

        private bool isSecurityLoaded;
        public bool IsSecurityLoaded
        {
            get { return isSecurityLoaded; }
            set { SetNotify(ref isSecurityLoaded, value); }
        }

        private SecurityViewModel selectedSecurity;
        public SecurityViewModel SelectedSecurity
        {
            get { return selectedSecurity; }
            set { SetNotify(ref selectedSecurity, value); }
        }
    }
}