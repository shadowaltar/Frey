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
            Environments.AddRange(Settings.Environments.Keys);

            Accents = new BindableCollection<Accent>();
            Themes = new BindableCollection<AppTheme>();

            Accents.AddRange(ThemeManager.Accents);
            Themes.AddRange(ThemeManager.AppThemes);

            SelectedAccent = DefaultAccent;
            SelectedTheme = DefaultTheme;
        }

        public BindableCollection<Accent> Accents { get; private set; }
        public BindableCollection<AppTheme> Themes { get; private set; }

        private static Accent DefaultAccent
        {
            get { return ThemeManager.Accents.FirstOrDefault(a => a.Name == "Cobalt"); }
        }

        private static AppTheme DefaultTheme
        {
            get { return ThemeManager.AppThemes.FirstOrDefault(a => a.Name == "BaseLight"); }
        }

        private Accent selectedAccent;
        public Accent SelectedAccent
        {
            get { return selectedAccent; }
            set
            {
                if (SetNotify(ref selectedAccent, value) && SelectedTheme != null && SelectedAccent != null)
                {
                    SwitchTheme();
                }
            }
        }

        private AppTheme selectedTheme;
        public AppTheme SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (SetNotify(ref selectedTheme, value) && SelectedTheme != null && SelectedAccent != null)
                {
                    SwitchTheme();
                }
            }
        }

        public override string SelectedEnvironment
        {
            get { return base.SelectedEnvironment; }
            set
            {
                if (value != base.SelectedEnvironment)
                {
                    base.SelectedEnvironment = value;
                    Apply();
                }
            }
        }

        public void SwitchTheme()
        {
            ThemeManager.ChangeAppStyle(Application.Current, SelectedAccent, SelectedTheme);
        }

        public void RestoreThemes()
        {
            SelectedAccent = DefaultAccent;
            SelectedTheme = DefaultTheme;
            SwitchTheme();
        }
    }
}