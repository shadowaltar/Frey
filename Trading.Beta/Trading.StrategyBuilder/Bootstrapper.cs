using Trading.Common;
using Trading.StrategyBuilder.ViewModels;

namespace Trading.StrategyBuilder
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}