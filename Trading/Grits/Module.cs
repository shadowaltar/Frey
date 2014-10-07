using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.Grits.ViewModels;

namespace Maintenance.Grits
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<GritsDataAccess>, FmsDataAccessFactory<GritsDataAccess>>();
            TryBindSingleton<ISettings, FmsSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IAddFlyoutViewModel>().To<AddFlyoutViewModel>();
            Bind<IEditFundFlyoutViewModel>().To<EditFundFlyoutViewModel>();
            Bind<IEditBenchmarkFlyoutViewModel>().To<EditBenchmarkFlyoutViewModel>();
            Bind<IOptionsFlyoutViewModel>().To<OptionsFlyoutViewModel>();
        }
    }
}