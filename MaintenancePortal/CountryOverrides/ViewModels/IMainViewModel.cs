using Caliburn.Micro;
using Maintenance.Common.Utils;
using Maintenance.CountryOverrides.Entities;
using System.Collections;
using System.Windows.Input;

namespace Maintenance.CountryOverrides.ViewModels
{
    public interface IMainViewModel : IHasViewService, IHasDataAccessFactory<CountryOverrideDataAccess>
    {
        BindableCollection<OverrideItem> OverrideItems { get; }
        OverrideItem SelectedOverrideItem { get; set; }

        bool IsBelongsToVisible { get; set; }
        bool IsAddFlyoutOpen { get; set; }
        bool IsEditFlyoutOpen { get; set; }
        bool IsFilterFlyoutOpen { get; set; }
        bool IsOptionsFlyoutOpen { get; set; }

        bool CanToggleAdd { get; set; }
        bool CanToggleEdit { get; set; }
        bool CanDelete { get; set; }
        bool CanOverrideItems { get; set; }

        void ToggleAdd();
        void ToggleEdit();
        void Delete();
        void ToggleFilter();
        void ToggleOptions();
        void Refresh();

        void HandleShortcutKeys(KeyEventArgs e);

        void SelectMultipleItems(IList items);

        void CopySelected();
        void CopyAll();

        /// <summary>
        /// Handle the post-operation tasks here, like refreshing the grid.
        /// </summary>
        /// <param name="message"></param>
        void Handle(ActivityMessage<IMainViewModel> message);
    }
}