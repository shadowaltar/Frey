using System.Collections.Generic;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.Benchmarks.ViewModels
{
    public interface IEditFlyoutViewModel : IHasViewService, IHasDataAccessFactory
    {
        IEnumerable<Benchmark> ExistingItems { get; set; }
        BindableCollection<Index> Indexs { get; }
        BindableCollection<string> Types { get; }
        bool CanSave { get; set; }
        bool IsReady { get; set; }
        string Code { get; set; }
        string SelectedType { get; set; }
        Index SelectedIndex { get; set; }

        /// <summary>
        /// Let main vm sets the selected item to the edit vm.
        /// </summary>
        /// <param name="item"></param>
        void SetItem(Benchmark item);

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        void Save();

        void UndoAll();
    }
}