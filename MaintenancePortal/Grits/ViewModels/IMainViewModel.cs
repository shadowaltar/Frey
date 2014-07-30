using Caliburn.Micro;
using Maintenance.Common.Utils;
using Maintenance.Grits.Entities;
using System.Collections;
using System.Windows.Input;

namespace Maintenance.Grits.ViewModels
{
    public interface IMainViewModel
    {
        IAddFlyoutViewModel AddFlyout { get; }
        IEditFundFlyoutViewModel EditFundFlyout { get; }
        IEditBenchmarkFlyoutViewModel EditBenchmarkFlyout { get; }
        IOptionsFlyoutViewModel OptionsFlyout { get; }

        BindableCollection<GritsFund> Funds { get; }
        GritsFund SelectedFund { get; set; }

        BindableCollection<GritsBenchmark> Benchmarks { get; }
        GritsBenchmark SelectedBenchmark { get; set; }

        bool IsAddFlyoutOpen { get; set; }
        bool IsEditFundFlyoutOpen { get; set; }
        bool IsEditBenchmarkFlyoutOpen { get; set; }
        bool IsOptionsFlyoutOpen { get; set; }

        bool CanToggleAdd { get; set; }
        bool CanToggleEdit { get; set; }

        void ToggleAdd();
        void ToggleEdit();
        void ToggleOptions();

        void HandleShortcutKeys(KeyEventArgs e);

        void SelectMultipleItems(IList items);

        /// <summary>
        /// Handle the post-operation tasks here, like refreshing the grid.
        /// </summary>
        /// <param name="message"></param>
        void Handle(ActivityMessage<IMainViewModel> message);
    }
}