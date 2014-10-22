using Trading.Common;
using Trading.SecurityScreener.ViewModels;

namespace Trading.SecurityScreener
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}