using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.SharedSettings;
using Maintenance.CountryOverrides.ViewModels;

namespace Maintenance.CountryOverrides
{
    public class Module : ModuleBase
    {
        public override void Load()
        {
            TryBind<IDataAccessFactory<CountryOverrideDataAccess>,
                ImapDataAccessFactory<CountryOverrideDataAccess>>();
            TryBindSingleton<ISettings, ImapSettings>();

            Bind<IMainViewModel>().To<MainViewModel>();
            Bind<IAddFlyoutViewModel>().To<AddFlyoutViewModel>();
            Bind<IEditFlyoutViewModel>().To<EditFlyoutViewModel>();
            Bind<IFilterFlyoutViewModel>().To<FilterFlyoutViewModel>();
            Bind<IOptionsFlyoutViewModel>().To<OptionsFlyoutViewModel>();
            Bind<ISecuritySearchResultViewModel>().To<SecuritySearchResultViewModel>();
            Bind<ICustomCountryEditorViewModel>().To<CustomCountryEditorViewModel>();
        }
    }
}