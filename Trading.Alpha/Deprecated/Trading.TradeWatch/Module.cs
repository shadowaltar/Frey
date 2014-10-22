using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.TradeWatch.ViewModels;

namespace Trading.TradeWatch
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<TradeWatchDataAccess>,
                TradingDataAccessFactory<TradeWatchDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IFilterFlyoutViewModel>().To<FilterFlyoutViewModel>();
        }
    }
}