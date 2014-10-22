using Trading.Common;
using Trading.Common.Data;
using Trading.WorkExtractor.ViewModels;

namespace Trading.WorkExtractor
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