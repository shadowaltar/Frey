using Caliburn.Micro;

namespace Maintenance.Grits.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        string SelectedEnvironment { get; set; }
        BindableCollection<string> Environments { get; }
    }
}