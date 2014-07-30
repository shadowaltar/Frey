using Maintenance.AssetMaps.ViewModels;
using Maintenance.Common;
using Maintenance.Common.Utils;

namespace Maintenance.AssetMaps
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}