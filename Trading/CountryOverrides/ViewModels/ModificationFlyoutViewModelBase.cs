using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.Common.ViewModels;
using Trading.CountryOverrides.Entities;


namespace Trading.CountryOverrides.ViewModels
{
    public class ModificationFlyoutViewModelBase : ViewModelBase
    {
        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private bool showBelongsToPortfolioManager;
        public bool ShowBelongsToPortfolioManager
        {
            get { return showBelongsToPortfolioManager; }
            set { SetNotify(ref showBelongsToPortfolioManager, value); }
        }

        private bool showBelongsToPortfolio;
        public bool ShowBelongsToPortfolio
        {
            get { return showBelongsToPortfolio; }
            set { SetNotify(ref showBelongsToPortfolio, value); }
        }

        private bool showBelongsTo;
        public bool ShowBelongsTo
        {
            get { return showBelongsTo; }
            set { SetNotify(ref showBelongsTo, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetNotify(ref name, value); }
        }

        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set { SetNotify(ref fullName, value); }
        }

        private string originalCountry;
        public string OriginalCountry
        {
            get { return originalCountry; }
            set { SetNotify(ref originalCountry, value); }
        }

        private string cusip;
        public string Cusip
        {
            get { return cusip; }
            set { SetNotify(ref cusip, value); }
        }

        private string sedol;
        public string Sedol
        {
            get { return sedol; }
            set { SetNotify(ref sedol, value); }
        }

        private readonly BindableCollection<Country> countries = new BindableCollection<Country>();
        public BindableCollection<Country> Countries { get { return countries; } }

        private readonly BindableCollection<Portfolio> portfolios = new BindableCollection<Portfolio>();
        public BindableCollection<Portfolio> Portfolios { get { return portfolios; } }

        private readonly BindableCollection<PortfolioManager> portfolioManagers = new BindableCollection<PortfolioManager>();
        public BindableCollection<PortfolioManager> PortfolioManagers { get { return portfolioManagers; } }
    }
}