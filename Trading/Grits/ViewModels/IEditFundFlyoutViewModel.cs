using Caliburn.Micro;
using Maintenance.Common.Utils;
using Maintenance.Grits.Entities;

namespace Maintenance.Grits.ViewModels
{
    public interface IEditFundFlyoutViewModel : IHasViewService,
        IHasDataAccessFactory<GritsDataAccess>
    {
        bool IsReady { get; set; }
        bool CanSave { get; set; }

        string Code { get; set; }
        GritsMode SelectedMode { get; set; }
        BindableCollection<GritsMode> Modes { get; }

        /// <summary>
        /// Let main vm sets the selected item to the edit vm.
        /// </summary>
        /// <param name="item"></param>
        void SetItem(GritsFund item);

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        void Save();

        void UndoAll();
    }
}