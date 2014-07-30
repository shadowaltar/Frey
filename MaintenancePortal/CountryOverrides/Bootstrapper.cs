using Maintenance.Common;
using Maintenance.Common.Utils;
using Maintenance.CountryOverrides.ViewModels;

namespace Maintenance.CountryOverrides
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}