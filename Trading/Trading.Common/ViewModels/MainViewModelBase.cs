using System.Reflection;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Ninject;

namespace Trading.Common.ViewModels
{
    public abstract class MainViewModelBase<TDA> :
        ViewModelBase, IHasViewService, IHasEnvironment,
        IHasDataAccessFactory<TDA>
        where TDA : DataAccess
    {
        public abstract string ProgramName { get; }

        protected MainViewModelBase(IDataAccessFactory<TDA> factory, ISettings settings)
        {
            DataAccessFactory = factory;
            Settings = settings;
            Environment = Settings.DefaultEnvironment;
            DataAccessFactory.Environment = environment;
            DataAccessFactory.Environments.ClearAndAddRange(Settings.Environments);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = view as MetroWindow;
            ViewService.ToFront();
            DisplayName = string.Format("{0} ({1})", ProgramName, environment);
        }

        protected string environment;
        private IEventAggregator eventAggregator;

        [Inject]
        public IViewService ViewService { get; set; }

        public IDataAccessFactory<TDA> DataAccessFactory { get; set; }

        public ISettings Settings { get; set; }

        public override IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
            set
            {
                eventAggregator = value;
                EventAggregator.Subscribe(this);
            }
        }

        public string Environment
        {
            get { return environment; }
            set
            {
                environment = value;
                DataAccessFactory.Environment = environment;
                DisplayName = string.Format("{0} ({1})", ProgramName, environment);
            }
        }
    }
}