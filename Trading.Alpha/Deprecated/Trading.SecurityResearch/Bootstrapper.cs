using Trading.Common;
using Trading.Common.Utils;
using Trading.SecurityResearch.ViewModels;

namespace Trading.SecurityResearch
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}