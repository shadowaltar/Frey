using Maintenance.Benchmarks.ViewModels;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Utils;

namespace Maintenance.Benchmarks
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Bind<IViewService, ViewService>();
            Bind<IDataAccessFactory, DataAccessFactory>();

            Load<Module>();
        }
    }
}