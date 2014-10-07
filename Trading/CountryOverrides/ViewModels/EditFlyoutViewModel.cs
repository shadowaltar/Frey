using MahApps.Metro.Controls.Dialogs;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.CountryOverrides.Entities;
using Trading.CountryOverrides.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Trading.CountryOverrides.ViewModels
{
    public class EditFlyoutViewModel : ModificationFlyoutViewModelBase, IEditFlyoutViewModel
    {
        private OverrideItem currentItem;

        public bool IsReady { get; set; }

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<CountryOverrideDataAccess> DataAccessFactory { get; set; }

        private OverrideType overrideType;
        public OverrideType OverrideType
        {
            get { return overrideType; }
            set { SetNotify(ref overrideType, value); }
        }

        private Country selectedCountry;
        public Country SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                if (SetNotify(ref selectedCountry, value))
                {
                    CanSave = CheckCanSave();
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
                    CanSave = CheckCanSave();
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
                    CanSave = CheckCanSave();
                }
            }
        }

        private long id;
        public long Id
        {
            get { return id; }
            set { SetNotify(ref id, value); }
        }

        /// <summary>
        /// Let main vm sets the selected item to the edit vm.
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(OverrideItem item)
        {
            currentItem = item;
            Id = item.Id;
            OverrideType = item.Type;
            Name = item.Name;
            FullName = item.FullName;
            OriginalCountry = item.OldCountry.DisplayName;
            SelectedCountry = Countries.FirstOrDefault(c => c.Equals(item.NewCountry));
            Cusip = item.Cusip;
            Sedol = item.Sedol;
            switch (OverrideType)
            {
                case OverrideType.ALL:
                    ShowBelongsTo = false;
                    ShowBelongsToPortfolio = false;
                    ShowBelongsToPortfolioManager = false;
                    SelectedPortfolio = null;
                    SelectedPortfolioManager = null;
                    break;
                case OverrideType.PM:
                    ShowBelongsTo = true;
                    ShowBelongsToPortfolio = false;
                    ShowBelongsToPortfolioManager = true;
                    SelectedPortfolio = null;
                    SelectedPortfolioManager = PortfolioManagers.FirstOrDefault(pm => pm.Id == item.PortfolioManagerId);
                    break;
                case OverrideType.PORTFOLIO:
                    ShowBelongsTo = true;
                    ShowBelongsToPortfolio = true;
                    ShowBelongsToPortfolioManager = false;
                    SelectedPortfolio = Portfolios.FirstOrDefault(p => p.Id == item.PortfolioId);
                    SelectedPortfolioManager = null;
                    break;
                default:
                    throw new InvalidOverrideTypeException(OverrideType);
            }
            CanSave = false;
        }

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        public async void Save()
        {
            var decision = await ViewService.ShowMessage("Confirm to save?",
                "Remember you can't undo, and applications would immediately make use of your change.",
                false, true, "Yes", "No");
            if (decision != MessageDialogResult.Affirmative)
                return;

            bool saveResult;
            try
            {
                saveResult = await InternalSave();
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when editing data in database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                CanSave = false;

                await ViewService.ShowMessage("Changes saved",
                    "The changes of entry for \"" + Name + "\" is saved successfully.");

                Publish<IMainViewModel>(ActivityType.Edit, currentItem);
            }
            else
            {
                await ViewService.ShowError("The changes of entry for \"" + Name + "\" cannot be saved.");
            }
        }

        private Task<bool> InternalSave()
        {
            return Task.Factory.StartNew(() =>
            {
                Log.InfoFormat("Edit CountryOverride in database");

                using (var da = DataAccessFactory.New())
                {
                    return da.Edit(OverrideType, Id, SelectedCountry.Code,
                        SelectedPortfolio == null ? 0 : SelectedPortfolio.Id,
                        SelectedPortfolioManager == null ? 0 : SelectedPortfolioManager.Id,
                        currentItem.PortfolioId, currentItem.PortfolioManagerId);
                }
            });
        }

        public void UndoAll()
        {
            SelectedCountry = currentItem.NewCountry;
            SelectedPortfolioManager = currentItem.PortfolioManager;
            SelectedPortfolio = currentItem.Portfolio;
            CanSave = false;
        }

        private bool CheckCanSave()
        {
            if (SelectedCountry != null)
            {
                switch (OverrideType)
                {
                    case OverrideType.ALL:
                    case OverrideType.FILC:
                        return !currentItem.NewCountry.Equals(SelectedCountry);
                    case OverrideType.PM:
                        return SelectedPortfolioManager != null &&
                            (!currentItem.PortfolioManager.Equals(SelectedPortfolioManager)
                            || !currentItem.NewCountry.Equals(SelectedCountry));
                    case OverrideType.PORTFOLIO:
                        return SelectedPortfolio != null &&
                            (!currentItem.Portfolio.Equals(SelectedPortfolio)
                            || !currentItem.NewCountry.Equals(SelectedCountry));
                    default:
                        throw new InvalidOverrideTypeException(OverrideType);
                }
            }
            return false;
        }
    }
}