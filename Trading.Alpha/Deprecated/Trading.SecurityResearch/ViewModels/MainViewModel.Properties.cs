using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.SecurityResearch.ViewModels.Entities;

namespace Trading.SecurityResearch.ViewModels
{
    public partial class MainViewModel
    {
        private readonly Dictionary<long, Market> allMarkets = new Dictionary<long, Market>();
        private ISet<SecurityViewModel> allSecurityViewModels = new HashSet<SecurityViewModel>();

        private readonly BindableCollection<SecurityViewModel> securities = new BindableCollection<SecurityViewModel>();
        public BindableCollection<SecurityViewModel> Securities { get { return securities; } }

        public IFilterFlyoutViewModel FilterFlyout { get; set; }
        public IResearchReportViewModel ResearchReport { get; set; }

        private bool isFilterFlyoutOpened;
        public bool IsFilterFlyoutOpened
        {
            get { return isFilterFlyoutOpened; }
            set { SetNotify(ref isFilterFlyoutOpened, value); }
        }

        private SecurityViewModel selectedSecurity;
        public SecurityViewModel SelectedSecurity
        {
            get { return selectedSecurity; }
            set { SetNotify(ref selectedSecurity, value); }
        }
    }
}