using Trading.Common;
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.CountryOverrides.ViewModels;

namespace Trading.CountryOverrides
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