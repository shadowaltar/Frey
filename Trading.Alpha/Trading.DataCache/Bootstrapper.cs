using System.Reflection;
using Trading.Common;
using Trading.Common.Data;
using Trading.DataCache.ViewModels;

namespace Trading.DataCache
{
    public class Bootstrapper : BootstrapperBase<IMainViewModel>
    {
        protected override void Bind()
        {
            Bind<IDataCache, Common.Data.DataCache>();
            Load<Module>();
        }
    }
}