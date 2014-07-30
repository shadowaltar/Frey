using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.CompositeBenchmarks.ViewModels;

namespace Maintenance.CompositeBenchmarks
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<CompositeBenchmarkDataAccess>,
                ImapDataAccessFactory<CompositeBenchmarkDataAccess>>();
            TryBindSingleton<ISettings, ImapSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IEditorViewModel>().To<EditorViewModel>();
            Bind<IOptionsFlyoutViewModel>().To<OptionsFlyoutViewModel>();
        }
    }
}