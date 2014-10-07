using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Utils;
using Trading.TradeWatch.ViewModels;

namespace Trading.TradeWatch
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