﻿using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.CountryOverrides.Entities;

namespace Maintenance.CountryOverrides.ViewModels
{
    public interface IFilterFlyoutViewModel
    {
        bool IsReady { get; set; }
        BindableCollection<Country> OriginalCountries { get; }
        BindableCollection<Country> NewCountries { get; }
        BindableCollection<Portfolio> Portfolios { get; }
        BindableCollection<PortfolioManager> PortfolioManagers { get; }
        BindableCollection<OverrideType> OverrideTypes { get; }
        OverrideType SelectedOverrideType { get; set; }
        string Name { get; set; }
        Country SelectedOriginalCountry { get; set; }
        Country SelectedNewCountry { get; set; }
        string Cusip { get; set; }
        string Sedol { get; set; }
        Portfolio SelectedPortfolio { get; set; }
        PortfolioManager SelectedPortfolioManager { get; set; }

        void ClearAll();
    }
}