using Caliburn.Micro;

namespace Maintenance.Common.Utils
{
    public interface IHasEventAggregator
    {
        IEventAggregator EventAggregator { get; set; }
    }
}