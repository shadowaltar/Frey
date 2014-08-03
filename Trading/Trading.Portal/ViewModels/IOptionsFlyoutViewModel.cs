using Caliburn.Micro;

namespace Trading.Portal.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        string SelectedEnvironment { get; set; }
        BindableCollection<string> Environments { get; }
    }
}