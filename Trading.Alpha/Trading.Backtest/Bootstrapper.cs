using Trading.Backtest.ViewModels;
using Trading.Common;

namespace Trading.Backtest
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}