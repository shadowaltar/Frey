using Caliburn.Micro;

namespace Maintenance.AssetMaps.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        string SelectedEnvironment { get; set; }
        BindableCollection<string> Environments { get; }
    }
}