using Maintenance.Common;
using Maintenance.Common.Utils;
using Maintenance.CompositeBenchmarks.ViewModels;

namespace Maintenance.CompositeBenchmarks
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}