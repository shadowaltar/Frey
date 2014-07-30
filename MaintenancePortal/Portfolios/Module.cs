using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.Portfolios.ViewModels;

namespace Maintenance.Portfolios
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<PortfolioDataAccess>,
                ImapDataAccessFactory<PortfolioDataAccess>>();
            TryBindSingleton<ISettings, ImapSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IEditFlyoutViewModel>().To<EditFlyoutViewModel>();
            Bind<IBenchmarkAssociationFlyoutViewModel>().To<BenchmarkAssociationFlyoutViewModel>();
            Bind<IFilterFlyoutViewModel>().To<FilterFlyoutViewModel>();
            Bind<IOptionsFlyoutViewModel>().To<OptionsFlyoutViewModel>();
            Bind<IBenchmarkHistoryEditorViewModel>().To<BenchmarkHistoryEditorViewModel>();
        }
    }
}