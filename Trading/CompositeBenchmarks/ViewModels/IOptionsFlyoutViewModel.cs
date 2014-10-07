using Caliburn.Micro;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        bool IsShowExpiredCompositeBenchmarks { get; set; }
        bool IsShowAllBenchmarkComponents { get; set; }
        bool IsAutoPopulateNewCompositeBenchmark { get; set; }

        BindableCollection<string> Environments { get; }

        void Apply();
        string SelectedEnvironment { get; set; }
        bool IsPublishingPreference { get; set; }
        bool IsShowInactiveCompositeBenchmarks { get; set; }
    }
}