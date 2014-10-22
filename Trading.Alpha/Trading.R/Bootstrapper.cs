using Trading.Common;
using Trading.Common.Data;
using Trading.R.ViewModels;

namespace Trading.R
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            BindSingleton<IDataCache, DataCache>();
            Load<Module>();
        }
    }
}