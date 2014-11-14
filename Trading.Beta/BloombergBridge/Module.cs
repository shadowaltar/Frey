using BloombergBridge.Data;
using BloombergBridge.ViewModels;
using Trading.Common;
using Trading.Common.Data;

namespace BloombergBridge
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<Access>, TradingDataAccessFactory<Access>>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Constants.InitializeDirectories();
        }
    }
}