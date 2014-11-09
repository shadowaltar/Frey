using Trading.Backtest.Data;
using Trading.Backtest.ViewModels;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;

namespace Trading.Backtest
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<BacktestDataAccess>,
                BacktestDataAccessFactory<BacktestDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
        }
    }
}