using Caliburn.Micro;

namespace Maintenance.Portal.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        string SelectedEnvironment { get; set; }
        BindableCollection<string> Environments { get; }
    }
}