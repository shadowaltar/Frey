using Trading.Common;
using Trading.Data.ViewModels;

namespace Trading.Data
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Load<Module>();
        }
    }
}