using BloombergBridge.ViewModels;
using Trading.Common;

namespace BloombergBridge
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}