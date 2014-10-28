using Trading.Backtest.ViewModels;
using Trading.Common;
using Trading.Common.Data;

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