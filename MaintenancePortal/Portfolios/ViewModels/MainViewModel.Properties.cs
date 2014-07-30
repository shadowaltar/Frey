using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Portfolios.Entities;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public partial class MainViewModel
    {
        private IList selectedItems;
        private bool isSingleSelection;

        private readonly Dictionary<long, Portfolio> allPortfolios = new Dictionary<long, Portfolio>();
        private readonly Dictionary<long, Benchmark> allBenchmarks = new Dictionary<long, Benchmark>();
        private readonly Dictionary<long, Index> allIndexes = new Dictionary<long, Index>();
        private readonly Dictionary<string, Location> allCountries = new Dictionary<string, Location>();
        private readonly List<string> allSectorSchemes = new List<string>();
        private readonly Dictionary<long, PortfolioManager> allPortfolioManagers = new Dictionary<long, PortfolioManager>();
        private readonly Dictionary<long, Instrument> allInstruments = new Dictionary<long, Instrument>();
        private readonly Dictionary<long, PortfolioExtendedInfo> allExtendedInfo = new Dictionary<long, PortfolioExtendedInfo>();
        private readonly Dictionary<long, long> assetClassFocusMapping = new Dictionary<long, long>();

        private readonly BindableCollection<Portfolio> portfolios = new BindableCollection<Portfolio>();
        public BindableCollection<Portfolio> Portfolios { get { return portfolios; } }

        public IEditFlyoutViewModel EditFlyout { get; set; }
        public IBenchmarkAssociationFlyoutViewModel BenchmarkAssociationFlyout { get; set; }
        public IFilterFlyoutViewModel FilterFlyout { get; set; }
        public IOptionsFlyoutViewModel OptionsFlyout { get; set; }

        private bool isEditFlyoutOpened;
        public bool IsEditFlyoutOpened
        {
            get { return isEditFlyoutOpened; }
            set { SetNotify(ref isEditFlyoutOpened, value); }
        }

        private bool isBenchmarkAssociationFlyoutOpened;
        public bool IsBenchmarkAssociationFlyoutOpened
        {
            get { return isBenchmarkAssociationFlyoutOpened; }
            set { SetNotify(ref isBenchmarkAssociationFlyoutOpened, value); }
        }

        private bool isOptionsFlyoutOpened;
        public bool IsOptionsFlyoutOpened
        {
            get { return isOptionsFlyoutOpened; }
            set { SetNotify(ref isOptionsFlyoutOpened, value); }
        }

        private bool isFilterFlyoutOpened;
        public bool IsFilterFlyoutOpened
        {
            get { return isFilterFlyoutOpened; }
            set { SetNotify(ref isFilterFlyoutOpened, value); }
        }

        private bool canToggleEditPortfolio;
        public bool CanToggleEditPortfolio
        {
            get { return canToggleEditPortfolio; }
            set { SetNotify(ref canToggleEditPortfolio, value); }
        }

        private bool canToggleBenchmarkAssociation;
        public bool CanToggleBenchmarkAssociation
        {
            get { return canToggleBenchmarkAssociation; }
            set { SetNotify(ref canToggleBenchmarkAssociation, value); }
        }

        private bool canToggleFilter;
        public bool CanToggleFilter
        {
            get { return canToggleFilter; }
            set { SetNotify(ref canToggleFilter, value); }
        }

        private bool isPortfoliosEnabled;
        public bool IsPortfoliosEnabled
        {
            get { return isPortfoliosEnabled; }
            set { SetNotify(ref isPortfoliosEnabled, value); }
        }

        private PortfolioExtendedInfo selectedPortfolioExtendedInfo;
        public PortfolioExtendedInfo SelectedPortfolioExtendedInfo
        {
            get { return selectedPortfolioExtendedInfo; }
            set { SetNotify(ref selectedPortfolioExtendedInfo, value); }
        }

        private Portfolio selectedPortfolio;
        public Portfolio SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set
            {
                if (SetNotify(ref selectedPortfolio, value) && value != null)
                {
                    if (isSingleSelection)
                    {
                        CanToggleEditPortfolio = CanToggleBenchmarkAssociation = true;
                    }
                    if (IsBenchmarkAssociationFlyoutOpened)
                        ToggleBenchmarkAssociation();
                    if (IsEditFlyoutOpened)
                        ToggleBenchmarkAssociation();

                    PortfolioExtendedInfo info;
                    allExtendedInfo.TryGetValue(value.Id, out info);
                    SelectedPortfolioExtendedInfo = info; // either set to null or get the correct value.
                }
                else
                {
                    CanToggleEditPortfolio = CanToggleBenchmarkAssociation = false;
                }
            }
        }
    }
}