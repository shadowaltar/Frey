using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.CountryOverrides.Entities;
using System.Collections;
using System.Collections.Generic;


namespace Trading.CountryOverrides.ViewModels
{
    public partial class MainViewModel
    {
        public IAddFlyoutViewModel AddFlyout { get; private set; }
        public IEditFlyoutViewModel EditFlyout { get; private set; }
        public ICustomCountryEditorViewModel CustomCountryEditor { get; private set; }
        public IFilterFlyoutViewModel FilterFlyout { get; private set; }
        public IOptionsFlyoutViewModel OptionsFlyout { get; private set; }

        private List<OverrideItem> allItems = new List<OverrideItem>();

        private List<Country> orderedCountries;
        private List<Portfolio> orderedPortfolios;
        private List<PortfolioManager> orderedPortfolioManagers;

        private readonly Dictionary<string, Country> countries = new Dictionary<string, Country>();
        private readonly Dictionary<long, Portfolio> portfolios = new Dictionary<long, Portfolio>();
        private readonly Dictionary<long, PortfolioManager> portfolioManagers = new Dictionary<long, PortfolioManager>();

        private readonly Dictionary<OverrideKey, string> oldCountryCodes = new Dictionary<OverrideKey, string>();
        private readonly Dictionary<OverrideKey, string> newCountryCodes = new Dictionary<OverrideKey, string>();
        private readonly Dictionary<OverrideKey, long> portfolioManagerMapping = new Dictionary<OverrideKey, long>();
        private readonly Dictionary<OverrideKey, long> portfolioMapping = new Dictionary<OverrideKey, long>();

        private IList selectedItems;
        private bool isOnlyOneSelected;

        private readonly BindableCollection<OverrideItem> overrideItems = new BindableCollection<OverrideItem>();
        public BindableCollection<OverrideItem> OverrideItems { get { return overrideItems; } }

        private bool isBelongsToVisible;
        public bool IsBelongsToVisible
        {
            get { return isBelongsToVisible; }
            set { SetNotify(ref isBelongsToVisible, value); }
        }

        private bool isAddFlyoutOpen;
        public bool IsAddFlyoutOpen
        {
            get { return isAddFlyoutOpen; }
            set { SetNotify(ref isAddFlyoutOpen, value); }
        }

        private bool isEditFlyoutOpen;
        public bool IsEditFlyoutOpen
        {
            get { return isEditFlyoutOpen; }
            set { SetNotify(ref isEditFlyoutOpen, value); }
        }

        private bool isFilterFlyoutOpen;
        public bool IsFilterFlyoutOpen
        {
            get { return isFilterFlyoutOpen; }
            set { SetNotify(ref isFilterFlyoutOpen, value); }
        }

        private bool isOptionsFlyoutOpen;
        public bool IsOptionsFlyoutOpen
        {
            get { return isOptionsFlyoutOpen; }
            set { SetNotify(ref isOptionsFlyoutOpen, value); }
        }

        private bool canSelectOverrideItems;
        public bool CanSelectOverrideItems
        {
            get { return canSelectOverrideItems; }
            set { SetNotify(ref canSelectOverrideItems, value); }
        }

        private bool canToggleAdd;
        public bool CanToggleAdd
        {
            get { return canToggleAdd; }
            set { SetNotify(ref canToggleAdd, value); }
        }

        private bool canToggleEdit;
        public bool CanToggleEdit
        {
            get { return canToggleEdit; }
            set { SetNotify(ref canToggleEdit, value); }
        }

        private bool canToggleFilter;
        public bool CanToggleFilter
        {
            get { return canToggleFilter; }
            set { SetNotify(ref canToggleFilter, value); }
        }

        private bool canDelete;
        public bool CanDelete
        {
            get { return canDelete; }
            set { SetNotify(ref canDelete, value); }
        }

        private bool canOverrideItems;
        public bool CanOverrideItems
        {
            get { return canOverrideItems; }
            set { SetNotify(ref canOverrideItems, value); }
        }

        private OverrideItem selectedOverrideItem;
        public OverrideItem SelectedOverrideItem
        {
            get { return selectedOverrideItem; }
            set
            {
                if (SetNotify(ref selectedOverrideItem, value) && value != null)
                {
                    if (CanToggleEdit && IsEditFlyoutOpen)
                        ToggleEdit();
                    CheckEnabledAndVisibility();
                }
            }
        }
    }
}