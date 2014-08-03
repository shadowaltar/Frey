using Caliburn.Micro;
using Trading.Common.Entities;
using Trading.CountryOverrides.Entities;

namespace Trading.CountryOverrides.ViewModels
{
    public interface ISecuritySearchResultViewModel
    {
        Security SelectedSecurity { get; set; }
        BindableCollection<Security> Securities { get; }
    }
}