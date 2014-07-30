using Maintenance.Common.Utils;

namespace Maintenance.CountryOverrides.ViewModels
{
    public interface ICustomCountryEditorViewModel : IHasViewService, IHasDataAccessFactory<CountryOverrideDataAccess>
    {
    }
}