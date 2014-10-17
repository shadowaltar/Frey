using Caliburn.Micro;
using Trading.Common.Utils;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ninject;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace Trading.Common.ViewModels
{
    public class ViewModelBase : Screen, IHasEventAggregator
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [Inject]
        public virtual IEventAggregator EventAggregator { get; set; }

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

        /// <summary>
        /// Publish an <see cref="ActivityMessage{T}"/> to the class <typeparamref name="T"/> which implements
        /// <see cref="IHandle{ActivityMessageT}"/>, with optional argument <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="activity"></param>
        /// <param name="item"></param>
        protected virtual void Publish<T>(ActivityType activity, object item = null)
        {
            EventAggregator.PublishOnUIThread(new ActivityMessage<T>(activity, item));
        }
    }
}