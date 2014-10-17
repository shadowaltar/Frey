using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.DataDownload.ViewModels;

namespace Trading.DataDownload
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<LoaderDataAccess>,
                TradingDataAccessFactory<LoaderDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
        }
    }
}