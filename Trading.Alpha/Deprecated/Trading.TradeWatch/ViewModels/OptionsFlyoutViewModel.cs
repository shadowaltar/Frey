using Trading.Common;
using Trading.Common.SharedSettings;
using Trading.Common.ViewModels;

namespace Trading.SecurityResearch.ViewModels
{
    public class OptionsFlyoutViewModel : OptionsViewModelBase<IMainViewModel>, IOptionsFlyoutViewModel
    {
        public OptionsFlyoutViewModel(ISettings settings)
        {
            Settings = settings;
        }
    }
}