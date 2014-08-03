using System;
using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.Common.ViewModels;
using Trading.CountryOverrides.Entities;
using System.Linq;


namespace Trading.CountryOverrides.ViewModels
{
    public class FilterFlyoutViewModel : FilterViewModelBase<IMainViewModel>, IFilterFlyoutViewModel
    {
        public FilterFlyoutViewModel()
        {
            OverrideTypes.AddRange(Enum.GetValues(typeof(OverrideType)).Cast<OverrideType>());
        }

        public bool IsReady { get; set; }

        private readonly BindableCollection<Country> countries = new BindableCollection<Country>();
        public BindableCollection<Country> OriginalCountries { get { return countries; } }
        public BindableCollection<Country> NewCountries { get { return countries; } }

        private readonly BindableCollection<PortfolioManager> portfolioManagers = new BindableCollection<PortfolioManager>();
        public BindableCollection<PortfolioManager> PortfolioManagers { get { return portfolioManagers; } }

        private readonly BindableCollection<Portfolio> portfolios = new BindableCollection<Portfolio>();
        public BindableCollection<Portfolio> Portfolios { get { return portfolios; } }

        private readonly BindableCollection<OverrideType> overrideTypes = new BindableCollection<OverrideType>();
        public BindableCollection<OverrideType> OverrideTypes { get { return overrideTypes; } }

        private OverrideType selectedOverrideType;
        public OverrideType SelectedOverrideType
        {
            get { return selectedOverrideType; }
            set
            {
                if (SetNotify(ref selectedOverrideType, value))
                    Filter("OverrideType", value);
            }
        }

        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                if (SetNotify(ref id, value) && IsReady) // empty string is useful here
                    Filter("StartOfId", value);
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetNotify(ref name, value) && IsReady) // empty string is useful here
                    Filter("StartOfName", value);
            }
        }

        private Country selectedOriginalCountry;
        public Country SelectedOriginalCountry
        {
            get { return selectedOriginalCountry; }
            set
            {
                if (SetNotify(ref selectedOriginalCountry, value) && IsReady && value != null)
                    Filter("OriginalCountry", value.Code);
            }
        }

        private Country selectedNewCountry;
        public Country SelectedNewCountry
        {
            get { return selectedNewCountry; }
            set
            {
                if (SetNotify(ref selectedNewCountry, value) && IsReady && value != null)
                    Filter("NewCountry", value.Code);
            }
        }

        private string cusip;
        public string Cusip
        {
            get { return cusip; }
            set
            {
                if (SetNotify(ref cusip, value) && IsReady) // empty string is useful here
                    Filter("StartOfCusip", value);
            }
        }

        private string sedol;
        public string Sedol
        {
            get { return sedol; }
            set
            {
                if (SetNotify(ref sedol, value) && IsReady) // empty string is useful here
                    Filter("StartOfSedol", value);
            }
        }

        private Portfolio selectedPortfolio;
        public Portfolio SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set
            {
                if (SetNotify(ref selectedPortfolio, value) && IsReady && value != null)
                    Filter("PortfolioId", value.Id);
            }
        }

        private PortfolioManager selectedPortfolioManager;
        public PortfolioManager SelectedPortfolioManager
        {
            get { return selectedPortfolioManager; }
            set
            {
                if (SetNotify(ref selectedPortfolioManager, value) && IsReady && value != null)
                    Filter("PortfolioManagerId", value.Id);
            }
        }

        /// <summary>
        /// Clear all fields.
        /// </summary>
        public void ClearAll()
        {
            SelectedOverrideType = OverrideTypes.FirstOrDefault();
            SelectedPortfolio = Portfolios.FirstOrDefault(); // would be 'AnyPortfolio'
            SelectedNewCountry = NewCountries.FirstOrDefault(); // would be 'AnyCountry'
            SelectedOriginalCountry = OriginalCountries.FirstOrDefault(); // would be 'AnyCountry'
            SelectedPortfolioManager = PortfolioManagers.FirstOrDefault(); // would be 'AnyPerson'

            Id = string.Empty;
            Name = string.Empty;
            Cusip = string.Empty;
            Sedol = string.Empty;

            CurrentOptions.Clear();
        }

        /// <summary>
        /// Clear all fields and also reset main view's list of items.
        /// </summary>
        public void Reset()
        {
            ClearAll();
            ResetFilterTarget();
        }
    }
}