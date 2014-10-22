using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.SecurityScreener.ViewModels;

namespace Trading.SecurityScreener
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<SecurityScreenerDataAccess>,
                TradingDataAccessFactory<SecurityScreenerDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IUsMarketScreenerViewModel>().To<UsMarketScreenerViewModel>();
        }
    }
}