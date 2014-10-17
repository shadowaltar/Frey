using Caliburn.Micro;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;

namespace Trading.Common.ViewModels
{
    public class OptionsViewModelBase<T> : ViewModelBase
    {
        public OptionsViewModelBase()
        {
            Environments = new BindableCollection<string>();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (Settings == null || Settings.Environments.Count == 0)
            {
                Log.Error("Cannot set environments into option flyout.");
            }
            else
            {
                Environments.AddRange(Settings.Environments.Keys);
            }
        }

        public ISettings Settings { get; set; }

        public BindableCollection<string> Environments { get; private set; }

        private string selectedEnvironment;

        public virtual string SelectedEnvironment
        {
            get { return selectedEnvironment; }
            set { SetNotify(ref selectedEnvironment, value); }
        }

        /// <summary>
        /// Apply the change of the db environment.
        /// </summary>
        public virtual void Apply()
        {
            Publish<T>(ActivityType.ChangeEnvironment, SelectedEnvironment);
        }
    }
}