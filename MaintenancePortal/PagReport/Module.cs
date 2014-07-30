using Maintenance.Common.SharedSettings;
using Maintenance.PagReport.ViewModels;
using Maintenance.Common;
using Maintenance.Common.Data;

namespace Maintenance.PagReport
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            Bind<IMainViewModel>().To<MainViewModel>();
            TryBind<IDataAccessFactory<PagReportDataAccess>,
                ImapDataAccessFactory<PagReportDataAccess>>();
            TryBind<ISettings, ImapSettings>();
        }
    }
}