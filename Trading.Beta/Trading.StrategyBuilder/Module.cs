using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.StrategyBuilder.Data;
using Trading.StrategyBuilder.ViewModels;

namespace Trading.StrategyBuilder
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<Access>, AccessFactory<Access>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Constants.InitializeDirectories();
        }
    }
}