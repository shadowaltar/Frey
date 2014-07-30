namespace Maintenance.Portfolios.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        void Apply();
        string SelectedEnvironment { get; set; }
    }
}