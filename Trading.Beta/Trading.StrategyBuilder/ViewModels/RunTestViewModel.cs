using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.ViewModels
{
    public class RunTestViewModel : ViewModelBase, IRunTestViewModel
    {
        public IViewService ViewService { get; set; }
    }

    public interface IRunTestViewModel : IHasViewService
    {
    }
}