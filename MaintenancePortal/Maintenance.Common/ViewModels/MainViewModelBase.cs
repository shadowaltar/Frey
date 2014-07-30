using System.Reflection;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Ninject;

namespace Maintenance.Common.ViewModels
{
    public abstract class MainViewModelBase<TDA, TC> :
        ViewModelBase, IHasViewService, IHasEnvironment,
        IHasDataAccessFactory<TDA>
        where TDA : DataAccess
        where TC : ISettings
    {
        public abstract string ProgramName { get; }

        protected MainViewModelBase(IDataAccessFactory<TDA> factory, ISettings settings)
        {
            DataAccessFactory = factory;
            Settings = (TC)settings;
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

        public TC Settings { get; set; }

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