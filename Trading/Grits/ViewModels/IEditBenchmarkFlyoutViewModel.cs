using Maintenance.Common.Utils;
using Maintenance.Grits.Entities;

namespace Maintenance.Grits.ViewModels
{
    public interface IEditBenchmarkFlyoutViewModel : IHasViewService,
        IHasDataAccessFactory<GritsDataAccess>
    {
        bool IsReady { get; set; }
        bool CanSave { get; set; }

        void Save();
        void Disable();
        void UndoAll();
        void SetItem(GritsBenchmark benchmark);
    }
}