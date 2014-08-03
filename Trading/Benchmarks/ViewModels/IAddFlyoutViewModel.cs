using System.Collections.Generic;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.Benchmarks.ViewModels
{
    public interface IAddFlyoutViewModel : IHasViewService, IHasDataAccessFactory
    {
        bool IsReady { get; set; }
        BindableCollection<Index> Indexs { get; }
        BindableCollection<string> Types { get; }
        IEnumerable<Benchmark> ExistingItems { get; set; }
        bool CanSave { get; set; }
        string SelectedType { get; set; }
        Index SelectedIndex { get; set; }
        string Code { get; set; }

        void Save();
        void ClearAll();
    }
}