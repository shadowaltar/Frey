using Caliburn.Micro;

namespace Maintenance.Benchmarks.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        string SelectedEnvironment { get; set; }
        BindableCollection<string> Environments { get; }
    }
}