using Trading.Common;
using Trading.Common.Data;
using Trading.DataDownload.ViewModels;

namespace Trading.DataDownload
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