using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.R.ViewModels;

namespace Trading.R
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<TradingDataAccess>,
                TradingDataAccessFactory<TradingDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
        }
    }
}