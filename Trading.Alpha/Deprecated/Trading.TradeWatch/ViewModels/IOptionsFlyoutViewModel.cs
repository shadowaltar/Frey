namespace Trading.SecurityResearch.ViewModels
{
    public interface IOptionsFlyoutViewModel
    {
        void Apply();
        string SelectedEnvironment { get; set; }
    }
}