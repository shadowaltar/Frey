using Caliburn.Micro;
using MahApps.Metro;
using Trading.Common;
using Trading.Common.SharedSettings;
using Trading.Common.ViewModels;
using System.Linq;
using System.Windows;

namespace Trading.Portal.ViewModels
{
    public class OptionsFlyoutViewModel : OptionsViewModelBase<IPortalViewModel>, IOptionsFlyoutViewModel
    {
        public OptionsFlyoutViewModel(ISettings settings)
        {
            Settings = settings;
        }

        public BindableCollection<Accent> Accents { get; private set; }
        public BindableCollection<AppTheme> Themes { get; private set; }
    }
}