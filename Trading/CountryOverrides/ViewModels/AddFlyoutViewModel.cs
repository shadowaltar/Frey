using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.CountryOverrides.Entities;
using Trading.CountryOverrides.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Trading.CountryOverrides.ViewModels
{
    public class AddFlyoutViewModel : ModificationFlyoutViewModelBase, IAddFlyoutViewModel
    {
        public AddFlyoutViewModel(ISecuritySearchResultViewModel securitySearchResult)
        {
            this.securitySearchResult = securitySearchResult;

            OverrideTypes.Add(OverrideType.ALL); // FILC is readonly.
            OverrideTypes.Add(OverrideType.PM);
            OverrideTypes.Add(OverrideType.PORTFOLIO);
        }

        private readonly ISecuritySearchResultViewModel securitySearchResult;

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<CountryOverrideDataAccess> DataAccessFactory { get; set; }

        public bool IsReady { get; set; }

        private OverrideType? selectedOverrideType;
        public OverrideType? SelectedOverrideType
        {
            get { return selectedOverrideType; }
            set
            {
                if (SetNotify(ref selectedOverrideType, value))
                {
                    CheckOverrideConfigurations(value);
                    CheckCanSave();
                }
            }
        }

        private Country selectedCountry;
        public Country SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                if (SetNotify(ref selectedCountry, value))
                {
                    CheckCanSave();
                }
            }
        }

        private Portfolio selectedPortfolio;
        public Portfolio SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set
            {
                if (SetNotify(ref selectedPortfolio, value))
                {
                    CheckCanSave();
                }
            }
        }

        private PortfolioManager selectedPortfolioManager;
        public PortfolioManager SelectedPortfolioManager
        {
            get { return selectedPortfolioManager; }
            set
            {
                if (SetNotify(ref selectedPortfolioManager, value))
                {
                    CheckCanSave();
                }
            }
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { SetNotify(ref id, value); }
        }

        private readonly BindableCollection<OverrideType> overrideTypes = new BindableCollection<OverrideType>();
        public BindableCollection<OverrideType> OverrideTypes { get { return overrideTypes; } }

        private readonly List<Security> searchedSecurities = new List<Security>();
        public List<Security> SearchedSecurities { get { return searchedSecurities; } }

        public List<OverrideItem> ExistingItems { get; set; }

        public async void SearchByCusip(KeyEventArgs keyArgs)
        {
            if (keyArgs.Key != Key.Enter || string.IsNullOrEmpty(Cusip))
                return;

            // reset other fields before search.
            Name = string.Empty;
            Sedol = string.Empty;
            var progress = await ViewService.ShowProgress("Searching security",
                "By CUSIP equals to: " + Cusip);
            await Search(string.Empty, Cusip, string.Empty);
            await progress.Stop();
            ShowSearchResult();
        }

        public async void SearchBySedol(KeyEventArgs keyArgs)
        {
            if (keyArgs.Key != Key.Enter || string.IsNullOrEmpty(Sedol))
                return;

            if (string.IsNullOrEmpty(Sedol))
                return;

            // reset other fields before search.
            Cusip = string.Empty;
            Name = string.Empty;
            var progress = await ViewService.ShowProgress("Searching security",
                "By SEDOL equals to: " + Sedol);
            await Search(string.Empty, string.Empty, Sedol);
            await progress.Stop();
            ShowSearchResult();
        }

        public async void SearchByName(KeyEventArgs keyArgs)
        {
            if (keyArgs.Key != Key.Enter || string.IsNullOrEmpty(Name))
                return;

            if (string.IsNullOrEmpty(Name))
                return;

            // reset other fields before search.
            Cusip = string.Empty;
            Sedol = string.Empty;
            var progress = await ViewService.ShowProgress("Searching security",
                 "By security name starts with: " + Name);
            await Search(Name, string.Empty, string.Empty);
            await progress.Stop();
            ShowSearchResult();
        }

        public async void Save()
        {
            if (SelectedOverrideType == null)
                return;

            var decision = await ViewService.AskSave();

            if (decision != MessageDialogResult.Affirmative)
                return;

            bool saveResult;
            try
            {
                saveResult = await InternalSave();
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when adding data to database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                // disable save button if success
                CanSave = false;

                await ViewService.ShowMessage("New entry created",
                    "The new override entry for \"" + Name + "\" is created successfully.");

                ClearAll();
                Publish<IMainViewModel>(ActivityType.Add);
            }
            else
            {
                await ViewService.ShowError("The new override entry for \"" + Name + "\" cannot be created.");
            }
        }

        private Task<bool> InternalSave()
        {
            return Task.Factory.StartNew(() =>
            {
                if (SelectedOverrideType == null)
                    throw new InvalidOverrideTypeException("You must select a type.");

                Log.InfoFormat("Add new item to database: {0},{1},{2},{3},{4}",
                    SelectedOverrideType, Name, SelectedCountry.DisplayName,
                    SelectedPortfolio == null ? string.Empty : SelectedPortfolio.Name,
                    SelectedPortfolioManager == null ? string.Empty : SelectedPortfolioManager.Name);
                using (var da = DataAccessFactory.New())
                {
                    return da.Add((OverrideType)SelectedOverrideType,
                        Id.ConvertLong(), SelectedCountry.Code,
                        SelectedPortfolio == null ? 0 : SelectedPortfolio.Id,
                        SelectedPortfolioManager == null ? 0 : SelectedPortfolioManager.Id);
                }
            });
        }

        public void ClearAll()
        {
            SelectedOverrideType = null;
            Id = null;
            FullName = null;
            OriginalCountry = null;
            SelectedCountry = null;
            Name = null;
            Cusip = null;
            Sedol = null;
        }

        private Task Search(string n = "", string c = "", string s = "")
        {
            return TaskEx.Run(() =>
            {
                using (var da = DataAccessFactory.New())
                {
                    var searchResults = da.Search(n, c, s);
                    var items = searchResults.Rows.Cast<DataRow>()
                        .Select(r => new Security
                        {
                            Id = r["ID"].ConvertLong(),
                            Cusip = r["FMRCUSIP"].ToString(),
                            Country = Countries.FirstOrDefault(ctry => ctry.Code == r["LOCATION"].ToString()),
                            Name = r["NAME"].ToString(),
                            FullName = r["FULLNAME"].ToString(),
                            Sedol = r["SEDOL"].ToString(),
                        });
                    SearchedSecurities.ClearAndAddRange(items);
                }
            });
        }

        private async void ShowSearchResult()
        {
            await ViewService.Darken();

            securitySearchResult.Securities.ClearAndAddRange(SearchedSecurities);

            var result = await ViewService.ShowDialog(securitySearchResult as ViewModelBase);

            await ViewService.LightUp();

            var sec = securitySearchResult.SelectedSecurity;
            if (result != null && (bool)result && sec != null)
            {
                Id = sec.Id.ToString();
                Name = sec.Name;
                Cusip = sec.Cusip;
                OriginalCountry = sec.Country.ToString();
                Sedol = sec.Sedol;
                FullName = sec.FullName;
            }
            if (CheckItemExists())
            {
                CanSave = false;
                await ViewService.ShowMessage("Oops!", @"You can't create an item of a security 
with the same type, which already exists in the database. Instead, you can edit the item if you wish.");
            }
            else
            {
                CheckCanSave();
            }
        }

        private void CheckCanSave()
        {
            var result = false;
            if (!string.IsNullOrEmpty(Id) && Id.ConvertLong() != 0 && SelectedCountry != null)
            {
                switch (SelectedOverrideType)
                {
                    case OverrideType.ALL:
                    case OverrideType.FILC:
                        result = true;
                        break;
                    case OverrideType.PM:
                        if (SelectedPortfolioManager != null)
                            result = true;
                        break;
                    case OverrideType.PORTFOLIO:
                        if (SelectedPortfolio != null)
                            result = true;
                        break;
                    case null: // result = false;
                        break;
                    default:
                        throw new InvalidOverrideTypeException(SelectedOverrideType);
                }
            }
            CanSave = !CheckItemExists() && result;
        }

        private bool CheckItemExists()
        {
            var teid = Id.ConvertLong();
            return ExistingItems.Any(i => i.Type == selectedOverrideType && i.Id == teid);
        }

        private void CheckOverrideConfigurations(OverrideType? type)
        {
            switch (type)
            {
                case OverrideType.PM:
                    ShowBelongsToPortfolioManager = true;
                    ShowBelongsToPortfolio = false;
                    ShowBelongsTo = true;
                    SelectedPortfolio = null;
                    break;
                case OverrideType.PORTFOLIO:
                    ShowBelongsToPortfolioManager = false;
                    ShowBelongsToPortfolio = true;
                    ShowBelongsTo = true;
                    SelectedPortfolioManager = null;
                    break;
                default:
                    ShowBelongsToPortfolioManager = false;
                    ShowBelongsToPortfolio = false;
                    ShowBelongsTo = false;
                    SelectedPortfolio = null;
                    SelectedPortfolioManager = null;
                    break;
            }
        }
    }
}