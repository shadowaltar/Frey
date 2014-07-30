using Maintenance.Common;
using Maintenance.Common.Utils;
using Maintenance.Portfolios.ViewModels;

namespace Maintenance.Portfolios
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}