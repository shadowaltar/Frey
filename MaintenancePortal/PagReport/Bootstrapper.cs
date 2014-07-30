using Maintenance.PagReport.ViewModels;
using Maintenance.Common;

namespace Maintenance.PagReport
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}