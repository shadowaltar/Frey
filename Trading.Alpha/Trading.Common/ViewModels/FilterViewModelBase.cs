using Caliburn.Micro;
using Trading.Common.Utils;

namespace Trading.Common.ViewModels
{
    public class FilterViewModelBase<T> : ViewModelBase
    {
        protected FilterOptions CurrentOptions = new FilterOptions();

        /// <summary>
        /// Publish an <see cref="ActivityMessage{T}"/> to the class which implements
        /// <see cref="IHandle{TMessage}"/>, with optional argument <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterKey"></param>
        /// <param name="item"></param>
        protected virtual void Filter(string filterKey, object item = null)
        {
            Publish<T>(ActivityType.Filter, ChangeFilter(filterKey, item));
        }

        /// <summary>
        /// Clear all fields and also reset main view's list of items.
        /// </summary>
        public virtual void ResetFilterTarget()
        {
            CurrentOptions.IsReset = true;
            CurrentOptions.Clear();
            EventAggregator.PublishOnCurrentThread(CurrentOptions);
        }

        private FilterOptions ChangeFilter(string type, object value)
        {
            CurrentOptions.IsReset = false;
            CurrentOptions[type] = value == null ? "" : value.ToString();
            return CurrentOptions;
        }
    }
}