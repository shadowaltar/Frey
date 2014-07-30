using System.Windows.Input;
using Caliburn.Micro;
using Maintenance.Benchmarks.Entities;
using Maintenance.Common.ViewModels;

namespace Maintenance.Benchmarks.ViewModels
{
    public class BenchmarkDependencyReportViewModel : ViewModelBase, IBenchmarkDependencyReportViewModel
    {
        public BenchmarkDependencyReportViewModel()
        {
            DisplayName = "Cannot Delete Due to Existing Relationships";
        }

        private readonly BindableCollection<BenchmarkDependency> dependencies = new BindableCollection<BenchmarkDependency>();
        public BindableCollection<BenchmarkDependency> Dependencies { get { return dependencies; } }

        private BenchmarkDependency selectedDependency;
        public BenchmarkDependency SelectedDependency
        {
            get { return selectedDependency; }
            set { SetNotify(ref selectedDependency, value); }
        }

        public void Ok()
        {
            TryClose(null);
        }

        public void HandleKeys(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedDependency = null;
                TryClose(null);
                Dependencies.Clear();
            }
        }
    }

    public interface IBenchmarkDependencyReportViewModel
    {
        BindableCollection<BenchmarkDependency> Dependencies { get; }
    }
}