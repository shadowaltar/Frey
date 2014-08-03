using Maintenance.Common;
using Maintenance.Grits.ViewModels;

namespace Maintenance.Grits
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}