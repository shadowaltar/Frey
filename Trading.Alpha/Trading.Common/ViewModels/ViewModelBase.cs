using System.Reflection;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using Ninject;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;


namespace Trading.Common.ViewModels
{
    public class ViewModelBase : Screen
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Inject]
        public virtual IEventAggregator EventAggregator { get; set; }

        public bool SetNotify<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(target, value))
            {
                target = value;
                NotifyOfPropertyChange(propertyName);
                return true;
            }
            return false;
        }

        public bool SetNotify<T>(ref T target, T value, System.Action onChangedCallback, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(target, value))
            {
                target = value;
                NotifyOfPropertyChange(propertyName);
                onChangedCallback();
                return true;
            }
            return false;
        }
    }
}