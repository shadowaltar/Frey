using Caliburn.Micro;

namespace Trading.Common.Utils
{
    public interface IHasEventAggregator
    {
        IEventAggregator EventAggregator { get; set; }
    }
}