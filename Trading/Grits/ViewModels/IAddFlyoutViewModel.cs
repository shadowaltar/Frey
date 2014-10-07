using Caliburn.Micro;
using Maintenance.Common.Utils;
using Maintenance.Grits.Entities;
using System.Collections.Generic;

namespace Maintenance.Grits.ViewModels
{
    public interface IAddFlyoutViewModel : IHasViewService,
        IHasDataAccessFactory<GritsDataAccess>
    {
        bool CanSave { get; set; }

        bool IsReady { get; set; }

        GritsMode? SelectedGritsMode { get; set; }
        Fund SelectedFund { get; set; }
        Benchmark SelectedBenchmark { get; set; }
        bool AddBenchmark { get; set; }
        bool IsBenchmarkLoadedAtNight { get; set; }
        BindableCollection<Fund> Funds { get; }
        BindableCollection<Benchmark> Benchmarks { get; }
        BindableCollection<GritsMode> GritsModes { get; }
        List<GritsFund> ExistingGritsFunds { get; set; }
        List<GritsBenchmark> ExistingGritsBenchmarks { get; set; }
        void Save();
        void CleanAll();
    }
}