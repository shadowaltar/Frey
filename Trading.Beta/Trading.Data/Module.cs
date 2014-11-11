using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Data.Data;
using Trading.Data.ViewModels;

namespace Trading.Data
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<Access>, AccessFactory<Access>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IDataCacheViewModel>().To<DataCacheViewModel>();
            Bind<IDatabaseViewModel>().To<DatabaseViewModel>();
            Bind<IDownloadViewModel>().To<DownloadViewModel>();
            Bind<IInteractiveBrokersViewModel>().To<InteractiveBrokersViewModel>();
            Bind<IMainViewModel>().To<MainViewModel>();
            Constants.InitializeDirectories();
        }
    }
}