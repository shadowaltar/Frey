using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.CountryOverrides.Entities;

namespace Maintenance.CountryOverrides.ViewModels
{
    public interface ISecuritySearchResultViewModel
    {
        Security SelectedSecurity { get; set; }
        BindableCollection<Security> Securities { get; }
    }
}