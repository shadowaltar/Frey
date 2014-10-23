using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.DataCache.Data;
using Trading.DataCache.ViewModels;

namespace Trading.DataCache
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<CacheDataAccess>, CacheDataAccessFactory<CacheDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
        }
    }
}