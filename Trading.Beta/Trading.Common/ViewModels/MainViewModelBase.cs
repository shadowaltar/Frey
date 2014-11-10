using Caliburn.Micro;
using MahApps.Metro.Controls;
using Ninject;
using System;
using System.Diagnostics;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;

namespace Trading.Common.ViewModels
{
    public abstract class MainViewModelBase<TDA> :
        ViewModelBase, IHasViewService,
        IHasDataAccessFactory<TDA>
        where TDA : DataAccess
    {
        public Process CurrentProcess { get { return Process.GetCurrentProcess(); } }
        public abstract string ProgramName { get; }

        private int k = (int)Math.Pow(2, 10);
        private int m = (int)Math.Pow(2, 20);
        private int g = (int)Math.Pow(2, 30);

        protected MainViewModelBase(IDataAccessFactory<TDA> factory, ISettings settings)
        {
            DataAccessFactory = factory;
            Settings = settings;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = view as MetroWindow;
            ViewService.ToFront();
            SetWindowTitle();
            PeriodicTask.Run(SetWindowTitle, TimeSpan.FromSeconds(2));
        }

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

        public virtual void SetWindowTitle()
        {
            var mem = CurrentProcess.PrivateMemorySize64;
            string str;
            if (mem > g)
            {
                str = ((double)mem / g).ToString("N2") + "G";
            }
            else if (mem > m)
            {
                str = ((double)mem / m).ToString("N2") + "M";
            }
            else if (mem > k)
            {
                str = ((double)mem / k).ToString("N2") + "K";
            }
            else
            {
                str = mem.ToString("N2");
            }
            DisplayName = string.Format("{0} ({1})", ProgramName, str);
        }
    }
}