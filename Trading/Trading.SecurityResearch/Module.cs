using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.SecurityResearch.ViewModels;

namespace Trading.SecurityResearch
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<SecuritiesDataAccess>,
                TradingDataAccessFactory<SecuritiesDataAccess>>();
            TryBindSingleton<ISettings, InfrastructureSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IFilterFlyoutViewModel>().To<FilterFlyoutViewModel>();
            Bind<IResearchReportViewModel>().To<ResearchReportViewModel>();
        }
    }
}