using PropertyChanged;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class EnterSetupViewModel : ViewModelBase, IEnterSetupViewModel
    {
        public IViewService ViewService { get; set; }
    }

    public interface IEnterSetupViewModel : IHasViewService
    {
    }
}