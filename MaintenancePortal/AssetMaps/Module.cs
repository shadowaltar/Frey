using Maintenance.AssetMaps.ViewModels;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;

namespace Maintenance.AssetMaps
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<AssetMapDataAccess>,
                ImapDataAccessFactory<AssetMapDataAccess>>();
            TryBindSingleton<ISettings, ImapSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IOptionsFlyoutViewModel>().To<OptionsFlyoutViewModel>();
            Bind<IEditorViewModel>().To<EditorViewModel>();
        }
    }
}