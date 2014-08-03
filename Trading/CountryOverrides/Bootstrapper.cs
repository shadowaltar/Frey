using Trading.Common;
using Trading.Common.Utils;
using Trading.CountryOverrides.ViewModels;

namespace Trading.CountryOverrides
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}