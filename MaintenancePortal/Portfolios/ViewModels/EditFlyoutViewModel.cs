using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.Portfolios.Entities;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public class EditFlyoutViewModel : ViewModelBase, IEditFlyoutViewModel
    {
        public static Location NotSetLocationOption = Dummies.NewDummyLocation;
        public static bool DefaultIsTopLevelFund = false;

        private PortfolioExtendedInfo info;
        private PortfolioExtendedInfo originalInfo;
        private Portfolio portfolio;
        private bool isNewEntry;

        public IViewService ViewService { get; set; }
        public IDataAccessFactory<PortfolioDataAccess> DataAccessFactory { get; set; }

        private readonly BindableCollection<string> assetClassFocuses = new BindableCollection<string>();
        public BindableCollection<string> AssetClassFocuses { get { return assetClassFocuses; } }

        private readonly BindableCollection<Location> locations = new BindableCollection<Location>();
        public BindableCollection<Location> Locations { get { return locations; } }

        private readonly BindableCollection<string> sectorSchemes = new BindableCollection<string>();
        public BindableCollection<string> SectorSchemes { get { return sectorSchemes; } }

        private bool isTopLevelFund;
        public bool IsTopLevelFund
        {
            get { return isTopLevelFund; }
            set
            {
                if (SetNotify(ref isTopLevelFund, value))
                {
                    CanSave = CheckCanSave();
                    info.IsTopLevelFund = value;
                }
            }
        }

        private Location selectedLocation;
        public Location SelectedLocation
        {
            get { return selectedLocation; }
            set
            {
                if (SetNotify(ref selectedLocation, value))
                {
                    CanSave = CheckCanSave();
                    info.Location = value;
                }
            }
        }

        private string selectedAssetClassFocus;
        public string SelectedAssetClassFocus
        {
            get { return selectedAssetClassFocus; }
            set
            {
                if (SetNotify(ref selectedAssetClassFocus, value))
                {
                    CanSave = CheckCanSave();
                    info.AssetClassFocus = value;
                }
            }
        }

        private string selectedSectorScheme;
        public string SelectedSectorScheme
        {
            get { return selectedSectorScheme; }
            set
            {
                if (SetNotify(ref selectedSectorScheme, value))
                {
                    CanSave = CheckCanSave();
                    info.SectorScheme = value;
                }
            }
        }

        private bool CheckCanSave()
        {
            if (isNewEntry
                && (SelectedAssetClassFocus != Texts.NotSet
                    || SelectedSectorScheme != Texts.NotSet
                    || SelectedLocation != NotSetLocationOption
                    || IsTopLevelFund != DefaultIsTopLevelFund)) // default value of IsTopLevelFund is false
                return true;

            if (!isNewEntry
                && (SelectedAssetClassFocus != originalInfo.AssetClassFocus
                    || SelectedSectorScheme != originalInfo.SectorScheme
                    || SelectedLocation != originalInfo.Location
                    || IsTopLevelFund != originalInfo.IsTopLevelFund))
                return true;

            return false;
        }

        public bool IsReady { get; set; }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        public void SetItem(Portfolio selectedPortfolio, PortfolioExtendedInfo selectedInfo)
        {
            portfolio = selectedPortfolio;

            originalInfo = selectedInfo == null ? null : selectedInfo.Copy();

            // 'not set' means null in db
            if (!SectorSchemes.Contains(Texts.NotSet))
            {
                SectorSchemes.Insert(0, Texts.NotSet);
            }

            if (!AssetClassFocuses.Contains(Texts.NotSet))
            {
                AssetClassFocuses.Insert(0, Texts.NotSet);
            }

            if (!Locations.Contains(NotSetLocationOption))
            {
                Locations.Insert(0, NotSetLocationOption);
            }

            if (selectedInfo == null)
            {
                isNewEntry = true;
                info = new PortfolioExtendedInfo();
                SelectedSectorScheme = Texts.NotSet;
                SelectedAssetClassFocus = Texts.NotSet;
                SelectedLocation = NotSetLocationOption;
                IsTopLevelFund = DefaultIsTopLevelFund;
            }
            else
            {
                isNewEntry = false;
                info = selectedInfo;
                SelectedSectorScheme = info.SectorScheme ?? Texts.NotSet;
                SelectedAssetClassFocus = info.AssetClassFocus ?? Texts.NotSet;
                SelectedLocation = info.Location ?? NotSetLocationOption;
                IsTopLevelFund = info.IsTopLevelFund;
            }

            CanSave = false;
        }

        public void UndoAll()
        {
            SetItem(portfolio, originalInfo);
        }

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        public async void Save()
        {
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
                Log.Error("Error occurs when editing data in database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                CanSave = false;

                await ViewService.ShowMessage("Changes saved",
                    "The changes of entry for \"" + portfolio.Code + "\" is saved successfully.");

                Publish<IMainViewModel>(ActivityType.Edit, info);
            }
            else
            {
                await ViewService.ShowError("The changes of entry for \"" + portfolio.Code + "\" cannot be saved.");
            }
        }

        private Task<bool> InternalSave()
        {
            return TaskEx.Run(() =>
            {
                Log.InfoFormat("Edit Portfolio_Ext in database");

                info.Location = info.Location != NotSetLocationOption ? info.Location : null;
                info.SectorScheme = info.SectorScheme != Texts.NotSet ? info.SectorScheme : null;
                info.AssetClassFocus = info.AssetClassFocus != Texts.NotSet ? info.AssetClassFocus : null;

                using (var access = DataAccessFactory.New())
                {
                    if (isNewEntry)
                        return access.InsertPortfolioExtendedInfo(portfolio.Id, info);
                    return access.UpdatePortfolioExtendedInfo(portfolio.Id, info);
                }
            });
        }
    }
}