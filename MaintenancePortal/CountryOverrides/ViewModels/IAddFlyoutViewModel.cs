﻿using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.CountryOverrides.Entities;
using System.Collections.Generic;
using System.Windows.Input;

namespace Maintenance.CountryOverrides.ViewModels
{
    public interface IAddFlyoutViewModel : IHasViewService,
        IHasDataAccessFactory<CountryOverrideDataAccess>
    {
        bool IsReady { get; set; }

        bool ShowBelongsToPortfolioManager { get; set; }
        bool ShowBelongsToPortfolio { get; set; }
        bool ShowBelongsTo { get; set; }

        OverrideType? SelectedOverrideType { get; set; }
        Country SelectedCountry { get; set; }
        Portfolio SelectedPortfolio { get; set; }
        PortfolioManager SelectedPortfolioManager { get; set; }

        BindableCollection<Country> Countries { get; }
        BindableCollection<Portfolio> Portfolios { get; }
        BindableCollection<PortfolioManager> PortfolioManagers { get; }
        BindableCollection<OverrideType> OverrideTypes { get; }
        List<Security> SearchedSecurities { get; }
        List<OverrideItem> ExistingItems { get; set; }
        void SearchByCusip(KeyEventArgs keyArgs);
        void SearchBySedol(KeyEventArgs keyArgs);
        void SearchByName(KeyEventArgs keyArgs);
        void Save();
        void ClearAll();
    }
}